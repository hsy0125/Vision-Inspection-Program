//
// Golden vs current template diff (Halcon logic, WinForms-friendly).
// Not partial HDevelopExport / no second Main entry point.
//

using System;
using System.Collections.Generic;
using System.Drawing;
using HalconDotNet;

namespace InspectionProgram.Halcon
{
    /// <summary>Compare golden and current images; return defect axis-aligned boxes.</summary>
    public sealed class GoldenTemplateDiffInspector
    {
        public double ThDiff { get; set; } = 30.0;
        public double MinDefectArea { get; set; } = 50.0;
        public double OpeningRadius { get; set; } = 2.0;
        public double ClosingRadius { get; set; } = 4.0;

        /// <summary>Defect boxes in image coordinates (X = column, Y = row).</summary>
        public IReadOnlyList<RectangleF> Run(string goldenImagePath, string currentImagePath)
        {
            if (string.IsNullOrWhiteSpace(goldenImagePath))
                throw new ArgumentException("golden path required", nameof(goldenImagePath));
            if (string.IsNullOrWhiteSpace(currentImagePath))
                throw new ArgumentException("current path required", nameof(currentImagePath));

            var boxes = new List<RectangleF>();

            HObject ho_ImageGoldRaw = null;
            HObject ho_ImageCurRaw = null;
            HObject ho_ImageGold = null;
            HObject ho_TmpG = null;
            HObject ho_Rg = null;
            HObject ho_Gg = null;
            HObject ho_Bg = null;
            HObject ho_Ag = null;
            HObject ho_ImageRgbG = null;
            HObject ho_ImageCur = null;
            HObject ho_TmpC = null;
            HObject ho_Rc = null;
            HObject ho_Gc = null;
            HObject ho_Bc = null;
            HObject ho_Ac = null;
            HObject ho_ImageRgbC = null;
            HObject ho_ImageCurSz = null;
            HObject ho_ImageDiff = null;
            HObject ho_RegionDiffRaw = null;
            HObject ho_RegionDiffOpen = null;
            HObject ho_RegionDiff = null;
            HObject ho_DefectParts = null;
            HObject ho_DefectBoxSrc = null;
            HObject ho_OneDef = null;

            try
            {
                HOperatorSet.GenEmptyObj(out ho_ImageGoldRaw);
                HOperatorSet.GenEmptyObj(out ho_ImageCurRaw);
                HOperatorSet.GenEmptyObj(out ho_ImageGold);
                HOperatorSet.GenEmptyObj(out ho_TmpG);
                HOperatorSet.GenEmptyObj(out ho_Rg);
                HOperatorSet.GenEmptyObj(out ho_Gg);
                HOperatorSet.GenEmptyObj(out ho_Bg);
                HOperatorSet.GenEmptyObj(out ho_Ag);
                HOperatorSet.GenEmptyObj(out ho_ImageRgbG);
                HOperatorSet.GenEmptyObj(out ho_ImageCur);
                HOperatorSet.GenEmptyObj(out ho_TmpC);
                HOperatorSet.GenEmptyObj(out ho_Rc);
                HOperatorSet.GenEmptyObj(out ho_Gc);
                HOperatorSet.GenEmptyObj(out ho_Bc);
                HOperatorSet.GenEmptyObj(out ho_Ac);
                HOperatorSet.GenEmptyObj(out ho_ImageRgbC);
                HOperatorSet.GenEmptyObj(out ho_ImageCurSz);
                HOperatorSet.GenEmptyObj(out ho_ImageDiff);
                HOperatorSet.GenEmptyObj(out ho_RegionDiffRaw);
                HOperatorSet.GenEmptyObj(out ho_RegionDiffOpen);
                HOperatorSet.GenEmptyObj(out ho_RegionDiff);
                HOperatorSet.GenEmptyObj(out ho_DefectParts);
                HOperatorSet.GenEmptyObj(out ho_DefectBoxSrc);
                HOperatorSet.GenEmptyObj(out ho_OneDef);

                ho_ImageGoldRaw.Dispose();
                HOperatorSet.ReadImage(out ho_ImageGoldRaw, goldenImagePath);
                ho_ImageCurRaw.Dispose();
                HOperatorSet.ReadImage(out ho_ImageCurRaw, currentImagePath);

                ToByteGray(ho_ImageGoldRaw, ref ho_ImageGold, ref ho_TmpG, ref ho_Rg, ref ho_Gg, ref ho_Bg, ref ho_Ag, ref ho_ImageRgbG);
                ToByteGray(ho_ImageCurRaw, ref ho_ImageCur, ref ho_TmpC, ref ho_Rc, ref ho_Gc, ref ho_Bc, ref ho_Ac, ref ho_ImageRgbC);

                HTuple hv_W, hv_H, hv_Wc, hv_Hc;
                HOperatorSet.GetImageSize(ho_ImageGold, out hv_W, out hv_H);
                HOperatorSet.GetImageSize(ho_ImageCur, out hv_Wc, out hv_Hc);

                ho_ImageCurSz.Dispose();
                if ((int)(new HTuple(hv_Wc.TupleNotEqual(hv_W))) != 0)
                    HOperatorSet.ZoomImageSize(ho_ImageCur, out ho_ImageCurSz, hv_W, hv_H, "bilinear");
                else if ((int)(new HTuple(hv_Hc.TupleNotEqual(hv_H))) != 0)
                    HOperatorSet.ZoomImageSize(ho_ImageCur, out ho_ImageCurSz, hv_W, hv_H, "bilinear");
                else
                    HOperatorSet.CopyImage(ho_ImageCur, out ho_ImageCurSz);

                ho_ImageDiff.Dispose();
                HOperatorSet.AbsDiffImage(ho_ImageGold, ho_ImageCurSz, out ho_ImageDiff, 1.0);

                ho_RegionDiffRaw.Dispose();
                HOperatorSet.Threshold(ho_ImageDiff, out ho_RegionDiffRaw, ThDiff, 255);
                ho_RegionDiffOpen.Dispose();
                HOperatorSet.OpeningCircle(ho_RegionDiffRaw, out ho_RegionDiffOpen, OpeningRadius);
                ho_RegionDiff.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionDiffOpen, out ho_RegionDiff, ClosingRadius);
                ho_DefectParts.Dispose();
                HOperatorSet.Connection(ho_RegionDiff, out ho_DefectParts);
                ho_DefectBoxSrc.Dispose();
                HOperatorSet.SelectShape(ho_DefectParts, out ho_DefectBoxSrc, "area", "and", MinDefectArea, 999999999);

                HTuple hv_NDef;
                HOperatorSet.CountObj(ho_DefectBoxSrc, out hv_NDef);
                int n = hv_NDef.I;
                for (int idx = 1; idx <= n; idx++)
                {
                    ho_OneDef.Dispose();
                    HOperatorSet.SelectObj(ho_DefectBoxSrc, out ho_OneDef, idx);
                    HTuple hv_R1, hv_C1, hv_R2, hv_C2;
                    HOperatorSet.SmallestRectangle1(ho_OneDef, out hv_R1, out hv_C1, out hv_R2, out hv_C2);
                    double r1 = hv_R1.D;
                    double c1 = hv_C1.D;
                    double r2 = hv_R2.D;
                    double c2 = hv_C2.D;
                    float left = (float)Math.Min(c1, c2);
                    float top = (float)Math.Min(r1, r2);
                    float right = (float)Math.Max(c1, c2);
                    float bottom = (float)Math.Max(r1, r2);
                    boxes.Add(RectangleF.FromLTRB(left, top, right, bottom));
                }

                return boxes;
            }
            finally
            {
                ho_OneDef?.Dispose();
                ho_DefectBoxSrc?.Dispose();
                ho_DefectParts?.Dispose();
                ho_RegionDiff?.Dispose();
                ho_RegionDiffOpen?.Dispose();
                ho_RegionDiffRaw?.Dispose();
                ho_ImageDiff?.Dispose();
                ho_ImageCurSz?.Dispose();
                ho_ImageRgbC?.Dispose();
                ho_Ac?.Dispose();
                ho_Bc?.Dispose();
                ho_Gc?.Dispose();
                ho_Rc?.Dispose();
                ho_TmpC?.Dispose();
                ho_ImageCur?.Dispose();
                ho_ImageRgbG?.Dispose();
                ho_Ag?.Dispose();
                ho_Bg?.Dispose();
                ho_Gg?.Dispose();
                ho_Rg?.Dispose();
                ho_TmpG?.Dispose();
                ho_ImageGold?.Dispose();
                ho_ImageCurRaw?.Dispose();
                ho_ImageGoldRaw?.Dispose();
            }
        }

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
    }
}
