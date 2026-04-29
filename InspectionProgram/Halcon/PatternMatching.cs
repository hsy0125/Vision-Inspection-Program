using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InspectionProgram.Halcon
{
    public partial class HDevelopExport
    {
#if !NO_EXPORT_APP_MAIN
        public HDevelopExport()
        {
            // Default settings used in HDevelop 
            HOperatorSet.SetSystem("do_low_error", "false");
            //action();
        }
#endif

        // Main procedure 

        public HalconResult Run(string imagePath)
        {
            HObject ho_Image, ho_ROI_0, ho_ImageReduced;
            HTuple hv_PatternModelID = new HTuple();
            HTuple hv_Row, hv_Column, hv_Angle, hv_Score, hv_pred_count;

            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ROI_0);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);

            HalconResult result = new HalconResult();

            const double TeachRow1 = 495.358;
            const double TeachCol1 = 812.188;
            const double TeachRow2 = 577.756;
            const double TeachCol2 = 894.578;

            try
            {
                HOperatorSet.ReadImage(out ho_Image, imagePath);

                result.TemplateWidth = TeachCol2 - TeachCol1;
                result.TemplateHeight = TeachRow2 - TeachRow1;

                HOperatorSet.GenRectangle1(out ho_ROI_0, TeachRow1, TeachCol1, TeachRow2, TeachCol2);
                HOperatorSet.ReduceDomain(ho_Image, ho_ROI_0, out ho_ImageReduced);

                HOperatorSet.CreateNccModel(
                    ho_ImageReduced,
                    "auto",
                    -0.39,
                    0.79,
                    "auto",
                    "use_polarity",
                    out hv_PatternModelID
                );

                HOperatorSet.FindNccModel(
                    ho_Image,
                    hv_PatternModelID,
                    -0.39,
                    0.79,
                    0.8,
                    0,
                    0.5,
                    "true",
                    0,
                    out hv_Row,
                    out hv_Column,
                    out hv_Angle,
                    out hv_Score
                );

                HOperatorSet.TupleLength(hv_Score, out hv_pred_count);

                result.Count = hv_pred_count.I;
                result.Row = hv_Row;
                result.Column = hv_Column;
                result.Angle = hv_Angle;
                result.Score = hv_Score;

                return result;
            }
            finally
            {
                try
                {
                    if (hv_PatternModelID != null && hv_PatternModelID.TupleLength() > 0)
                        HOperatorSet.ClearNccModel(hv_PatternModelID);
                }
                catch
                {
                }

                ho_Image.Dispose();
                ho_ROI_0.Dispose();
                ho_ImageReduced.Dispose();
            }
        }
        private void action()
        {

            // Local iconic variables 

            HObject ho_Image, ho_ROI_0, ho_ImageReduced;
            HObject ho_SortedRegions;


            // Local control variables 

            HTuple hv_Width, hv_Height, hv_WindowHandle;
            HTuple hv_PatternModelID, hv_Row, hv_Column, hv_Angle;
            HTuple hv_Score, hv_pred_count, hv_i;

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ROI_0);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);

            //1. Pattern Matching *

            //1. 이미지 불러오기
            //read_image (Image, 'C:/Users/USER/Desktop/halcon_exam/예제 이미지/Test Image/10. PRS.tif')
            ho_Image.Dispose();
            HOperatorSet.ReadImage(out ho_Image, @"C:\Users\user\Desktop\study\HalconEx\10. PRS");
            
            //2. roi 추출
            ho_ROI_0.Dispose();
            HOperatorSet.GenRectangle1(out ho_ROI_0, 495.358, 812.188, 577.756, 894.578);
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(ho_Image, ho_ROI_0, out ho_ImageReduced);

            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            HOperatorSet.SetWindowAttr("background_color", "black");
            HOperatorSet.OpenWindow(0, 0, hv_Width * 0.3, hv_Height * 0.3, 0, "", "", out hv_WindowHandle);
            HDevWindowStack.Push(hv_WindowHandle);
            if (HDevWindowStack.IsOpen())
            {
                HOperatorSet.DispObj(ho_ImageReduced, HDevWindowStack.GetActive());
            }

            //3. 패턴 매칭

            //1) 패턴 매칭을 하기위해 모델을 생성
            //Model()
            HOperatorSet.CreateNccModel(ho_ImageReduced, "auto", -0.39, 0.79, "auto", "use_polarity",
                out hv_PatternModelID);

            //2) 생성된 모델을 이용해 패턴을 찾음
            //Model.predict()
            //[수정됨] Halcon 10: find_ncc_models -> find_ncc_model (단수형 사용, 마지막 Model 인자 제거)
            HOperatorSet.FindNccModel(ho_Image, hv_PatternModelID, -0.39, 0.79, 0.8, 0, 0.5,
                "true", 0, out hv_Row, out hv_Column, out hv_Angle, out hv_Score);

            //모델의 아웃풋 형식
            ho_SortedRegions.Dispose();
            HOperatorSet.SortRegion(ho_ROI_0, out ho_SortedRegions, "first_point", "true",
                "row");
            if (HDevWindowStack.IsOpen())
            {
                HOperatorSet.DispObj(ho_SortedRegions, HDevWindowStack.GetActive());
            }

            if (HDevWindowStack.IsOpen())
            {
                HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
            }

            HOperatorSet.TupleLength(hv_Score, out hv_pred_count);

            //[디버깅용]
            System.Windows.Forms.MessageBox.Show("검출 개수: " + hv_pred_count.ToString());
            //인덱스    0 ~

            //반복횟수  1 ~
            //* for i := 1 to pred_count-1 by 1

            HOperatorSet.SetColor(hv_WindowHandle, "red");

            //3) 여러개를 찾는데 하나씩 for문 돌리면서 display
            for (hv_i = 0; hv_i.Continue(hv_pred_count - 1, 1); hv_i = hv_i.TupleAdd(1))
            {
                //y좌표     x좌표  dotSize , 검출된 각도
                HOperatorSet.DispCross(hv_WindowHandle, hv_Row.TupleSelect(hv_i), hv_Column.TupleSelect(
                    hv_i), 50, hv_Angle.TupleSelect(hv_i));
                //disp_cross 만 하니까 핸들러에만 띄워줘서 검은배경에 빨간 점만 찍힘
                //핸들러에 찍어주는 거니까 해당 핸들러에 이미지 미리 띄워놓고 점 찍게함 // dev_display(Image) 추가
            }
            ho_Image.Dispose();
            ho_ROI_0.Dispose();
            ho_ImageReduced.Dispose();
            ho_SortedRegions.Dispose();

        }


    }


}
