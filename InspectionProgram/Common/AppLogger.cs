using System;
using System.IO;
using System.Text;

namespace InspectionProgram.Common
{
    public static class AppLogger
    {
        private static readonly object SyncRoot = new object();
        private static bool _enabled = true;
        private static string _folderPath = string.Empty;

        public static bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public static void Initialize(string basePath)
        {
            try
            {
                _folderPath = Path.Combine(basePath, "Log");
                if (!Directory.Exists(_folderPath))
                    Directory.CreateDirectory(_folderPath);
            }
            catch
            {
            }
        }

        public static void Write(string category, string message)
        {
            try
            {
                if (!_enabled)
                    return;

                if (string.IsNullOrWhiteSpace(_folderPath))
                    Initialize(AppDomain.CurrentDomain.BaseDirectory);

                string filePath = Path.Combine(_folderPath, DateTime.Now.ToString("yyyyMMdd") + "_InspectionProgram.log");
                string line = string.Format("{0} [{1}] {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), category ?? "APP", message ?? string.Empty);

                lock (SyncRoot)
                {
                    File.AppendAllText(filePath, line + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
            }
        }
    }
}
