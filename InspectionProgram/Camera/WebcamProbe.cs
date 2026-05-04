using OpenCvSharp;
using System;
using System.Text;
using System.Threading;

namespace InspectionProgram.Camera
{
    internal static class WebcamProbe
    {
        public static string ProbeDevice(int deviceIndex, int warmupReads = 8)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Probe device " + deviceIndex);

            AppendApiResult(sb, deviceIndex, VideoCaptureAPIs.MSMF, warmupReads);
            AppendApiResult(sb, deviceIndex, VideoCaptureAPIs.DSHOW, warmupReads);
            AppendApiResult(sb, deviceIndex, VideoCaptureAPIs.ANY, warmupReads);

            return sb.ToString();
        }

        private static void AppendApiResult(StringBuilder sb, int deviceIndex, VideoCaptureAPIs api, int warmupReads)
        {
            sb.AppendLine("- API: " + api);
            VideoCapture cap = null;
            try
            {
                cap = new VideoCapture(deviceIndex, api);
                bool opened = cap != null && cap.IsOpened();
                sb.AppendLine("  opened=" + opened);
                if (!opened)
                    return;

                // basic config hints
                TrySet(cap, VideoCaptureProperties.FrameWidth, 640);
                TrySet(cap, VideoCaptureProperties.FrameHeight, 480);
                TrySet(cap, VideoCaptureProperties.Fps, 30);
                TrySet(cap, VideoCaptureProperties.FourCC, Fourcc('M', 'J', 'P', 'G'));

                sb.AppendLine("  width=" + TryGet(cap, VideoCaptureProperties.FrameWidth));
                sb.AppendLine("  height=" + TryGet(cap, VideoCaptureProperties.FrameHeight));
                sb.AppendLine("  fps=" + TryGet(cap, VideoCaptureProperties.Fps));
                sb.AppendLine("  fourcc=" + TryGet(cap, VideoCaptureProperties.FourCC));

                int ok = 0;
                int okGrab = 0;
                int total = Math.Max(1, warmupReads);
                using (var mat = new Mat())
                {
                    for (int i = 0; i < total; i++)
                    {
                        bool r = false;
                        try { r = cap.Read(mat); } catch { r = false; }
                        if (r && mat.Empty() == false && mat.Width > 1 && mat.Height > 1)
                            ok++;

                        bool g = false;
                        bool rr = false;
                        try
                        {
                            g = cap.Grab();
                            if (g)
                                rr = cap.Retrieve(mat);
                        }
                        catch
                        {
                            g = false;
                            rr = false;
                        }
                        if (g && rr && mat.Empty() == false && mat.Width > 1 && mat.Height > 1)
                            okGrab++;
                        Thread.Sleep(30);
                    }
                }

                sb.AppendLine("  read_ok=" + ok + "/" + total);
                sb.AppendLine("  grab_retrieve_ok=" + okGrab + "/" + total);
            }
            catch (Exception ex)
            {
                sb.AppendLine("  ex=" + ex.GetType().Name + ": " + ex.Message);
            }
            finally
            {
                try { cap?.Release(); } catch { }
                try { cap?.Dispose(); } catch { }
            }
        }

        private static void TrySet(VideoCapture cap, VideoCaptureProperties prop, double value)
        {
            try { cap?.Set(prop, value); } catch { }
        }

        private static string TryGet(VideoCapture cap, VideoCaptureProperties prop)
        {
            try
            {
                if (cap == null)
                    return "n/a";
                return cap.Get(prop).ToString("0.###");
            }
            catch
            {
                return "n/a";
            }
        }

        private static int Fourcc(char c1, char c2, char c3, char c4)
        {
            return (byte)c1 | ((byte)c2 << 8) | ((byte)c3 << 16) | ((byte)c4 << 24);
        }
    }
}

