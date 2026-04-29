using System;
using System.IO;

namespace InspectionProgram.Common
{
    /// <summary>
    /// Teaching에서 저장/선택한 NCC 모델을 AutoRun에서도 쓰기 위한 공유 상태.
    /// (앱 세션 내 메모리 상태)
    /// </summary>
    public static class NccSharedModelState
    {
        private static string _modelPath = string.Empty;
        private static double _templateWidth;
        private static double _templateHeight;
        private static double _minScore = 0.75;

        private static bool _hasReferencePose;
        private static double _refRow;
        private static double _refCol;
        private static double _refAngle;
        private static string _goldenImagePath = string.Empty;

        /// <summary>
        /// 골든 기준 자세로의 이미지 정렬(방법 A). 참조 자세가 없으면 효과 없음.
        /// 기본값은 false: 이미지 픽셀을 회전/이동하면 뷰어에서 ROI가 "이상해 보이는" 문제가 생길 수 있어,
        /// 기본 동작은 NCC로 ROI만 추적(이동)하고 이미지는 원본 그대로 유지합니다.
        /// </summary>
        public static bool AlignImagesEnabled { get; set; } = false;

        public static void Set(string modelPath, double templateWidth, double templateHeight, double minScore)
        {
            if (string.IsNullOrWhiteSpace(modelPath))
                return;
            if (File.Exists(modelPath) == false)
                return;

            string normNew = Path.GetFullPath(modelPath);
            string normOld = string.IsNullOrEmpty(_modelPath) ? string.Empty : Path.GetFullPath(_modelPath);
            if (!string.Equals(normNew, normOld, StringComparison.OrdinalIgnoreCase))
            {
                ClearReferencePose();
            }

            _modelPath = modelPath;
            _templateWidth = templateWidth;
            _templateHeight = templateHeight;
            if (minScore >= 0.0 && minScore <= 1.0)
                _minScore = minScore;
        }

        /// <summary>모델 저장 직후 골든 이미지에서 매칭한 참조 자세를 함께 저장합니다.</summary>
        public static void SetWithReferencePose(
            string modelPath,
            double templateWidth,
            double templateHeight,
            double minScore,
            double refRow,
            double refCol,
            double refAngle,
            string goldenImagePath)
        {
            Set(modelPath, templateWidth, templateHeight, minScore);
            _hasReferencePose = true;
            _refRow = refRow;
            _refCol = refCol;
            _refAngle = refAngle;
            _goldenImagePath = goldenImagePath != null ? goldenImagePath : string.Empty;
        }

        private static void ClearReferencePose()
        {
            _hasReferencePose = false;
            _refRow = 0.0;
            _refCol = 0.0;
            _refAngle = 0.0;
            _goldenImagePath = string.Empty;
        }

        public static bool TryGet(
            out string modelPath,
            out double templateWidth,
            out double templateHeight,
            out double minScore,
            out bool hasReferencePose,
            out double refRow,
            out double refCol,
            out double refAngle)
        {
            modelPath = _modelPath;
            templateWidth = _templateWidth;
            templateHeight = _templateHeight;
            minScore = _minScore;
            hasReferencePose = _hasReferencePose;
            refRow = _refRow;
            refCol = _refCol;
            refAngle = _refAngle;
            return !string.IsNullOrWhiteSpace(modelPath) && File.Exists(modelPath);
        }
    }
}
