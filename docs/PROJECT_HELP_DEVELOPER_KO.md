# InspectionProgram 개발자용 도움말

이 문서는 `InspectionProgram` 유지보수/기능개발 담당자를 위한 기술 문서입니다.

## 1) 주요 구성

- 메인 셸: `InspectionProgram/MainForm.cs`
  - 모드 전환(`AutoRun`, `Teaching`)
  - 상단 툴바 액션 분기
  - 언어 변경 전파
- Teaching 화면: `InspectionProgram/UcTeachingShell.cs`
  - ROI/NCC 모델 저장 및 레시피 저장
- Auto Run 화면: `InspectionProgram/UcAutoRunShell.cs`
  - 레시피 적용 검사 실행, CSV/로그 저장
- 공용 검사 흐름: `InspectionProgram/GUI/BlobInspectRunFlow.cs`
  - 단건 검사, 자동 사이클, 결과 기록
- 공용 스트립 UI: `InspectionProgram/GUI/UcInspectFlowStrip.cs`

## 2) 검사 로직 개요

### Teaching
- 골든 이미지 로드
- ROI 지정
- 필요 시 NCC 모델 생성/저장
- `InspectionRecipe` 생성 후 저장

### Auto Run
- 이미지 또는 폴더 로드
- Run 시 레시피 준비 훅 호출
- 검사 경로
  - NCC 모델 사용 가능: 전역 NCC 카운트
  - 그 외: ROI 기반 Blob 검사
- 결과를 세션 리스트에 누적
- CSV 저장 기능 제공

## 3) Run/Count 동작 정합성

- Auto Run의 `3) Run inspect`
- MainForm 상단 `개수` 메뉴

두 경로는 동일 검사 흐름으로 연결되도록 구현되어 있습니다.

## 4) 다국어(Localization) 규칙

- 언어 타입: `InspectionProgram/Common/LanguageType.cs`
- 문자열 소스: `InspectionProgram/Common/LocalizationService.cs`
- 사용 규칙
  - 하드코딩 문자열 대신 `LocalizationService.GetText(key, language)` 사용
  - 신규 UI 문자열 추가 시 키를 먼저 등록
  - 화면별 `ApplyLanguage(...)`에서 컨트롤 텍스트 매핑
- 현재 적용 범위
  - 주요 버튼/레이블/그룹 타이틀/탭 관련 문구 반영
  - 일부 로그/예외 문구는 고정 문자열이 남을 수 있음

## 5) 문구 변경/추가 작업 절차

1. `LocalizationService`에 키 추가
2. 관련 화면(`MainForm`, `UcAutoRunShell`, `UcTeachingShell`, `UcInspectFlowStrip`)의 `ApplyLanguage`에 연결
3. 디자이너 기본 텍스트 의존이 있으면 런타임에서 덮어쓰기
4. 빌드 확인

## 6) 기능 변경 시 영향 포인트

- 검사 플로우 변경
  - `BlobInspectRunFlow` 우선 확인
  - `UcAutoRunShell`의 Run 호출/훅 연결 점검
- Teaching 레시피 변경
  - 레시피 저장/적용 로직 동시 확인
- NCC 처리 변경
  - Teaching/AutoRun 양쪽에서 모델 경로와 파라미터 일관성 확인

## 7) 운영 이슈 디버깅 가이드

- “이미지 있는데 Run 안 됨”
  - 이미지 로드 상태, strip 버튼 활성 조건, 배치 실행 상태 확인
- “자동 사이클 중 중복/누락”
  - 폴더 목록 구성, 인덱스 이동, 취소 토큰 처리 확인
- “NCC 결과 불안정”
  - 모델 유효성, score range, 레퍼런스 정렬 옵션 확인
- “다국어 일부 미적용”
  - 해당 컨트롤이 `ApplyLanguage`에서 매핑됐는지 확인

## 8) 문서 유지보수 규칙

- 작업자 절차 변경 시: `PROJECT_HELP_OPERATOR_KO.md` 동시 갱신
- 코드 구조/로직 변경 시: 본 문서(`PROJECT_HELP_DEVELOPER_KO.md`) 갱신
- 릴리즈 전 최소 1회 문서-동작 정합성 체크 권장
