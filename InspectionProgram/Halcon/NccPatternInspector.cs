using System;
using System.Drawing;
using System.IO;
using HalconDotNet;

namespace InspectionProgram.Halcon
{
    /// <summary>NCC: 골든 ROI로 모델을 만들어 저장하거나, 저장된 모델로 현재 이미지를 검사합니다.</summary>
    public static class NccPatternInspector
    {
        private const string NumLevels = "auto";
        private const string Optimization = "auto";
        private const string Metric = "use_polarity";

        // Halcon 10 — <see cref="HDevelopExport.Run"/> / action() 과 동일한 NCC 수치
        // 회전 탐색 범위는 과도하게 키우면(360도 등) 성능 저하가 커서 기본값(기존 프로젝트 값)을 유지합니다.
        private const double AngleStart = -0.39;
        private const double AngleExtent = 0.79;
        private const int Greediness = 0;
        private const string SubPixel = "true";
        private const int LastNum = 0;

        /// <summary>골든 이미지와 ROI로 NCC 모델을 만들어 파일로 저장합니다.</summary>
        public static void CreateAndWriteModel(
            string imagePath,
            double row1,
            double col1,
            double row2,
            double col2,
            string modelFilePath,
            out double templateWidth,
            out double templateHeight)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || File.Exists(imagePath) == false)
                throw new FileNotFoundException("Image not found.", imagePath);
            if (string.IsNullOrWhiteSpace(modelFilePath))
                throw new ArgumentException("Model path is required.", nameof(modelFilePath));

            HObject ho_Raw = null;
            HObject ho_Gray = null;
            HObject ho_Tmp = null;
            HObject ho_R = null;
            HObject ho_G = null;
            HObject ho_B = null;
            HObject ho_A = null;
            HObject ho_Rgb = null;
            HObject ho_Roi = null;
            HObject ho_Reduced = null;
            HTuple hv_ModelId = new HTuple();
            HOperatorSet.GenEmptyObj(out ho_Raw);
            HOperatorSet.GenEmptyObj(out ho_Gray);
            HOperatorSet.GenEmptyObj(out ho_Tmp);
            HOperatorSet.GenEmptyObj(out ho_R);
            HOperatorSet.GenEmptyObj(out ho_G);
            HOperatorSet.GenEmptyObj(out ho_B);
            HOperatorSet.GenEmptyObj(out ho_A);
            HOperatorSet.GenEmptyObj(out ho_Rgb);
            HOperatorSet.GenEmptyObj(out ho_Roi);
            HOperatorSet.GenEmptyObj(out ho_Reduced);
            string dir = Path.GetDirectoryName(modelFilePath);
            if (string.IsNullOrEmpty(dir) == false)
                Directory.CreateDirectory(dir);

