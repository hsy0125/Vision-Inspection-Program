using System;
using System.Globalization;
using System.IO;

namespace InspectionProgram.Halcon
{
    /// <summary>골든 이미지와 같은 폴더에 원본파일명_001.ncm 형식으로 NCC 모델 경로를 냅니다.</summary>
    public static class NccModelFileNaming
    {
        public const string NccFileExtension = "ncm";

        public static string GetNextModelFilePath(string sourceImagePath)
        {
            if (string.IsNullOrWhiteSpace(sourceImagePath))
                throw new ArgumentException("Image path is required.", nameof(sourceImagePath));

            string full = Path.GetFullPath(sourceImagePath);
            string dir = Path.GetDirectoryName(full);
            if (string.IsNullOrEmpty(dir))
                throw new InvalidOperationException("Could not determine directory for: " + sourceImagePath);

            string stem = Path.GetFileNameWithoutExtension(full);
            if (string.IsNullOrEmpty(stem))
                throw new InvalidOperationException("Could not get file name without extension: " + sourceImagePath);

            for (int n = 1; n <= 9999; n++)
            {
                string name = string.Format(CultureInfo.InvariantCulture, "{0}_{1:000}.{2}", stem, n, NccFileExtension);
                string candidate = Path.Combine(dir, name);
                if (File.Exists(candidate) == false)
                    return candidate;
            }

            throw new InvalidOperationException("Could not allocate a new model file name (too many ." + NccFileExtension + " files).");
        }
    }
}
