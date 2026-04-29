using System;
using System.IO;

namespace InspectionProgram.Common
{
    // 로그 저장 위치 : C:\Users\user\Desktop\VisionInspectEx\InspectionProgram\bin\x64\Debug\Log
    /// <summary>
    /// 검사 결과 CSV 등을 둘 폴더 경로. 실행 파일 기준 하위 <c>Log</c> (개발 시에는 보통 <c>bin\...\Debug\Log</c>).
    /// 앱 시작 시 <see cref="EnsureDirectoryExists"/>로 폴더를 보장합니다.
    /// </summary>
    public static class InspectionResultLogPaths
    {
        public static string GetLogDirectory()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
        }

        public static void EnsureDirectoryExists()
        {
            string dir = GetLogDirectory();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        /// <summary>
        /// CSV 저장용 파일 전체 경로를 만듭니다.
        /// 시각은 <c>yy-MM-dd-HHmmss</c> 형태로 붙입니다. Windows 파일명에 콜론(<c>:</c>)을 쓸 수 없어
        /// 사용자가 말한 <c>hh:mm:ss</c> 느낌은 <c>HHmmss</c> 연속 숫자로 대응합니다.
        /// </summary>
        public static string BuildCsvFilePath(DateTime localTime)
        {
            EnsureDirectoryExists();
            string fileName = localTime.ToString("yy-MM-dd-HHmmss") + ".csv";
            return Path.Combine(GetLogDirectory(), fileName);
        }

        /// <summary>Inspection Log 텍스트 저장용 기본 파일명(확장자 .txt 포함).</summary>
        public static string BuildInspectionLogTxtFileName(DateTime localTime)
        {
            return "InspectionLog_" + localTime.ToString("yyyy-MM-dd_HHmmss") + ".txt";
        }
    }
}
