using System;
using System.Diagnostics;
using System.Windows.Forms;
using InspectionProgram.Common;
using InspectionProgram.GUI;

namespace InspectionProgram
{
    /// <summary>
    /// 엔트리 포인트. 장시간 검사·카메라 연동 시에는 UI 스레드를 막지 않고,
    /// 예외는 <see cref="AppExceptionHandler"/> 로 흘려보내 로그 후에도 프로세스가 유지되도록 확장하는 것을 권장합니다.
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                AppLogger.Initialize(basePath);

                try
                {
                    using (var p = Process.GetCurrentProcess())
                    {
                        AppLogger.Write(
                            "BOOT",
                            "PID=" + p.Id +
                            " | 64bit=" + Environment.Is64BitProcess.ToString() +
                            " | CLR=" + Environment.Version +
                            " | Base=" + basePath +
                            " | WS(MB)=" + (p.WorkingSet64 / (1024 * 1024)).ToString());
                    }
                }
                catch
                {
                }

                // Native crash(AccessViolation 등) 추적용 미니덤프
                try
                {
                    NativeCrashDumper.InstallBestEffort(basePath);
                }
                catch
                {
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                AppExceptionHandler.RegisterWinFormsGlobalHandlers();
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                try
                {
                    AppLogger.Write("FATAL", ex.ToString());
                }
                catch
                {
                }

                string details = ex.ToString();
                if (ex.InnerException != null)
                    details += "\r\n\r\n--- InnerException ---\r\n" + ex.InnerException;

                MessageBox.Show(
                    details,
                    LocalizationService.GetText("StartupErrorTitle", LanguageType.Kr),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
