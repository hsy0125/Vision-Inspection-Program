using System;
using System.Collections.Generic;
using System.Drawing;

namespace InspectionProgram.Common
{
    /// <summary>
    /// 티칭에서 저장해 자동 실행으로 넘기는 Blob 검사 레시피(ROI 좌표·임계값 등).
    /// 좌표는 이미지 픽셀 기준입니다.
    /// </summary>
    public sealed class InspectionRecipe
    {
        public DateTime SavedUtc { get; set; }

        /// <summary>사각형 ROI 목록(이미지 픽셀).</summary>
        public List<Rectangle> RoiRectangles { get; set; } = new List<Rectangle>();

        public int Threshold { get; set; } = 128;

        /// <summary>Blob 최소 면적(티칭 MinArea 슬라이더).</summary>
        public int MinArea { get; set; }

        /// <summary>0이면 개수 일치 판정 비활성(Blob ≥1 이면 OK 등 기존 규칙).</summary>
        public int ExpectedBlobCount { get; set; }

        /// <summary>
        /// ROI 내부 전경 픽셀 수 하한(0이면 비활성).
        /// 전경 기준은 Blob 임계값(Threshold~255)과 동일합니다.
        /// </summary>
        public int ForegroundPixelMin { get; set; }

        /// <summary>
        /// ROI 내부 전경 픽셀 수 상한(0이면 비활성).
        /// </summary>
        public int ForegroundPixelMax { get; set; }

        /// <summary>NCC 전역 카운트 시 스코어 필터 하한/상한(0~1). 둘 다 유효할 때만 레시피 구간을 사용합니다.</summary>
        public double NccFilterMinScore { get; set; } = double.NaN;

        public double NccFilterMaxScore { get; set; } = double.NaN;

        /// <summary>티칭에서 저장한 NCC 모델 파일 경로(자동 실행에서 세션 없이도 전역 카운트 가능).</summary>
        public string NccModelPath { get; set; }

        /// <summary>모델 템플릿 가로·세로(px). 전역 NCC 매칭 박스 표시에 사용합니다.</summary>
        public double NccTemplateWidth { get; set; }

        public double NccTemplateHeight { get; set; }

        /// <summary>판정 최소 NCC 점수(0~1). NaN이면 기본 0.75.</summary>
        public double NccMinScore { get; set; } = double.NaN;

        public bool IsEmpty => RoiRectangles == null || RoiRectangles.Count == 0;
    }
}
