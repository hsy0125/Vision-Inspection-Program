using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InspectionProgram.Common
{
    /// <summary>
    /// 프로젝트 목표: 카메라·검사 루프가 도는 동안 사용자가 끄기 전까지 프로세스가 죽지 않게 하는 것.
    /// WinForms UI 스레드에서 잡히지 않은 예외는 기본적으로 대화 상자 후 종료로 이어질 수 있으므로,
    /// 여기서 전역으로 로그를 남기고(필요 시 나중에 상태줄·토스트로 확장) 계속 실행되도록 훅을 걸어 둡니다.
    /// 신입/학부 수준에서도 추적하기 쉽게, 예외 처리는 이 클래스와 각 화면의 try/catch 두 축으로 생각하면 됩니다.
    /// </summary>
    public static class AppExceptionHandler
    {
        private static int _registered;

        /// <summary>
        /// UI 이벤트 등에서 "실패해도 앱은 살아 있어야" 하는 코드를 감쌉니다. 로그만 남기고 예외는 삼킵니다.
        /// </summary>
        public static void ExecuteBestEffort(string context, Action action)
        {
            if (action == null)
                return;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                try
                {
                    AppLogger.Write("EX", context + " : " + ex.Message);
                }
                catch
                {
                    // 로거까지 실패하면 더 이상 할 수 있는 것이 없음
                }
            }
        }

        /// <summary>
        /// <see cref="Application.Run"/> 전에 한 번 호출하세요. UI 스레드 예외를 로그로 남깁니다.
        /// </summary>
        public static void RegisterWinFormsGlobalHandlers()
        {
            if (Interlocked.Exchange(ref _registered, 1) == 1)
                return;

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Application.ApplicationExit += Application_ApplicationExit;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            try
            {
                AppLogger.Write("EXIT", "ApplicationExit");
            }
            catch
            {
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            try
            {
                AppLogger.Write("EXIT", "ProcessExit");
            }
            catch
            {
            }
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                AppLogger.Write("TASK-EX", e.Exception != null ? e.Exception.ToString() : "unknown");
            }
            catch
            {
            }

            try
            {
                // Keep process alive; still logged above.
                e.SetObserved();
            }
            catch
            {
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                AppLogger.Write("UI-EX", e.Exception.ToString());
            }
            catch
            {
            }

            // 여기서 MessageBox를 띄우면 현장 설비용으로는 부담이 될 수 있음 → 로그 우선.
            // 디버깅 중에만 띄우고 싶다면 #if DEBUG 로 감싸면 됩니다.
#if DEBUG
            try
            {
                MessageBox.Show(
                    "UI 예외가 발생했습니다. 로그를 확인하세요.\r\n" + e.Exception.Message,
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch
            {
            }
#endif
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                string msg = ex != null ? ex.ToString() : e.ExceptionObject?.ToString() ?? "unknown";
                AppLogger.Write("FATAL", msg + " | IsTerminating=" + e.IsTerminating.ToString());
            }
            catch
            {
            }
        }
    }
}
