using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace InspectionProgram.Camera
{
    /// <summary>
    /// 테스트용 웹캠 프레임 소스. (OpenCV VideoCapture)
    /// </summary>
    public sealed class OpenCvWebcamFrameSource : IFrameSource
    {
        private enum CaptureBackendKind
        {
            DirectShow,
            Default,
            MediaFoundation,
        }

        private readonly int _deviceIndex;
        private readonly int _targetFps;
        private readonly Action<string> _diagnostic;
        private VideoCapture _cap;
        private volatile bool _running;
        private volatile bool _firstFrameDiagDone;

        public bool IsRunning => _running;

        /// <param name="diagnostic">티칭 로그 등으로만 전달. 카메라 디버깅 한 줄.</param>
        public OpenCvWebcamFrameSource(int deviceIndex = 0, int targetFps = 30, Action<string> diagnostic = null)
        {
            _deviceIndex = Math.Max(0, deviceIndex);
            _targetFps = Math.Max(1, Math.Min(60, targetFps));
            _diagnostic = diagnostic;
        }

        private void Diag(string line)
        {
            try
            {
                _diagnostic?.Invoke(line);
            }
            catch
            {
            }
        }

        /// <summary>인덱스 스캔 등 — 열리기만 확인.</summary>
        private static IEnumerable<CaptureBackendKind> BackendOrder()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                yield return CaptureBackendKind.DirectShow;
                yield return CaptureBackendKind.MediaFoundation;
                yield return CaptureBackendKind.Default;
            }
            else
            {
                yield return CaptureBackendKind.Default;
            }
        }

        /// <summary>라이브: 기본(ANY) 백엔드는 Windows에서 첫 Read가 영구 블록되는 사례가 있어 MSMF를 앞에 두고 기본은 제외.</summary>
        private static IEnumerable<CaptureBackendKind> LiveBackendOrder()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                yield return CaptureBackendKind.DirectShow;
                yield return CaptureBackendKind.MediaFoundation;
            }
            else
            {
                yield return CaptureBackendKind.Default;
            }
        }

        private static VideoCapture CreateCapture(int deviceIndex, CaptureBackendKind kind)
        {
            int di = Math.Max(0, deviceIndex);
            switch (kind)
            {
                case CaptureBackendKind.DirectShow:
                    return new VideoCapture(di, VideoCaptureAPIs.DSHOW);
                case CaptureBackendKind.MediaFoundation:
                    return new VideoCapture(di, VideoCaptureAPIs.MSMF);
                default:
                    return new VideoCapture(di);
            }
        }

        private static string BackendLabel(CaptureBackendKind kind)
        {
            switch (kind)
            {
                case CaptureBackendKind.DirectShow:
                    return "DirectShow";
                case CaptureBackendKind.MediaFoundation:
                    return "Media Foundation";
                default:
                    return "기본 백엔드";
            }
        }

        private void LogBackendOpened(CaptureBackendKind kind, int di)
        {
            try
            {
                _diagnostic?.Invoke("OpenCV: " + BackendLabel(kind) + ", 장치 인덱스 " + di.ToString());
            }
            catch
            {
            }
        }

        /// <summary>카메라 번호 검사(WebcamIndexScanner)용 — 웜업 없이 첫 성공 백엔드만 연다.</summary>
        internal static VideoCapture TryOpenWebcam(int deviceIndex, int targetFps = 30)
        {
            int di = Math.Max(0, deviceIndex);
            foreach (CaptureBackendKind kind in BackendOrder())
            {
                VideoCapture c = CreateCapture(di, kind);
                if (c != null && c.IsOpened())
                {
                    ApplyCaptureDefaults(c, targetFps);
                    return c;
                }

                TryDispose(ref c);
            }

            return null;
        }

        private static void ApplyCaptureDefaults(VideoCapture cap, int targetFps, bool setStandardResolution = true)
        {
            int fps = Math.Max(1, Math.Min(60, targetFps));
            try { cap.Set(VideoCaptureProperties.BufferSize, 1); } catch { }
            try { cap.Set(VideoCaptureProperties.Fps, fps); } catch { }
            // 일부 드라이버는 640x480 강제 시 검은 프레임만 주고, 다른 백엔드·기본 해상도에서는 정상이다.
            if (setStandardResolution)
            {
                try { cap.Set(VideoCaptureProperties.FrameWidth, 640); } catch { }
                try { cap.Set(VideoCaptureProperties.FrameHeight, 480); } catch { }
            }
        }

        /// <summary>라이브 전용: USB 웹캠에서 YUY2만 검은 화면·저FPS일 때 MJPEG이 도움이 되는 경우가 많다.</summary>
        private static void ApplyLiveUsbPreferredSettings(VideoCapture cap, int targetFps)
        {
            int fps = Math.Max(1, Math.Min(60, targetFps));
            try { cap.Set(VideoCaptureProperties.BufferSize, 1); } catch { }
            try { cap.Set(VideoCaptureProperties.Fps, fps); } catch { }
            try
            {
                int mjpg = VideoWriter.FourCC('M', 'J', 'P', 'G');
                cap.Set(VideoCaptureProperties.FourCC, mjpg);
            }
            catch
            {
            }

            try { cap.Set(VideoCaptureProperties.FrameWidth, 640); } catch { }
            try { cap.Set(VideoCaptureProperties.FrameHeight, 480); } catch { }
        }

        /// <summary>완전 검정(잘못된 가상 장치·백엔드)과 구분 — 평균·최대 픽셀 모두 0에 가까우면 실패 처리.</summary>
        private static double GetMeanBrightness(Mat mat)
        {
            if (mat == null || mat.Empty())
                return 0;
            Scalar mean = Cv2.Mean(mat);
            if (mat.Channels() >= 3)
                return (mean.Val0 + mean.Val1 + mean.Val2) / 3.0;
            return mean.Val0;
        }

        private static bool FrameHasNonBlackSignal(Mat mat)
        {
            if (mat == null || mat.Empty())
                return false;
            try
            {
                if (GetMeanBrightness(mat) >= 1.0)
                    return true;

                using (Mat gray = new Mat())
                {
                    if (mat.Channels() == 3)
                        Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
                    else if (mat.Channels() == 1)
                        mat.CopyTo(gray);
                    else
                        return false;

                    Cv2.MinMaxLoc(gray, out _, out double maxVal);
                    return maxVal >= 1.0;
                }
            }
            catch
            {
                return false;
            }
        }

        private static void TryDispose(ref VideoCapture cap)
        {
            if (cap == null)
                return;
            try
            {
                cap.Release();
            }
            catch
            {
            }

            try
            {
                cap.Dispose();
            }
            catch
            {
            }

            cap = null;
        }

        /// <summary>Read 후 빈 프레임이면 Grab+Retrieve 시도(일부 드라이버/백엔드).</summary>
        internal static bool TryReadFrame(VideoCapture cap, Mat mat)
        {
            return ReadFrameReliable(cap, mat);
        }

        private static bool ReadFrameReliable(VideoCapture cap, Mat mat)
        {
            if (cap == null || !cap.IsOpened())
                return false;
            try
            {
                if (cap.Read(mat) && mat.Empty() == false)
                    return true;
            }
            catch
            {
            }

            try
            {
                if (cap.Grab() && cap.Retrieve(mat) && mat.Empty() == false)
                    return true;
            }
            catch
            {
            }

            return false;
        }

        private async Task<bool> TryWarmupOnceAsync(
            VideoCapture cap,
            Mat mat,
            int maxAttempts,
            int delayMs,
            CancellationToken cancellationToken)
        {
            int blackOnlyStreak = 0;
            const int maxBlackOnlyBeforeGiveUp = 22;

            for (int w = 0; w < maxAttempts; w++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (_running == false)
                    return false;
                if (w == 0 || ((w + 1) % 10) == 0)
                    Diag("OpenCV: 웜업 시도 " + (w + 1).ToString() + "/" + maxAttempts.ToString() + " …");

                if (ReadFrameReliable(cap, mat))
                {
                    if (FrameHasNonBlackSignal(mat))
                    {
                        Diag("OpenCV: 웜업 성공 (" + (w + 1).ToString() + "회 시도), 스트림 수신");
                        return true;
                    }

                    blackOnlyStreak++;
                    if (blackOnlyStreak >= maxBlackOnlyBeforeGiveUp)
                    {
                        Diag(
                            "OpenCV: 검정 프레임만 "
                            + blackOnlyStreak.ToString()
                            + "회 연속 — 이 백엔드는 건너뜀");
                        return false;
                    }

                    if (w == 0 || ((w + 1) % 15) == 0)
                    {
                        Diag(
                            "OpenCV: 프레임은 수신되나 완전 검정(평균≈"
                            + GetMeanBrightness(mat).ToString("F1")
                            + ") — 잘못된 장치/백엔드일 수 있음, 계속 시도…");
                    }
                }
                else
                    blackOnlyStreak = 0;

                try
                {
                    await Task.Delay(delayMs, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                }
            }

            return false;
        }

        public Task StartAsync(Action<Bitmap> onFrame, CancellationToken cancellationToken)
        {
            if (onFrame == null)
                throw new ArgumentNullException(nameof(onFrame));
            if (_running)
                return Task.CompletedTask;

            _running = true;
            _firstFrameDiagDone = false;

            return Task.Run(async () =>
            {
                try
                {
                    Diag("OpenCV: 스트림 스레드 시작");

                    int frameDelayMs = (int)Math.Round(1000.0 / _targetFps);
                    int failStreak = 0;
                    int di = _deviceIndex;

                    using (Mat mat = new Mat())
                    {
                        bool opened = false;
                        foreach (CaptureBackendKind kind in LiveBackendOrder())
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            if (_running == false)
                                break;

                            VideoCapture candidate = CreateCapture(di, kind);
                            if (candidate == null || candidate.IsOpened() == false)
                            {
                                TryDispose(ref candidate);
                                continue;
                            }

                            LogBackendOpened(kind, di);
                            ApplyLiveUsbPreferredSettings(candidate, _targetFps);
                            Diag("OpenCV: 웜업 시작 (" + BackendLabel(kind) + ", MJPEG·640x480 선호) …");

                            bool warmed = await TryWarmupOnceAsync(
                                    candidate,
                                    mat,
                                    maxAttempts: 60,
                                    delayMs: 25,
                                    cancellationToken)
                                .ConfigureAwait(false);

                            if (warmed)
                            {
                                _cap = candidate;
                                opened = true;
                                break;
                            }

                            TryDispose(ref candidate);
                            if (cancellationToken.IsCancellationRequested || _running == false)
                                break;

                            Diag("OpenCV: " + BackendLabel(kind) + "에서 비검정 프레임 없음 — 다음 방식 시도");

                            // MJPEG+해상도가 오히려 검은 화면이면, 같은 백엔드로 해상도 비강제만 재시도
                            candidate = CreateCapture(di, kind);
                            if (candidate == null || candidate.IsOpened() == false)
                            {
                                TryDispose(ref candidate);
                                continue;
                            }

                            LogBackendOpened(kind, di);
                            ApplyCaptureDefaults(candidate, _targetFps, setStandardResolution: false);
                            Diag("OpenCV: 재시도 (" + BackendLabel(kind) + ", 자동 해상도) …");

                            warmed = await TryWarmupOnceAsync(
                                    candidate,
                                    mat,
                                    maxAttempts: 40,
                                    delayMs: 25,
                                    cancellationToken)
                                .ConfigureAwait(false);

                            if (warmed)
                            {
                                _cap = candidate;
                                opened = true;
                                break;
                            }

                            TryDispose(ref candidate);
                            if (cancellationToken.IsCancellationRequested || _running == false)
                                break;

                            Diag("OpenCV: " + BackendLabel(kind) + "(자동 해상도)도 비검정 없음 — 다음 방식 시도");
                        }

                        if (opened == false || _cap == null)
                        {
                            if (cancellationToken.IsCancellationRequested || _running == false)
                                return;

                            Diag("OpenCV: 모든 백엔드에서 웜업 실패 — 인덱스·다른 앱 점유·드라이버를 확인하세요.");
                            throw new InvalidOperationException(
                                "웹캠을 열 수 없거나 프레임을 받지 못했습니다. 카메라 탭(인덱스)·권한·다른 앱 점유를 확인하세요.");
                        }

                        while (_running && cancellationToken.IsCancellationRequested == false)
                        {
                            if (_cap == null || _cap.IsOpened() == false)
                                break;

                            bool ok = ReadFrameReliable(_cap, mat);
                            if (ok && mat.Empty() == false)
                            {
                                failStreak = 0;

                                if (_firstFrameDiagDone == false)
                                {
                                    _firstFrameDiagDone = true;
                                    try
                                    {
                                        Scalar mean = Cv2.Mean(mat);
                                        double bright;
                                        if (mat.Channels() >= 3)
                                            bright = (mean.Val0 + mean.Val1 + mean.Val2) / 3.0;
                                        else
                                            bright = mean.Val0;

                                        Diag(
                                            "OpenCV: 첫 프레임 "
                                            + mat.Width.ToString() + "x" + mat.Height.ToString()
                                            + ", 채널=" + mat.Channels().ToString()
                                            + ", 평균밝기≈" + bright.ToString("F1")
                                            + " (낮으면 검은 화면·잘못된 장치 가능)");
                                    }
                                    catch
                                    {
                                    }
                                }

                                Bitmap bmp = null;
                                try
                                {
                                    bmp = BitmapConverter.ToBitmap(mat);
                                    onFrame(bmp);
                                    bmp = null;
                                }
                                catch (Exception convEx)
                                {
                                    Diag("OpenCV: BitmapConverter 실패 — " + convEx.Message);
                                    if (bmp != null)
                                    {
                                        try
                                        {
                                            bmp.Dispose();
                                        }
                                        catch
                                        {
                                        }

                                        bmp = null;
                                    }
                                }
                                finally
                                {
                                    if (bmp != null)
                                        bmp.Dispose();
                                }
                            }
                            else
                            {
                                failStreak++;
                                if (failStreak == 45)
                                    Diag("OpenCV: 연속으로 프레임을 못 읽었습니다(Read 실패). 드라이버·다른 앱 점유를 확인하세요.");
                            }

                            try
                            {
                                await Task.Delay(frameDelayMs, cancellationToken).ConfigureAwait(false);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                finally
                {
                    _running = false;
                }
            }, cancellationToken);
        }

        public void Stop()
        {
            _running = false;
        }

        public void Dispose()
        {
            try
            {
                Stop();
                if (_cap != null)
                {
                    _cap.Release();
                    _cap.Dispose();
                    _cap = null;
                }
            }
            catch
            {
            }
        }
    }
}
