using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using HalconDotNet;
using InspectionProgram.Common;

namespace InspectionProgram.Halcon
{
    /// <summary>
    /// NCC 매칭 자세를 골든(참조) 자세에 맞추어 이미지를 강체 변환으로 정렬합니다(방법 A).
    /// </summary>
    public static class NccImageAlignment
    {
        private static double AngleAbsErrorRad(double a, double b)
        {
            // minimal absolute difference with 2*pi wrap
            double d = a - b;
            double pi2 = Math.PI * 2.0;
            while (d > Math.PI) d -= pi2;
            while (d < -Math.PI) d += pi2;
            return Math.Abs(d);
        }

        /// <summary>
        /// 원본 파일을 읽어 참조 자세로 정렬한 뒤 비트맵으로 반환합니다.
        /// 변환 방향이 애매한 경우(좌표계/부호), 정렬 결과를 다시 NCC로 재매칭해 각도 오차가 더 작은 쪽을 채택합니다.
        /// 실패 시 null과 예외를 설정합니다.
        /// </summary>
        public static Bitmap AlignImageFileToReferenceBitmap(
            string imagePath,
            string modelFilePath,
            double templateWidth,
            double templateHeight,
            double matchRow,
            double matchCol,
            double matchAngle,
            double refRow,
            double refCol,
            double refAngle,
            out Exception error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(imagePath) || File.Exists(imagePath) == false)
            {
                error = new FileNotFoundException("Image not found.", imagePath);
                return null;
            }
            if (string.IsNullOrWhiteSpace(modelFilePath) || File.Exists(modelFilePath) == false)
            {
                error = new FileNotFoundException("NCC model file not found.", modelFilePath);
                return null;
            }

            HObject hoIn = null;
            HObject hoOut1 = null;
            HObject hoOut2 = null;
            string temp1 = null;
            string temp2 = null;
            try
            {
                AppLogger.Write(
                    "ALIGN",
                    string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "Align start: img={0}, match(rc,a)=({1:0.00},{2:0.00},{3:0.000}), ref(rc,a)=({4:0.00},{5:0.00},{6:0.000})",
                        Path.GetFileName(imagePath),
                        matchRow, matchCol, matchAngle,
                        refRow, refCol, refAngle));

                HOperatorSet.ReadImage(out hoIn, imagePath);

                // 명시적 강체 변환: Δθ 회전 + 중심 이동
                // (Row, Col) = (Y, X)
                double delta = refAngle - matchAngle;
                double deltaAlt = -delta; // angle 부호 규약이 반대인 경우 대비

                HTuple hvH0;
                HOperatorSet.HomMat2dIdentity(out hvH0);

                HTuple hvR1;
                HOperatorSet.HomMat2dRotate(hvH0, new HTuple(delta), new HTuple(matchRow), new HTuple(matchCol), out hvR1);
                // hom_mat2d_translate: Tx(=Column/X), Ty(=Row/Y)
                HTuple hvT1;
                HOperatorSet.HomMat2dTranslate(hvR1, new HTuple(refCol - matchCol), new HTuple(refRow - matchRow), out hvT1);

                HTuple hvR2;
                HOperatorSet.HomMat2dRotate(hvH0, new HTuple(deltaAlt), new HTuple(matchRow), new HTuple(matchCol), out hvR2);
                HTuple hvT2;
                HOperatorSet.HomMat2dTranslate(hvR2, new HTuple(refCol - matchCol), new HTuple(refRow - matchRow), out hvT2);

                HOperatorSet.AffineTransImage(hoIn, out hoOut1, hvT1, "bilinear", "false");
                HOperatorSet.AffineTransImage(hoIn, out hoOut2, hvT2, "bilinear", "false");

                temp1 = Path.Combine(Path.GetTempPath(), "VisionInspectAlign_" + Guid.NewGuid().ToString("N") + "_1.bmp");
                temp2 = Path.Combine(Path.GetTempPath(), "VisionInspectAlign_" + Guid.NewGuid().ToString("N") + "_2.bmp");
                // write_image: (Image, Format, FillColor/Quality, FileName) — BMP는 품질값이 의미 없으므로 0 전달
                HOperatorSet.WriteImage(hoOut1, "bmp", 0, temp1);
                HOperatorSet.WriteImage(hoOut2, "bmp", 0, temp2);

                double err1 = double.PositiveInfinity;
                double err2 = double.PositiveInfinity;
                try
                {
                    var r1 = NccPatternInspector.MatchFromModelFile(temp1, modelFilePath, 0.5, templateWidth, templateHeight);
                    if (r1 != null && r1.Count > 0)
                    {
                        err1 = AngleAbsErrorRad(r1.Angle[0].D, refAngle);
                    }
                }
                catch
                {
                }

                try
                {
                    var r2 = NccPatternInspector.MatchFromModelFile(temp2, modelFilePath, 0.5, templateWidth, templateHeight);
                    if (r2 != null && r2.Count > 0)
                    {
                        err2 = AngleAbsErrorRad(r2.Angle[0].D, refAngle);
                    }
                }
                catch
                {
                }

                string chosen = err1 <= err2 ? temp1 : temp2;
                AppLogger.Write(
                    "ALIGN",
                    string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "Align choose: err1={0:0.000}, err2={1:0.000}, chosen={2}",
                        err1, err2, chosen == temp1 ? "H" : "H_alt"));
                using (FileStream fs = new FileStream(chosen, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (Image src = Image.FromStream(fs))
                {
                    Bitmap converted = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(converted))
                        g.DrawImage(src, 0, 0, src.Width, src.Height);
                    return converted;
                }
            }
            catch (Exception ex)
            {
                error = ex;
                AppLogger.Write("ALIGN", "Align failed: " + ex.Message);
                return null;
            }
            finally
            {
                try
                {
                    hoIn?.Dispose();
                    hoOut1?.Dispose();
                    hoOut2?.Dispose();
                }
                catch
                {
                }

                try { if (!string.IsNullOrEmpty(temp1) && File.Exists(temp1)) File.Delete(temp1); } catch { }
                try { if (!string.IsNullOrEmpty(temp2) && File.Exists(temp2)) File.Delete(temp2); } catch { }
            }
        }
    }
}
