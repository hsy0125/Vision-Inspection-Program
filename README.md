### Auto Run Page
<img width="1913" height="1032" alt="image" src="https://github.com/user-attachments/assets/dd9992d4-3780-48e8-806e-5a9e987ad88f" />
### Teaching Page
<img width="1913" height="1032" alt="image" src="https://github.com/user-attachments/assets/056a60fd-7c95-4b46-a641-f062bbe88a53" />
### Image load
<img width="1920" height="1032" alt="image" src="https://github.com/user-attachments/assets/5d88318d-6d25-4be6-927e-3f482131381d" />
--- 
### 패턴 검사
Teaching Page 에서의 패턴 검사(NCC 알고리즘)
<img width="1913" height="1032" alt="image" src="https://github.com/user-attachments/assets/fdfc7324-1856-46c8-8543-86c58cf93fa2" />
레시피 저장 후 Auto Run Page 에서의 검사 
<img width="1913" height="1032" alt="image" src="https://github.com/user-attachments/assets/3bddead5-b17b-4a9f-b0fc-fe0684b05b73" />

--- 
### 블랍 count
Teaching Page 에서의 Blob 검사 (count, Matching)
<img width="1913" height="1032" alt="image" src="https://github.com/user-attachments/assets/447d11aa-48ca-4b6a-b274-4d3724603dda" />
이미지를 사이클로 돌려서 실시간으로 blob 탐지하는 것 시연
<img width="1913" height="1032" alt="image" src="https://github.com/user-attachments/assets/285cd181-de81-41b3-98f8-829e31bbc280" />

---
### 다국어 변환 
<img width="767" height="360" alt="image" src="https://github.com/user-attachments/assets/1d6bf897-2ff0-49e6-a7a7-ceb7cdc2b5d6" />

---
### Log(검사 로그) / CSV (파라미터 값 저장)
<img width="418" height="327" alt="image" src="https://github.com/user-attachments/assets/d2e84764-f53f-4990-976c-cfce185e4f8b" />

---
## 0. 한 문장 요약
`InspectionProgram`은 **Teaching(티칭)**에서 “검사 기준(ROI/임계값/NCC 모델)”을 만들고 저장한 뒤, **Auto Run(자동 실행)**에서 그 기준을 적용해 이미지(단일/폴더)를 검사하여 **OK/NG** 판정 및 **CSV/로그**를 저장하는 **WinForms 기반 비전 검사 프로그램**이다.
<img width="424" height="840" alt="image" src="https://github.com/user-attachments/assets/a4495f3e-6a45-4750-8614-272104f6151e" />

---

## 1. 시스템 개요 
- **Teaching**: “정답지(골든 이미지)”를 열고, 검사할 영역(ROI)을 지정하고, 필요하면 “찾을 물체의 모양(NCC 모델)”을 저장한다.
- **Auto Run**: 검사할 이미지(한 장 또는 폴더)를 열고 Run을 누르면,
  - 모델이 있으면 **사진 전체에서 같은 패턴을 찾아 ‘개수’를 세고**,
  - 모델이 없으면 **지정한 영역(ROI) 안에서 밝기 기준으로 덩어리(Blob)를 찾아 ‘개수’를 센다**.
- 결과는 세션에 누적되며 **CSV 저장**, 화면의 **Inspection Log는 txt로 저장** 가능하다.

---

## 2. 프로젝트 구조(파일/모듈 역할 맵)

### 2.1 진입점/메인 UI
- `InspectionProgram/Program.cs`
  - 앱 부팅, `AppLogger` 초기화, 전역 예외 처리 등록
  - `MainForm` 실행
- `InspectionProgram/MainForm.cs`
  - 모드 전환: `AutoRun`, `Teaching`
  - 상단 메뉴 액션(Inspection/Count/Log 등)을 각 화면(UserControl)로 전달

### 2.2 화면(UserControl) 2개
- `InspectionProgram/UcTeachingShell.cs` (Teaching)
  - 골든 이미지 로드, ROI 설정
  - NCC 모델 저장/검사/카운트 기능
  - “검사 레시피 저장” (Auto Run에서 재사용할 파라미터 저장)
- `InspectionProgram/UcAutoRunShell.cs` (Auto Run)
  - 이미지/폴더 로드
  - Teaching 레시피 적용 후 검사 실행
  - 결과 누적, CSV 저장, Inspection Log txt 저장

### 2.3 공용 검사 플로우(핵심 엔진)
- `InspectionProgram/GUI/BlobInspectRunFlow.cs`
  - 단일 검사 + 폴더 전체 배치(자동 사이클) 검사
  - 결과 세션 누적 및 CSV 저장
  - 핵심 분기: **NCC 전역 카운트 vs ROI 기반 Blob 검사**

