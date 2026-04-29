using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace InspectionProgram.Camera
{
    /// <summary>
    /// 비디오 프레임 공급자(웹캠/실카메라 공통). Teaching에서 Live 미리보기 용도로 사용합니다.
    /// </summary>
    public interface IFrameSource : IDisposable
    {
        bool IsRunning { get; }

        /// <summary>
        /// 프레임 스트림을 시작합니다. 프레임은 백그라운드에서 생성되며, 콜백은 호출 스레드가 보장되지 않습니다.
        /// UI 업데이트는 호출자가 UI 스레드로 marshal 해야 합니다.
        /// </summary>
        Task StartAsync(Action<Bitmap> onFrame, CancellationToken cancellationToken);

        void Stop();
    }
}