            try
            {
                HOperatorSet.ReadImage(out ho_Raw, imagePath);
                HOperatorSet.GenRectangle1(out ho_Roi, row1, col1, row2, col2);
                HOperatorSet.ReduceDomain(ho_Raw, ho_Roi, out ho_Reduced);

                ToByteGray(
                    ho_Reduced,
                    ref ho_Gray,
                    ref ho_Tmp,
                    ref ho_R,
                    ref ho_G,
                    ref ho_B,
                    ref ho_A,
                    ref ho_Rgb);

                HOperatorSet.CreateNccModel(
                    ho_Gray,
                    NumLevels,
                    AngleStart,
                    AngleExtent,
                    Optimization,
                    Metric,
                    out hv_ModelId);

                HOperatorSet.WriteNccModel(hv_ModelId, new HTuple(modelFilePath));
                HOperatorSet.ClearNccModel(hv_ModelId);
                hv_ModelId = null;

                // 표시/검사용: 사용자 ROI 겉크기(픽셀)
                templateWidth = col2 - col1 + 1.0;
                if (templateWidth < 1.0)
                    templateWidth = 1.0;
                templateHeight = row2 - row1 + 1.0;
                if (templateHeight < 1.0)
                    templateHeight = 1.0;
            }
            finally
            {
                if (hv_ModelId != null && hv_ModelId.TupleLength() > 0)
                {
                    try
                    {
                        HOperatorSet.ClearNccModel(hv_ModelId);
                    }
                    catch
                    {
                    }
                }

                ho_Raw.Dispose();
                ho_Gray.Dispose();
                ho_Tmp.Dispose();
                ho_R.Dispose();
                ho_G.Dispose();
                ho_B.Dispose();
                ho_A.Dispose();
                ho_Rgb.Dispose();
                ho_Roi.Dispose();
                ho_Reduced.Dispose();
            }
        }

        /// <param name="minScoreInFind">find_ncc_model의 내부 점수 하한(기본 0.5, <see cref="HDevelopExport.Run"/> 과 동일).</param>
        /// <param name="templateWidth">티칭 시 ROI 가로(픽셀). 0이면 박스 대신 십자만 그릴 수 있습니다.</param>
        /// <param name="templateHeight">티칭 시 ROI 세로(픽셀).</param>
        /// <param name="nccFindMinScore">find_ncc_model 5번째 MinScore(매칭 후보 하한). 전체 다중 매칭 시 낮춰 후보를 더 받습니다(기본 0.8).</param>
        public static HalconResult MatchFromModelFile(
            string imagePath,
            string modelFilePath,
            double minScoreInFind = 0.5,
            double templateWidth = 0.0,
            double templateHeight = 0.0,
            double nccFindMinScore = 0.8)
        {
            if (nccFindMinScore < 0.0)
                nccFindMinScore = 0.0;
            if (nccFindMinScore > 1.0)
                nccFindMinScore = 1.0;
            if (string.IsNullOrWhiteSpace(modelFilePath) || File.Exists(modelFilePath) == false)
                throw new FileNotFoundException("NCC model file not found.", modelFilePath);
            if (string.IsNullOrWhiteSpace(imagePath) || File.Exists(imagePath) == false)
                throw new FileNotFoundException("Image not found.", imagePath);

            HObject ho_Raw = null;
            HObject ho_Gray = null;
            HObject ho_Tmp = null;
            HObject ho_R = null;
            HObject ho_G = null;
            HObject ho_B = null;
            HObject ho_A = null;
            HObject ho_Rgb = null;
            HTuple hv_ModelId = new HTuple();
            HTuple hv_Row, hv_Col, hv_Angle, hv_Score, hv_Pred;
            HOperatorSet.GenEmptyObj(out ho_Raw);
            HOperatorSet.GenEmptyObj(out ho_Gray);
            HOperatorSet.GenEmptyObj(out ho_Tmp);
            HOperatorSet.GenEmptyObj(out ho_R);
            HOperatorSet.GenEmptyObj(out ho_G);
            HOperatorSet.GenEmptyObj(out ho_B);
            HOperatorSet.GenEmptyObj(out ho_A);
            HOperatorSet.GenEmptyObj(out ho_Rgb);
            var result = new HalconResult();
            try
            {
                HOperatorSet.ReadImage(out ho_Raw, imagePath);
                HOperatorSet.ReadNccModel(new HTuple(modelFilePath), out hv_ModelId);
                result.TemplateWidth = templateWidth;
                result.TemplateHeight = templateHeight;
                ToByteGray(
                    ho_Raw,
                    ref ho_Gray,
                    ref ho_Tmp,
                    ref ho_R,
                    ref ho_G,
                    ref ho_B,
                    ref ho_A,
                    ref ho_Rgb);

                HOperatorSet.FindNccModel(
                    ho_Gray,
                    hv_ModelId,
                    AngleStart,
                    AngleExtent,
                    nccFindMinScore,
                    Greediness,
                    minScoreInFind,
                    SubPixel,
                    LastNum,
                    out hv_Row,
                    out hv_Col,
                    out hv_Angle,
                    out hv_Score);

                HOperatorSet.TupleLength(hv_Score, out hv_Pred);
                result.Count = hv_Pred.I;
                result.Row = hv_Row;
                result.Column = hv_Col;
                result.Angle = hv_Angle;
                result.Score = hv_Score;
                HOperatorSet.ClearNccModel(hv_ModelId);
                hv_ModelId = null;
                return result;
            }
            finally
            {
                if (hv_ModelId != null && hv_ModelId.TupleLength() > 0)
                {
                    try
                    {
                        HOperatorSet.ClearNccModel(hv_ModelId);
                    }
                    catch
                    {
                    }
                }

                ho_Raw.Dispose();
                ho_Gray.Dispose();
                ho_Tmp.Dispose();
                ho_R.Dispose();
                ho_G.Dispose();
                ho_B.Dispose();
                ho_A.Dispose();
                ho_Rgb.Dispose();
            }
        }

        /// <summary>Test1.ToByteGray와 동일한 그레이 변환(1/3/4채널).</summary>
        private static void ToByteGray(
            HObject raw,
            ref HObject grayByte,
            ref HObject tmp,
            ref HObject r,
            ref HObject g,
            ref HObject b,
            ref HObject a,
            ref HObject rgb)
        {
            HTuple ch;
            HOperatorSet.CountChannels(raw, out ch);
            if ((int)(new HTuple(ch.TupleEqual(1))) != 0)
            {
                grayByte.Dispose();
                HOperatorSet.ConvertImageType(raw, out grayByte, "byte");
                return;
            }

            if ((int)(new HTuple(ch.TupleEqual(3))) != 0)
            {
                tmp.Dispose();
                HOperatorSet.Rgb1ToGray(raw, out tmp);
                grayByte.Dispose();
                HOperatorSet.ConvertImageType(tmp, out grayByte, "byte");
                return;
            }

            r.Dispose();
            g.Dispose();
            b.Dispose();
            a.Dispose();
            HOperatorSet.Decompose4(raw, out r, out g, out b, out a);
            rgb.Dispose();
            HOperatorSet.Compose3(r, g, b, out rgb);
            tmp.Dispose();
            HOperatorSet.Rgb1ToGray(rgb, out tmp);
            grayByte.Dispose();
            HOperatorSet.ConvertImageType(tmp, out grayByte, "byte");
        }

        /// <summary>Halcon (Row, Column) 중심, 템플릿 가로·세로(픽셀)로 이미지 좌표 사각형을 구합니다. Row=Y, Column=X.</summary>
        public static RectangleF GetMatchImageRectFromCenteredTemplate(double row, double col, double tw, double th)
        {
            if (tw < 1.0) tw = 1.0;
            if (th < 1.0) th = 1.0;
            return new RectangleF(
                (float)(col - tw * 0.5),
                (float)(row - th * 0.5),
                (float)tw,
                (float)th);
        }
    }
}
