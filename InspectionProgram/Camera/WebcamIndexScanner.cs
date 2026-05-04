using OpenCvSharp;
using System;
using System.Text;
using System.Threading;

namespace InspectionProgram.Camera
{
    /// <summary>
    /// OpenCV 비디오 인덱스별로 짧게 열어 프레임 가능 여부를 문자열로 모읍니다.
    /// </summary>
    internal static class WebcamIndexScanner
    {
        public static string BuildScanLog(int maxExclusive = 8, int targetFps = 30)
        {
            var sb = new StringBuilder();
            sb.AppendLine("--- OpenCV camera index ---");
            for (int i = 0; i < maxExclusive; i++)
                sb.AppendLine(ProbeLine(i, targetFps));
            sb.AppendLine("Use the tab with the same number as [n], then start Live.");
            return sb.ToString();
        }

        private static string ProbeLine(int index, int targetFps)
        {
            VideoCapture cap = OpenCvWebcamFrameSource.TryOpenWebcam(index, targetFps);
            if (cap == null)
                return "[ " + index + " ] cannot open";

            try
            {
                using (var mat = new Mat())
                {
                    for (int k = 0; k < 24; k++)
                    {
                        if (OpenCvWebcamFrameSource.TryReadFrame(cap, mat)
                            && mat.Width > 2
                            && mat.Height > 2)
                            return "[ " + index + " ] OK  " + mat.Width + "x" + mat.Height;

                        Thread.Sleep(28);
                    }

                    return "[ " + index + " ] opened, no usable frame (close other apps using the camera)";
                }
            }
            finally
            {
                try { cap.Release(); } catch { }
                try { cap.Dispose(); } catch { }
            }
        }
    }
}
