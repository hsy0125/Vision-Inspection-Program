using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace InspectionProgram.Halcon
{
    /// <summary>NCC 패턴 매칭 실행 결과 및 기준값 대비 OK/NG 판정.</summary>
    public class HalconResult
    {
        public int Count { get; set; }
        public HTuple Row { get; set; }
        public HTuple Column { get; set; }
        public HTuple Angle { get; set; }
        public HTuple Score { get; set; }

        /// <summary>NCC 템플릿 ROI 가로(픽셀). 매칭 중심 기준 박스 크기에 사용합니다.</summary>
        public double TemplateWidth { get; set; }

        /// <summary>NCC 템플릿 ROI 세로(픽셀).</summary>
        public double TemplateHeight { get; set; }

        /// <summary><see cref="EvaluateJudgment"/> 호출 여부.</summary>
        public bool JudgmentEvaluated { get; private set; }

        /// <summary>기준값 비교 후 OK 여부. 미평가 시 false.</summary>
        public bool IsOk { get; private set; }

        /// <summary>한 줄 요약: OK 또는 NG 사유.</summary>
        public string JudgmentSummary { get; private set; } = string.Empty;

        /// <summary>
        /// 티칭에서 정한 기준으로 판정합니다.
        /// </summary>
        /// <param name="expectedMatchCount">예상 매칭 개수(예: 1). 0이면 검출 0건이 OK.</param>
        /// <param name="minScorePerMatch">각 매칭 NCC 스코어 하한. 검출이 없으면 검사하지 않습니다.</param>
        public void EvaluateJudgment(int expectedMatchCount, double minScorePerMatch)
        {
            JudgmentEvaluated = true;
            IsOk = false;
            JudgmentSummary = string.Empty;

            if (Count != expectedMatchCount)
            {
                if (Count < expectedMatchCount)
                {
                    JudgmentSummary = string.Format(
                        CultureInfo.InvariantCulture,
                        "NG  Missing: {0} (detected {1}, expected {2})",
                        expectedMatchCount - Count,
                        Count,
                        expectedMatchCount);
                }
                else
                {
                    JudgmentSummary = string.Format(
                        CultureInfo.InvariantCulture,
                        "NG  Extra: {0} (detected {1}, expected {2})",
                        Count - expectedMatchCount,
                        Count,
                        expectedMatchCount);
                }
                return;
            }

            if (Count == 0)
            {
                IsOk = true;
                JudgmentSummary = "OK";
                return;
            }

            if (Score == null || Score.TupleLength() != Count)
            {
                JudgmentSummary = "NG  Score tuple mismatch";
                return;
            }

            for (int i = 0; i < Count; i++)
            {
                double sc = Score.TupleSelect(i).D;
                if (sc < minScorePerMatch)
                {
                    JudgmentSummary = string.Format(
                        CultureInfo.InvariantCulture,
                        "NG  Score[{0}]={1:F3} < min {2:F3}",
                        i,
                        sc,
                        minScorePerMatch);
                    return;
                }
            }

            IsOk = true;
            JudgmentSummary = string.Format(
                CultureInfo.InvariantCulture,
                "OK  count={0}, minScore>={1:F3}",
                Count,
                minScorePerMatch);
        }

        /// <summary>
        /// 각 매칭의 <see cref="Score"/>가 [minScore, maxScore] 안에 드는 것만 남깁니다 (전체 이미지 NCC 카운트용).
        /// </summary>
        public HalconResult FilterByScoreRange(double minScore, double maxScore)
        {
            if (minScore > maxScore)
            {
                double t = minScore;
                minScore = maxScore;
                maxScore = t;
            }

            var o = new HalconResult
            {
                TemplateWidth = TemplateWidth,
                TemplateHeight = TemplateHeight,
            };

            if (Score == null || Count <= 0)
            {
                o.Count = 0;
                o.Row = new HTuple();
                o.Column = new HTuple();
                o.Angle = new HTuple();
                o.Score = new HTuple();
                return o;
            }

            var rows = new List<double>();
            var cols = new List<double>();
            var angs = new List<double>();
            var scs = new List<double>();

            for (int i = 0; i < Count; i++)
            {
                double s = Score[i].D;
                if (s >= minScore && s <= maxScore)
                {
                    rows.Add(Row[i].D);
                    cols.Add(Column[i].D);
                    angs.Add(Angle[i].D);
                    scs.Add(s);
                }
            }

            o.Count = rows.Count;
            if (o.Count == 0)
            {
                o.Row = new HTuple();
                o.Column = new HTuple();
                o.Angle = new HTuple();
                o.Score = new HTuple();
                return o;
            }

            o.Row = new HTuple(rows.ToArray());
            o.Column = new HTuple(cols.ToArray());
            o.Angle = new HTuple(angs.ToArray());
            o.Score = new HTuple(scs.ToArray());
            return o;
        }

        /// <summary>
        /// 지정한 인덱스만 남겨 새 결과를 만듭니다.
        /// </summary>
        public HalconResult FilterByIndices(List<int> indices)
        {
            var o = new HalconResult
            {
                TemplateWidth = TemplateWidth,
                TemplateHeight = TemplateHeight,
            };

            if (indices == null || indices.Count == 0 || Row == null || Column == null || Angle == null || Score == null)
            {
                o.Count = 0;
                o.Row = new HTuple();
                o.Column = new HTuple();
                o.Angle = new HTuple();
                o.Score = new HTuple();
                return o;
            }

            var rows = new List<double>();
            var cols = new List<double>();
            var angs = new List<double>();
            var scs = new List<double>();
            for (int k = 0; k < indices.Count; k++)
            {
                int i = indices[k];
                if (i < 0 || i >= Count)
                    continue;
                rows.Add(Row[i].D);
                cols.Add(Column[i].D);
                angs.Add(Angle[i].D);
                scs.Add(Score[i].D);
            }

            o.Count = rows.Count;
            if (o.Count <= 0)
            {
                o.Row = new HTuple();
                o.Column = new HTuple();
                o.Angle = new HTuple();
                o.Score = new HTuple();
                return o;
            }

            o.Row = new HTuple(rows.ToArray());
            o.Column = new HTuple(cols.ToArray());
            o.Angle = new HTuple(angs.ToArray());
            o.Score = new HTuple(scs.ToArray());
            return o;
        }
    }
}
