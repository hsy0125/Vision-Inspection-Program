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
        /// [NCC 모델 저장] 직후(또는 레시피 적용 시) 캔버스에 있던 사각 ROI 개수.
        /// 전역 NCC 카운트 검사에서 화면 ROI가 비어 있어도 로그/그리드의 ROI 개수에 사용합니다.
        /// 사용자가 ROI를 추가·삭제·이동·크기 변경하면(프로그램적 NCC 추적 제외) 0으로 돌아갑니다.
        /// </summary>
        private static int _patternTeachRoiCountFallback;

        private static int _programmaticRoiUpdateDepth;

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
                ClearPatternTeachRoiCountFallback();
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

        /// <summary>NCC 패턴 저장 시점의 ROI 개수(1장당 검사 ROI 수)를 기록합니다.</summary>
        public static void SetPatternTeachRoiCountFallback(int roiRectangleCount)
        {
            _patternTeachRoiCountFallback = roiRectangleCount > 0 ? roiRectangleCount : 0;
        }

        public static void ClearPatternTeachRoiCountFallback()
        {
            _patternTeachRoiCountFallback = 0;
        }

        /// <summary>NCC로 ROI만 자동 이동·추가할 때는 호출부에서 Begin/End로 감싸 ROI 변경으로 스냅샷이 지워지지 않게 합니다.</summary>
        public static void BeginProgrammaticRoiUpdate()
        {
            _programmaticRoiUpdateDepth++;
        }

        public static void EndProgrammaticRoiUpdate()
        {
            if (_programmaticRoiUpdateDepth > 0)
                _programmaticRoiUpdateDepth--;
        }

        /// <summary>캔버스 ROI 컬렉션이 바뀌었을 때 호출: NCC 추적이 아닌 사용자 편집이면 스냅샷을 무효화합니다.</summary>
        public static void NotifyUserRoiCollectionChangedMayInvalidatePatternSnapshot()
        {
            if (_programmaticRoiUpdateDepth > 0)
                return;
            ClearPatternTeachRoiCountFallback();
        }

        /// <summary>화면에 ROI가 없을 때, 마지막 패턴 저장 시 기록된 ROI 개수를 보조로 씁니다.</summary>
        public static int GetInspectRoiCountWithPatternFallback(int canvasRoiItemCount)
        {
            if (canvasRoiItemCount > 0)
                return canvasRoiItemCount;
            return _patternTeachRoiCountFallback > 0 ? _patternTeachRoiCountFallback : 0;
        }

        /// <summary>
        /// 뷰어 «지우기(Clear)» 시: 공유 NCC 모델·참조 자세·ROI 스냅샷 등 세션 상태를 초기화합니다.
        /// (정렬 옵션 <see cref="AlignImagesEnabled"/>는 사용자 설정으로 유지합니다.)
        /// </summary>
        public static void ResetSession()
        {
            _modelPath = string.Empty;
            _templateWidth = 0.0;
            _templateHeight = 0.0;
            _minScore = 0.75;
            ClearReferencePose();
            ClearPatternTeachRoiCountFallback();
            _programmaticRoiUpdateDepth = 0;
        }
    }
}