### 2.4 레시피(Teaching → AutoRun 전달 데이터)
- `InspectionProgram/Common/InspectionRecipe.cs`
  - ROI 좌표(픽셀), Threshold, MinArea, ExpectedBlobCount
  - NCC 모델 경로/템플릿 크기/스코어 필터 범위 등
- `InspectionProgram/Common/TeachingInspectionRecipeStore.cs`
  - 앱 실행 중(세션) 메모리에 레시피 1개 저장/조회

### 2.5 Halcon/NCC 관련
- `InspectionProgram/Halcon/NccPatternInspector.cs`
  - ROI로 NCC 모델 생성/저장(`*.ncm`)
  - 저장된 모델로 이미지 매칭 수행 (row/col/angle/score 반환)
- `InspectionProgram/Halcon/NccImageAlignment.cs`
  - 매칭 자세를 골든(참조) 자세로 맞추기 위한 이미지 강체 정렬(옵션)
- `InspectionProgram/Common/NccSharedModelState.cs`
  - Teaching에서 사용한 NCC 모델/참조 자세를 AutoRun과 공유 (세션 메모리)
- `InspectionProgram/Halcon/HalconResult.cs`
  - NCC 결과 구조 + score 필터링 + OK/NG 판정 유틸

### 2.6 로그/다국어/카메라(웹캠)
- 로그
  - `InspectionProgram/Common/AppLogger.cs`: 실행 경로 하위 `Log/`에 날짜별 로그 append
  - `InspectionProgram/Common/InspectionResultLogPaths.cs`: CSV/txt 기본 저장 경로 및 파일명 규칙
- 다국어
  - `InspectionProgram/Common/LocalizationService.cs`: 키 기반 다국어 문자열 매핑
- 카메라(Teaching Live 데모)
  - `InspectionProgram/Camera/IFrameSource.cs`: 프레임 공급자 인터페이스
  - `InspectionProgram/Camera/OpenCvWebcamFrameSource.cs`: OpenCV로 웹캠 프레임 제공

---
<img width="3600" height="2400" alt="InspectionProgram_Flowchart_KO" src="https://github.com/user-attachments/assets/d80dae58-c0eb-463e-9d25-6a593d7bd492" />

## 3. 전체 실행 플로우(코드 기준)

### 3.1 앱 시작
1) `Program.Main()`  
2) 로깅/크래시덤프/전역 예외 처리 설치  
3) `Application.Run(new MainForm())`

### 3.2 MainForm: 모드/상단 메뉴 라우팅
- 모드 전환:
  - AutoRun: `UcAutoRunShell` 표시
  - Teaching: `UcTeachingShell` 표시
- 상단 메뉴(중요):
  - `Inspection`, `Count` → AutoRun의 “Run inspect”와 동일 흐름 호출
  - `Log` → AutoRun의 Inspection Log txt 저장 호출
  - Teaching 모드에서는 `Live`, `Snap`을 Teaching으로 전달

### 3.3 Teaching → AutoRun으로 넘어가는 핵심(레시피)
- Teaching에서 ROI/Threshold/Expected 등의 파라미터를 확정
- “검사 레시피 저장” 클릭 시:
  - ROI 좌표/Threshold/MinArea/Expected/NCC 관련값이 `InspectionRecipe`로 구성됨
  - `TeachingInspectionRecipeStore.Set(recipe)`로 세션 메모리에 보관
- AutoRun에서 검사 시작 시:
  - `TryApplyTeachingInspectionRecipe()`가 레시피를 읽어
    - ROI를 뷰어에 재생성
    - Threshold 설정
    - `BlobInspectRunFlow`에 기대개수/픽셀필터/NCC 모델 정보를 주입

---

## 4. 검사 알고리즘(핵심) — “NCC vs Blob” 자동 분기

### 4.1 큰 그림
AutoRun에서 Run을 누르면 공용 엔진(`BlobInspectRunFlow`)이 실행되며,
1) **NCC 모델이 있으면**: 이미지 전체에서 패턴을 찾아 **개수 카운트(전역 NCC 카운트)**  
2) **NCC 모델이 없으면**: ROI 내부에서 밝기 임계값 기반 **Blob 카운트**

즉, 이 프로젝트의 Run은 “항상 Blob”이 아니라, **NCC 모델 유무/적용 상태에 따라 자동으로 검사 경로가 바뀜**.

