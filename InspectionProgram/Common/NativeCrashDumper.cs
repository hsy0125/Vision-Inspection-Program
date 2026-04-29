using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace InspectionProgram.Common
{
    /// <summary>
    /// Native(AccessViolation 등) 크래시를 대비해 미니덤프를 남깁니다.
    /// .NET 예외 핸들러(AppExceptionHandler)로 잡히지 않는 종료 원인 추적용입니다.
    /// </summary>
    internal static class NativeCrashDumper
    {
        private static bool _installed;

        public static void InstallBestEffort(string dumpDirectory)
        {
            if (_installed)
                return;
            _installed = true;

            try
            {
                if (string.IsNullOrWhiteSpace(dumpDirectory))
                    dumpDirectory = AppDomain.CurrentDomain.BaseDirectory;
                Directory.CreateDirectory(dumpDirectory);
                _dumpDirectory = dumpDirectory;

                _prevFilter = SetUnhandledExceptionFilter(UnhandledExceptionFilter);
            }
            catch
            {
            }
        }

        private static string _dumpDirectory;
        private static UnhandledExceptionFilterDelegate _prevFilter;

        private static uint UnhandledExceptionFilter(IntPtr exceptionPointers)
        {
            try
            {
                string dumpPath = BuildDumpPath(_dumpDirectory);
                WriteMiniDump(dumpPath, exceptionPointers);
                AppLogger.Write("DUMP", "MiniDump saved: " + dumpPath);
            }
            catch (Exception ex)
            {
                try { AppLogger.Write("DUMP", "MiniDump failed: " + ex.Message); } catch { }
            }

            // pass to previous filter (if any)
            try
            {
                if (_prevFilter != null)
                    return _prevFilter(exceptionPointers);
            }
            catch
            {
            }

            // EXCEPTION_EXECUTE_HANDLER
            return 1;
        }

        private static string BuildDumpPath(string dir)
        {
            string ts = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            int pid = 0;
            try { pid = Process.GetCurrentProcess().Id; } catch { }
            string name = "InspectionProgram_" + ts + "_pid" + pid + ".dmp";
            return Path.Combine(dir, name);
        }

        private static void WriteMiniDump(string dumpPath, IntPtr exceptionPointers)
        {
            using (var fs = new FileStream(dumpPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                IntPtr hFile = fs.SafeFileHandle.DangerousGetHandle();
                using (var p = Process.GetCurrentProcess())
                {
                    var mei = new MINIDUMP_EXCEPTION_INFORMATION
                    {
                        ThreadId = GetCurrentThreadId(),
                        ExceptionPointers = exceptionPointers,
                        ClientPointers = false
                    };

                    // WithDataSegs|WithHandleData|WithThreadInfo|WithUnloadedModules 는 분석에 꽤 도움 됨
                    MINIDUMP_TYPE type =
                        MINIDUMP_TYPE.MiniDumpWithDataSegs |
                        MINIDUMP_TYPE.MiniDumpWithHandleData |
                        MINIDUMP_TYPE.MiniDumpWithThreadInfo |
                        MINIDUMP_TYPE.MiniDumpWithUnloadedModules;

                    MiniDumpWriteDump(
                        p.Handle,
                        (uint)p.Id,
                        hFile,
                        type,
                        ref mei,
                        IntPtr.Zero,
                        IntPtr.Zero);
                }
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate uint UnhandledExceptionFilterDelegate(IntPtr exceptionPointers);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern UnhandledExceptionFilterDelegate SetUnhandledExceptionFilter(UnhandledExceptionFilterDelegate lpTopLevelExceptionFilter);

        [DllImport("dbghelp.dll", SetLastError = true)]
        private static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            uint processId,
            IntPtr hFile,
            MINIDUMP_TYPE dumpType,
            ref MINIDUMP_EXCEPTION_INFORMATION exceptionParam,
            IntPtr userStreamParam,
            IntPtr callbackParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct MINIDUMP_EXCEPTION_INFORMATION
        {
            public uint ThreadId;
            public IntPtr ExceptionPointers;
            [MarshalAs(UnmanagedType.Bool)]
            public bool ClientPointers;
        }

        [Flags]
        private enum MINIDUMP_TYPE : uint
        {
            MiniDumpNormal = 0x00000000,
            MiniDumpWithDataSegs = 0x00000001,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpWithThreadInfo = 0x00001000,
            MiniDumpWithUnloadedModules = 0x00000020,
        }
    }
}

