flowchart TD
  %% ========== App Boot ==========
  A[프로그램 시작<br/>Program.Main] --> B[로깅/예외처리/크래시덤프 설치<br/>AppLogger/AppExceptionHandler/NativeCrashDumper]
  B --> C[MainForm 실행<br/>Application.Run(MainForm)]

  %% ========== Mode ==========
  C --> D{모드 선택}
  D -->|Teaching| T0[UcTeachingShell]
  D -->|Auto Run| R0[UcAutoRunShell]

  %% ========== Teaching ==========
  subgraph TEACHING[Teaching (기준 만들기)]
    T0 --> T1[Load 골든 이미지]
    T1 --> T2[ROI+ 검사영역 지정]
    T2 --> T3[Threshold/MinArea/Expected 등 파라미터 설정]

    T3 --> T4{NCC 사용?}
    T4 -->|예| T5[NCC 모델 저장 (*.ncm)<br/>NccPatternInspector.CreateAndWriteModel]
    T5 --> T6[참조 자세(ref pose) 저장(옵션)<br/>NccSharedModelState.SetWithReferencePose]
    T4 -->|아니오| T7[Blob 기반 레시피만 사용]

    T6 --> T8[검사 레시피 저장<br/>InspectionRecipe 생성]
    T7 --> T8
    T8 --> T9[세션 메모리 저장<br/>TeachingInspectionRecipeStore.Set]
  end

  %% ========== AutoRun ==========
  subgraph AUTORUN[Auto Run (검사 실행)]
    R0 --> R1[Load 검사 이미지(단일/폴더)]
    R1 --> R2[Run Inspect 또는 상단 Count/Inspection]

    R2 --> R3[Teaching 레시피 적용<br/>TryApplyTeachingInspectionRecipe]
    R3 --> R4[ROI 재생성 + Threshold 적용 + 파라미터 주입<br/>BlobInspectRunFlow 설정]
    R4 --> R5[검사 실행<br/>BlobInspectRunFlow.ExecuteSameAsRunInspectButton]

    R5 --> R6{Auto Cycle(폴더 배치) ON?}
    R6 -->|OFF| S1[단일 이미지 검사]
    R6 -->|ON| B1[폴더 전체 반복 검사<br/>RunAutoCyclePassAllAsync]

    %% --- single pass core ---
    S1 --> CORE
    B1 --> CORE

    %% --- core decision ---
    CORE{NCC 모델 존재/해석 가능?} -->|예| NCC1[NCC 전역 카운트 검사<br/>TryFullImageNccCountInspect]
    CORE -->|아니오| BLOB1[ROI 기반 Blob 검사<br/>RunSingleInspectionSyncAndRecord]

    %% NCC path details
    NCC1 --> NCC2[find_ncc_model로 매칭 후보 탐색]
    NCC2 --> NCC3[Score 범위 필터링(옵션)]
    NCC3 --> NCC4[개수/기대값 비교 → OK/NG]
    NCC4 --> OUT

    %% Blob path details
    BLOB1 --> BLOB2[선처리 훅(옵션): NCC로 ROI 추적/정렬 가능]
    BLOB2 --> BLOB3[Threshold 이진화 + Blob 검출]
    BLOB3 --> BLOB4[ROI 없음/Blob 0/기대개수 불일치/픽셀범위(옵션) → OK/NG]
    BLOB4 --> OUT

    %% outputs
    OUT[세션 결과 누적 + 오버레이/요약 표시] --> SAVE{사용자 저장?}
    SAVE -->|CSV| CSV[CSV 저장<br/>SaveSessionCsvToFileWithDialog]
    SAVE -->|Log txt| TXT[Inspection Log txt 저장<br/>SaveInspectionLogToTextFileWithDialog]
    SAVE -->|안 함| END[종료/다음 이미지]
  end