### 4.2 NCC 기반 전역 패턴 카운트(정교/패턴 반복에 강함)
- 목적: 골든에서 저장한 패턴(템플릿)을 이미지 전체에서 찾아 **매칭 개수**를 센다.
- 출력: (Row, Column, Angle, Score)
- 핵심 판정 로직(요지)
  - 매칭 0개면 NG (“NCC 매칭 없음”)
  - 기대개수(Expected)가 있으면 개수 불일치 시 NG
  - score 범위(min~max) 필터 후 카운트에 반영
- 발표용 한 줄(현업):
  - “부품 한 개의 ‘모양’을 저장해두고, 생산 사진에서 그 부품이 **몇 개 있는지** 자동으로 세는 방식”

### 4.3 Blob 기반 ROI 검사(빠름/단순/영역 기반)
- 목적: 사용자가 지정한 ROI 안에서 threshold 이상 픽셀을 전경으로 보고 연결요소(Blob)를 찾아 카운트한다.
- 핵심 판정 로직(요지)
  - ROI가 없으면 NG
  - Blob이 0개면 NG
  - 기대개수(Expected)가 있으면 개수 불일치 시 NG
  - (옵션) ROI 내부 전경 픽셀 수가 [min,max] 범위를 벗어나면 NG
- 발표용 한 줄(현업):
  - “검사 구역을 네모로 지정하고, 그 안에서 밝은(혹은 어두운) 덩어리를 세어 기준과 비교”

### 4.4 ROI 추적 / 이미지 정렬(Alignment) 포인트
- ROI 추적: NCC 매칭 중심(row/col)을 이용해 **ROI를 자동 이동**시켜, 제품 위치가 흔들려도 ROI 기반 검사 안정성을 높임
- 이미지 정렬(옵션): 골든 자세(ref pose)에 맞춰 이미지를 회전/이동 변환하여 뷰어에 적용할 수 있음(기본 off)

---

## 5. 저장/로그/결과물
- **App 로그**: 실행 폴더 하위 `Log/`에 날짜별 `.log` 파일로 누적
- **Inspection Log(txt)**: AutoRun 화면의 로그 텍스트를 저장 대화상자로 `.txt`로 저장
- **CSV**: 세션 검사 결과를 `Time,FileName,Result,RoiCount,BlobCount,Threshold,Note` 형태로 저장

---

## 6. 현재 실행 불가(halcon.dll) 이슈를 “안전하게” 설명하는 문장
이 프로젝트는 `HalconDotNet` 기반 NCC 기능을 포함하기 때문에, 실행 환경에 HALCON 런타임 DLL(예: `halcon.dll`)이 없으면 비전 모듈 로딩이 불가합니다. 현재는 런타임/라이선스 환경이 없어 실행이 막혀 있지만, 코드 구조는 Teaching→AutoRun 플로우와 검사 분기(NCC/Blob)가 정상 동작하도록 설계되어 있습니다.

---

## 7. )

### 7.1 
- “이 프로젝트는 C# WinForms 기반 검사 UI이고, 핵심 검사 루프는 `BlobInspectRunFlow`로 공통화되어 있습니다.”
- “Teaching에서 ROI/Threshold/ExpectedCount와 NCC 모델을 만들고 `InspectionRecipe`로 묶어 세션 메모리에 저장합니다.”
- “AutoRun에서 Run 시 레시피를 적용해 ROI를 재구성한 뒤, 검사 루틴은 NCC 모델 존재 여부로 분기합니다.”
- “NCC 모델이 있으면 Halcon `find_ncc_model` 결과를 score 필터링 후 카운트하고, 기대개수와 비교해 OK/NG를 판단합니다.”
- “NCC가 없으면 ROI 내부에서 threshold 기반 blob 검출을 수행하고, blob count/전경 픽셀 범위로 NG를 판정합니다.”
- “결과는 세션으로 누적돼 CSV로 저장되고, 로그는 `Log/` 폴더에 남습니다.”

### 7.2 
- “이 시스템은 ‘기준을 먼저 가르치고(Teaching)’, 그 기준으로 ‘자동 검사(Auto Run)’하는 구조입니다.”
- “Teaching에서 기준 이미지로 검사 구역을 정하고, 필요하면 부품 모양을 모델로 저장합니다.”
- “Auto Run에서는 사진을 한 장 또는 폴더로 열고 Run을 누르면, 모델이 있으면 사진 전체에서 같은 부품을 찾아 개수를 세고, 모델이 없으면 지정한 구역 안에서 밝기 기준으로 덩어리를 찾아 개수를 셉니다.”
- “결과는 OK/NG로 보여주고, 누적 결과는 CSV로 저장돼 리포트/이력에 활용할 수 있습니다.”
- “현재 PC는 Halcon 런타임이 없어 실행은 막혀 있지만, 플로우와 로직은 코드상으로 완성되어 있습니다.”

