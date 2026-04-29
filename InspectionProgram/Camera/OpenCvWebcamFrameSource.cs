using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace InspectionProgram.Camera
{
    /// <summary>
    /// 테스트용 웹캠 프레임 소스. (OpenCV VideoCapture)
    /// </summary>
    public sealed class OpenCvWebcamFrameSource : IFrameSource
    {
        private readonly int _deviceIndex;
        private readonly int _targetFps;
        private VideoCapture _cap;
        private volatile bool _running;

        public bool IsRunning => _running;

        public OpenCvWebcamFrameSource(int deviceIndex = 0, int targetFps = 30)
        {
            _deviceIndex = Math.Max(0, deviceIndex);
            _targetFps = Math.Max(1, Math.Min(60, targetFps));
        }

        public Task StartAsync(Action<Bitmap> onFrame, CancellationToken cancellationToken)
        {
            if (onFrame == null)
                throw new ArgumentNullException(nameof(onFrame));
            if (_running)
                return Task.CompletedTask;

            _cap = new VideoCapture(_deviceIndex);
            if (_cap.IsOpened() == false)
                throw new InvalidOperationException("웹캠을 열 수 없습니다. 장치 인덱스/권한을 확인하세요.");

            _running = true;

            return Task.Run(async () =>
            {
                int frameDelayMs = (int)Math.Round(1000.0 / _targetFps);

                using (Mat mat = new Mat())
                {
                    while (_running && cancellationToken.IsCancellationRequested == false)
                    {
                        if (_cap == null || _cap.IsOpened() == false)
                            break;

                        bool ok = _cap.Read(mat);
                        if (ok && mat.Empty() == false)
                        {
                            // BitmapConverter는 매 프레임 Bitmap 생성 → caller가 Dispose해야 합니다.
                            Bitmap bmp = null;
                            try
                            {
                                bmp = BitmapConverter.ToBitmap(mat);
                                onFrame(bmp);
                                bmp = null;
                            }
                            finally
                            {
                                if (bmp != null)
                                    bmp.Dispose();
                            }
                        }

                        try
                        {
                            await Task.Delay(frameDelayMs, cancellationToken).ConfigureAwait(false);
                        }
                        catch
                        {
                            // ignore cancel/stop
                        }
                    }
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

