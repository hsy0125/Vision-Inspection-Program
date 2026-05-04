using System.Collections.Generic;

namespace InspectionProgram.Common
{
    public static class LocalizationService
    {
        private static readonly Dictionary<string, string[]> TextMap = new Dictionary<string, string[]>
        {
           { "AutoRun", new[] { "자동 실행", "Auto Run", "自动运行", "自動運転", "Tự động", "ऑटो रन" } },
            { "Teaching", new[] { "티칭", "Teaching", "教学", "ティーチング", "Dạy học", "टीचिंग" } },
            { "TeachingInfo", new[] { "티칭 정보", "Teaching info", "教学信息", "ティーチング情報", "Thông tin dạy", "टीचिंग जानकारी" } },
            { "Option", new[] { "옵션", "Option", "选项", "オプション", "Tùy chọn", "विकल्प" } },
            { "Board", new[] { "장치", "Device", "设备", "デバイス", "Thiết bị", "डिवाइस" } },
            { "Device", new[] { "장치", "Device", "设备", "デバイス", "Thiết bị", "डिवाइस" } },
            { "CurrentDevice", new[] { "현재 장치", "Current Device", "当前设备", "現在デバイス", "Thiết bị hiện tại", "वर्तमान डिवाइस" } },
            { "CurrentTime", new[] { "시간", "Time", "时间", "時間", "Thời gian", "समय" } },
            { "Version", new[] { "버전", "Version", "版本", "バージョン", "Phiên bản", "संस्करण" } },
            { "Network", new[] { "네트워크", "Network", "网络", "ネットワーク", "Mạng", "नेटवर्क" } },
            { "Connected", new[] { "연결됨", "Connected", "已连接", "接続", "Đã kết nối", "जुड़ा हुआ" } },
            { "Disconnected", new[] { "연결 안 됨", "Disconnected", "未连接", "未接続", "Ngắt kết nối", "डिस्कनेक्ट" } },
            { "UserLevel", new[] { "사용자 권한", "User Authority", "用户权限", "ユーザー権限", "Quyền người dùng", "उपयोगकर्ता अधिकार" } },
            { "Operator", new[] { "운영자", "Operator", "操作员", "オペレータ", "Vận hành", "ऑपरेटर" } },
            { "Engineer", new[] { "엔지니어", "Engineer", "工程师", "エンジニア", "Kỹ sư", "इंजीनियर" } },
            { "Admin", new[] { "관리자", "Admin", "管理员", "管理者", "Quản trị", "एडमिन" } },
            { "Test", new[] { "Test", "Test", "Test", "Test", "Test", "Test" } },
            { "UseGrid", new[] { "그리드 레이아웃", "Grid Layout", "网格布局", "グリッド表示", "Bố cục lưới", "ग्रिड लेआउट" } },
            { "InspectionLog", new[] { "검사 로그", "Inspection Log", "检查日志", "検査ログ", "Nhật ký kiểm tra", "निरीक्षण लॉग" } },
            { "SystemLog", new[] { "시스템 로그", "System Log", "系统日志", "システムログ", "Nhật ký hệ thống", "सिस्टम लॉग" } },
            { "CountSummary", new[] { "카운트", "Count", "计数", "カウント", "Đếm", "गिनती" } },
            { "Camera", new[] { "카메라", "Camera", "相机", "カメラ", "Camera", "कैमरा" } },
            { "Total", new[] { "합계", "Total", "总数", "合计", "Tổng", "कुल" } },
            { "Good", new[] { "양품", "Good", "良品", "良品", "Tốt", "अच्छा" } },
            { "Reject", new[] { "불량", "Reject", "不良", "不良", "Loại", "अस्वीकृत" } },
            { "Yield", new[] { "수율", "Yield", "良率", "歩留り", "Tỷ lệ", "यील्ड" } },
            { "TeachingTabs", new[] {
                "설정|기준점|크기|무시영역|볼위치|볼|X표시|스크래치|이물|콘덴서|자동보정|거리",
                "Setup|Fiducial|Size|Don't Care|Ball Pos|Ball|X Mark|Scratch|Foreign Material|Capacitor|AutoCal|Distance",
                "设置|基准点|尺寸|忽略区|球位置|球|X标记|划痕|异物|电容|自动校准|距离",
                "設定|基準|サイズ|無視領域|ボール位置|ボール|Xマーク|傷|異物|コンデンサ|自動補正|距離",
                "Thiết lập|Mốc|Kích thước|Bỏ qua|Vị trí bóng|Bóng|Dấu X|Trầy xước|Dị vật|Tụ|Tự hiệu chỉnh|Khoảng cách",
                "सेटअप|फ़िडूशियल|आकार|नज़रअंदाज़|बॉल स्थिति|बॉल|X निशान|खरोंच|विदेशी पदार्थ|कैपेसिटर|ऑटोकैल|दूरी" } },
            { "DeviceSettings", new[] { "장치 설정", "Device Setting", "设备设定", "デバイス設定", "Cài đặt thiết bị", "डिवाइस सेटिंग" } },
            { "History", new[] { "이력", "History", "历史", "履历", "Lịch sử", "इतिहास" } },
            { "SaveOption", new[] { "옵션 저장", "Save Option", "保存选项", "保存オプション", "Lưu tùy chọn", "विकल्प सहेजें" } },
            { "Priority", new[] { "우선순위", "Priority", "优先级", "優先度", "Ưu tiên", "प्राथमिकता" } },
            { "DisplayStyle", new[] { "표시 스타일", "Display Style", "显示样式", "表示样式", "Kiểu hiển thị", "प्रदर्शन शैली" } },
            { "Calibration", new[] { "보정", "Calibration", "校准", "キャリブレーション", "Hiệu chuẩn", "कैलिब्रेशन" } },
            { "Help", new[] { "도움말", "Help", "帮助", "ヘルプ", "Trợ giúp", "मदद" } },
            { "Run", new[] { "실행", "Run", "运行", "実行", "Chạy", "चलाएँ" } },
            { "Inspection", new[] { "검사", "Inspection", "检测", "検査", "Kiểm tra", "निरीक्षण" } },
            { "Count", new[] { "개수", "Count", "计数", "カウント", "Đếm", "गिनती" } },
            { "Log", new[] { "로그", "Log", "日志", "ログ", "Nhật ký", "लॉग" } },
            { "Error", new[] { "오류", "Error", "错误", "エラー", "Lỗi", "त्रुटि" } },
            { "DockingReset", new[] { "도킹 초기화", "Docking Reset", "复位布局", "ドッキング初期化", "Đặt lại docking", "डॉकिंग रीसेट" } },
            { "Live", new[] { "라이브", "Live", "实时", "ライブ", "Live", "लाइव" } },
            { "Snap", new[] { "촬영", "Snap", "抓拍", "スナップ", "Chụp", "स्नैप" } },
            { "RoiController", new[] { "ROI 제어", "ROI Controller", "ROI控制", "ROIコントローラ", "Điều khiển ROI", "ROI नियंत्रण" } },
            { "DeviceList", new[] { "장치 목록", "Device List", "设备列表", "デバイス一覧", "Danh sách thiết bị", "डिवाइस सूची" } },
            { "SpecDB", new[] { "사양 DB", "Spec DB", "规格DB", "仕様DB", "Spec DB", "स्पेक DB" } },
            { "NccPatternMatching", new[] { " 패턴 매칭", " Pattern Matching", " 模板匹配", " パターンマッチング", "So khớp mẫu ", " पैटर्न मिलान" } },
            { "NccModelSave", new[] { " 모델 저장", "Save  Model", "保存  模型", " モデル保存", "Lưu mô hình ", " मॉडल सहेजें" } },
            { "NccInspect", new[] { " 검사", " Inspect", " 检测", " 検査", "Kiểm tra ", " निरीक्षण" } },
            { "NccMinScore", new[] { "판정 최소 점수 ", "Min score ", "最小判定分数", "判定最小スコア", "Điểm tối thiểu ", "न्यूनतम स्कोर " } },
            { "NccCount", new[] { "카운트", "Count", "计数", "カウント", "Đếm", "गिनती" } },
            { "NccCountMin", new[] { "카운트 Min ", "Count Min ", "计数最小值", "カウント最小", "Đếm Min ", "गिनती न्यूनतम " } },
            { "NccCountMax", new[] { "카운트 Max ", "Count Max ", "计数最大值", "カウント最大", "Đếm Max ", "गिनती अधिकतम " } },
            { "NccCountJudgeMin", new[] { "판정 최소 점수 (카운트)", "Min score (Count)", "最小判定分数（计数）", "判定最小スコア（カウント）", "Điểm tối thiểu (Đếm)", "न्यूनतम स्कोर (गिनती)" } },
            { "Pre", new[] { "전처리", "Pre", "预处理", "前処理", "Tiền xử lý", "प्री-प्रोसेस" } },
            { "Blur", new[] { "블러", "Blur", "模糊", "ぼかし", "Làm mờ", "धुंधला" } },
            { "BG", new[] { "배경", "BG", "背景", "背景", "Nền", "पृष्ठभूमि" } },
            { "MinArea", new[] { "최소 면적", "Min Area", "最小面积", "最小面積", "Diện tích tối thiểu", "न्यूनतम क्षेत्रफल" } },
            { "Expected", new[] { "기대값", "Expected", "期望值", "期待値", "Giá trị kỳ vọng", "अपेक्षित मान" } },
            { "NccCounting", new[] { "카운트 중", "Counting", "计数中", "カウント中", "Đang đếm", "गिनती चल रही है" } },
            { "NccCountError", new[] { "카운트 오류", "Count error", "计数错误", "カウントエラー", "Lỗi đếm", "गिनती त्रुटि" } },
            { "NccCountFail", new[] { "카운트 실패", "Count failed", "计数失败", "カウント失敗", "Đếm thất bại", "गिनती विफल" } },
            { "NccModelSaved", new[] { "모델 저장됨", "Model saved", "模型已保存", "モデル保存済み", "Đã lưu mô hình", "मॉडल सहेजा गया" } },
            { "NccInspecting", new[] { "검사 중", "Inspecting", "检测中", "検査中", "Đang kiểm tra", "निरीक्षण चल रहा है" } },
            { "NccError", new[] { "오류", "Error", "错误", "エラー", "Lỗi", "त्रुटि" } },
            { "NccNoResult", new[] { "결과 없음", "No result", "无结果", "結果なし", "Không có kết quả", "कोई परिणाम नहीं" } },
            { "AvgNccScore", new[] { "Match (일치율)", "Match(match rate)", "Match（匹配率）", "Match（一致率）", "Match (khớp)", "Match (मैच)" } },
            { "JudgeOK", new[] { "OK", "OK", "OK", "OK", "OK", "OK" } },
            { "JudgeNG", new[] { "NG", "NG", "NG", "NG", "NG", "NG" } },
            { "Missing", new[] { "누락", "Missing", "缺失", "不足", "Thiếu", "अभाव" } },
            { "Extra", new[] { "과다", "Extra", "多余", "過剰", "Thừa", "अधिक" } },
            { "TestRun", new[] { "테스트 실행", "Test Run", "测试运行", "テスト運転", "Chạy thử", "टेस्ट रन" } },
            { "ManualSetup", new[] { "수동 설정", "Manual Setup", "手动设置", "手動設定", "Thiết lập tay", "मैनुअल सेटअप" } },
            { "OpenImage", new[] { "불러오기", "Load", "打开", "読込", "Mở", "लोड करें" } },
            { "ClearImage", new[] { "지우기", "Clear", "清除", "削除", "Xóa", "साफ़ करें" } },
            { "FitImage", new[] { "맞춤", "Fit", "适配", "フィット", "Vừa khít", "फ़िट" } },
            { "ViewerReady", new[] { "준비됨", "Ready", "就绪", "準備完了", "Sẵn sàng", "तैयार" } },
            { "TcpIpTitle", new[] { "TCP/IP 연결", "TCP/IP Connection", "TCP/IP连接", "TCP/IP接続", "Kết nối TCP/IP", "TCP/IP कनेक्शन" } },
            { "Address", new[] { "IP 주소", "IP Address", "IP地址", "IPアドレス", "Địa chỉ IP", "IP पता" } },
            { "Port", new[] { "포트", "Port", "端口", "ポート", "Cổng", "पोर्ट" } },
            { "Connect", new[] { "연결", "Connect", "连接", "接続", "Kết nối", "कनेक्ट" } },
            { "Disconnect", new[] { "연결 해제", "Disconnect", "断开", "切断", "Ngắt kết nối", "डिस्कनेक्ट" } },
            { "UserLevelChange", new[] { "사용자 권한", "User Authority", "用户权限", "ユーザー権限", "Quyền người dùng", "उपयोगकर्ता अधिकार" } },
            { "Password", new[] { "비밀번호", "Password", "密码", "パスワード", "Mật khẩu", "पासवर्ड" } },
            { "Apply", new[] { "적용", "Apply", "应用", "適用", "Áp dụng", "लागू करें" } },
            { "Confirm", new[] { "확인", "Confirm", "确认", "確認", "Xác nhận", "पुष्टि करें" } },
            { "SafetyMode", new[] { "안전 모드", "Safety Mode", "安全模式", "セーフティ", "An toàn", "सुरक्षा मोड" } },
            { "Cancel", new[] { "취소", "Cancel", "取消", "キャンセル", "Hủy", "रद्द करें" } },
            { "Create", new[] { "생성", "Create", "创建", "作成", "Tạo", "बनाएँ" } },
            { "Copy", new[] { "복사", "Copy", "复制", "コピー", "Sao chép", "कॉपी" } },
            { "Select", new[] { "선택", "Select", "选择", "選択", "Chọn", "चुनें" } },
            { "Rename", new[] { "이름 바꾸기", "Rename", "重命名", "名前変更", "Đổi tên", "नाम बदलें" } },
            { "Delete", new[] { "삭제", "Delete", "删除", "削除", "Xóa", "हटाएँ" } },
            { "Close", new[] { "닫기", "Close", "关闭", "閉じる", "Đóng", "बंद करें" } },
            { "Name", new[] { "이름", "Name", "名称", "名称", "Tên", "नाम" } },
            { "Label", new[] { "라벨", "Label", "标签", "ラベル", "Nhãn", "लेबल" } },
            { "Created", new[] { "생성", "Create", "创建", "作成", "Tạo", "बनाया गया" } },
            { "LastWrite", new[] { "마지막 기록", "LastWrite", "最后写入", "最終更新", "Ghi cuối", "अंतिम लेखन" } },
            { "LastAccess", new[] { "마지막 접근", "LastAccess", "最后访问", "最終アクセス", "Truy cập cuối", "अंतिम पहुँच" } },
            { "Permissions", new[] { "권한", "Permissions", "权限", "権限", "Quyền", "अनुमतियाँ" } },
            { "ShowPassword", new[] { "비밀번호 표시", "Show password", "显示密码", "パスワード表示", "Hiện mật khẩu", "पासवर्ड दिखाएँ" } },
            { "SignIn", new[] { "로그인", "Sign in", "登录", "サインイン", "Đăng nhập", "साइन इन" } },
            { "ChangePassword", new[] { "비밀번호 변경", "Change password", "修改密码", "パスワード変更", "Đổi mật khẩu", "पासवर्ड बदलें" } },
            { "Save", new[] { "저장", "Save", "保存", "保存", "Lưu", "सहेजें" } },
            { "AutoRunGroup", new[] { "자동 실행", "Auto Run", "自动运行", "自動運転", "Tự động", "ऑटो रन" } },
            { "TeachingGroup", new[] { "티칭", "Teaching", "教学", "ティーチング", "Dạy học", "टीचिंग" } },
            { "SettingGroup", new[] { "설정", "Setting", "设定", "設定", "Cài đặt", "सेटिंग" } },
            { "Language", new[] { "언어", "Language", "语言", "言語", "Ngôn ngữ", "भाषा" } },
            { "User", new[] { "사용자", "User", "用户", "ユーザー", "Người dùng", "उपयोगकर्ता" } },
            { "PasswordChanged", new[] { "비밀번호가 변경되었습니다.", "Password changed.", "密码已更改。", "パスワードを変更しました。", "Đã đổi mật khẩu.", "पासवर्ड बदल गया।" } },
            { "PermissionSaved", new[] { "권한이 저장되었습니다.", "Permissions saved.", "权限已保存。", "権限を保存しました。", "Đã lưu quyền.", "अनुमतियाँ सहेजी गईं।" } },
            { "InputDeviceName", new[] { "장치 이름을 입력하세요", "Enter device name", "输入设备名称", "デバイス名入力", "Nhập tên thiết bị", "डिवाइस नाम दर्ज करें" } },
            { "InputCopyName", new[] { "복사할 장치 이름을 입력하세요", "Enter copied device name", "输入复制设备名称", "コピー先デバイス名入力", "Nhập tên thiết bị sao chép", "कॉपी डिवाइस नाम दर्ज करें" } },
            { "InputRenameName", new[] { "새 장치 이름을 입력하세요", "Enter new device name", "输入新设备名称", "新しいデバイス名入力", "Nhập tên thiết bị mới", "नया डिवाइस नाम दर्ज करें" } },
            { "InputPassword", new[] { "새 비밀번호를 입력하세요", "Enter new password", "输入新密码", "新しいパスワード入力", "Nhập mật khẩu mới", "नया पासवर्ड दर्ज करें" } },
            { "QuestionDeleteDevice", new[] { "선택한 장치를 삭제하시겠습니까?", "Delete selected device?", "删除选中的设备吗？", "選択したデバイスを削除しますか？", "Xóa thiết bị đã chọn?", "चयनित डिवाइस हटाएँ?" } },
            { "QuestionOverwrite", new[] { "이미 있습니다. 덮어쓰시겠습니까?", "Already exists. Overwrite?", "已存在。要覆盖吗？", "既に存在します。上書きしますか？", "Đã tồn tại. Ghi đè?", "पहले से मौजूद है। ओवरराइट?" } },
            { "NotAllowed", new[] { "허용되지 않습니다.", "Not allowed.", "不允许。", "許可されていません。", "Không được phép.", "अनुमति नहीं है।" } },
            { "InvalidPassword", new[] { "비밀번호가 올바르지 않습니다.", "Password is invalid.", "密码无效。", "パスワードが無効です。", "Mật khẩu không hợp lệ.", "पासवर्ड अमान्य है।" } },
            { "AutoRunSample", new[] { "자동 실행 페이지가 로드되었습니다.", "Auto Run page loaded.", "自动运行页面已加载。", "自動運転ページを読み込みました。", "Đã nạp trang Auto Run.", "ऑटो रन पेज लोड हुआ।" } },
            { "TeachingSample", new[] { "티칭 페이지가 로드되었습니다.", "Teaching page loaded.", "教学页面已加载。", "ティーチングページを読み込みました。", "Đã nạp trang Teaching.", "टीचिंग पेज लोड हुआ।" } },
            { "OptionSample", new[] { "옵션 페이지가 로드되었습니다.", "Option page loaded.", "选项页面已加载。", "オプションページを読み込みました。", "Đã nạp trang Option.", "विकल्प पेज लोड हुआ।" } }
            ,{ "SaveImage", new[] { "저장", "Save", "保存", "保存", "Lưu", "सहेजें" } }
            ,{ "ZoomMode", new[] { "줌", "Zm", "缩放", "ズーム", "Thu phóng", "ज़ूम" } }
            ,{ "ZoomIn", new[] { "확대", "Z.I", "放大", "拡大", "Phóng to", "ज़ूम इन" } }
            ,{ "ZoomOut", new[] { "축소", "Z.O", "缩小", "縮小", "Thu nhỏ", "ज़ूम आउट" } }
            ,{ "OverlayClear", new[] { "오버레이 지우기", "Overlay Clear", "清除叠加", "オーバーレイ消去", "Xóa lớp phủ", "ओवरले साफ़" } }
            ,{ "Cross", new[] { "십자", "Cross", "十字线", "クロス", "Dấu cộng", "क्रॉस" } }
            ,{ "Gray", new[] { "그레이", "Gray", "灰度", "グレー", "Xám", "ग्रे" } }
            ,{ "Average", new[] { "평균", "Average", "平均", "平均", "Trung bình", "औसत" } }
            ,{ "SyncShort", new[] { "동기화", "Sync", "同步", "同期", "Đồng bộ", "सिंक" } }
            ,{ "MapShort", new[] { "맵", "Map", "地图", "マップ", "Bản đồ", "मैप" } }
            ,{ "RoiAdd", new[] { "ROI+", "ROI+", "ROI+", "ROI+", "ROI+", "ROI+" } }
            ,{ "RoiSave", new[] { "ROI 저장", "ROI Save", "ROI保存", "ROI保存", "Lưu ROI", "ROI सहेजें" } }
            ,{ "Threshold", new[] { "임계값", "Threshold", "阈值", "しきい値", "Ngưỡng", "थ्रेशोल्ड" } }
            ,{ "AutoCycleRun", new[] { "자동 사이클 (Run)", "Auto Cycle (Run)", "自动循环（Run）", "自動サイクル (Run)", "Chu kỳ tự động (Run)", "ऑटो साइकिल (Run)" } }
            ,{ "RunInspectStep", new[] { "검사 실행", "Run inspect", "运行检测", "検査実行", "Chạy kiểm tra", "निरीक्षण चलाएँ" } }
            ,{ "StopShort", new[] { "중지", "Stop", "停止", "停止", "Dừng", "रोकें" } }
            ,{ "SaveCsvStep", new[] { "CSV 저장", "Save CSV", "保存CSV", "CSV保存", "Lưu CSV", "CSV सहेजें" } }
            ,{ "SystemReady", new[] { "시스템 준비됨.", "System ready.", "系统就绪。", "システム準備完了。", "Hệ thống sẵn sàng.", "सिस्टम तैयार है।" } }
            ,{ "InspectionUiFlow", new[] {
                "검사 UI 흐름\r\n1) Load — [예] 폴더 / [아니오] 이미지 파일, ROI+ (선택)\r\n2) Threshold — 밝기 하한(이진화) 미리보기\r\n3) Run — 티칭 «검사 레시피» 자동 적용 후 검사(NCC 있으면 전역 패턴 카운트). 자동 사이클 시 목록 전체\r\n4) CSV 저장 — 세션 검사 결과(OK/NG) 파일 쓰기\r\n상단 «로그» — Inspection Log 내용을 .txt 로 저장\r\n※ 티칭에서 «검사 레시피 저장» 후 — «3) Run» 또는 상단 «개수»로 동일 검사",
                "Inspection UI flow\r\n1) Load - [Yes] folder / [No] image file, ROI+ (optional)\r\n2) Threshold - brightness cutoff preview\r\n3) Run - apply Teaching recipe then inspect (global NCC count if model exists). Auto cycle runs all images\r\n4) Save CSV - write session results (OK/NG)\r\nTop \"Log\" saves Inspection Log to .txt\r\n* After saving recipe in Teaching, run same inspection via \"3) Run\" or top \"Count\"",
                "检测UI流程\r\n1) Load - [是] 文件夹 / [否] 单张图片，ROI+（可选）\r\n2) Threshold - 亮度阈值预览\r\n3) Run - 先应用Teaching配方再检测（有NCC模型时全图计数）\r\n4) 保存CSV - 保存会话结果（OK/NG）\r\n顶部“日志”可将Inspection Log保存为.txt\r\n* Teaching保存配方后，可通过“3) Run”或顶部“Count”执行同样检测",
                "検査UIフロー\r\n1) Load - [はい] フォルダ / [いいえ] 画像ファイル、ROI+（任意）\r\n2) Threshold - 輝度しきい値プレビュー\r\n3) Run - Teachingレシピ適用後に検査（NCCモデルがあれば全体カウント）\r\n4) CSV保存 - セッション結果（OK/NG）を保存\r\n上部「ログ」でInspection Logを.txt保存\r\n* Teachingでレシピ保存後は「3) Run」または上部「Count」で同一検査",
                "Luồng UI kiểm tra\r\n1) Load - [Có] thư mục / [Không] ảnh đơn, ROI+ (tùy chọn)\r\n2) Threshold - xem trước ngưỡng sáng\r\n3) Run - áp dụng recipe từ Teaching rồi kiểm tra (nếu có mô hình NCC sẽ đếm toàn ảnh)\r\n4) Lưu CSV - lưu kết quả phiên (OK/NG)\r\n\"Log\" phía trên lưu Inspection Log ra .txt\r\n* Sau khi lưu recipe ở Teaching, chạy lại bằng \"3) Run\" hoặc \"Count\"",
                "इंस्पेक्शन UI फ्लो\r\n1) Load - [हाँ] फ़ोल्डर / [नहीं] इमेज फ़ाइल, ROI+ (वैकल्पिक)\r\n2) Threshold - ब्राइटनेस कटऑफ प्रीव्यू\r\n3) Run - Teaching रेसिपी लागू करके निरीक्षण (NCC मॉडल हो तो ग्लोबल काउंट)\r\n4) CSV सहेजें - सेशन परिणाम (OK/NG) सहेजें\r\nऊपर \"Log\" से Inspection Log को .txt में सेव करें\r\n* Teaching में रेसिपी सेव के बाद \"3) Run\" या ऊपर \"Count\" से वही निरीक्षण चलाएँ" } }
            ,{ "PatternTab", new[] { "패턴", "Pattern", "图案", "パターン", "Mẫu", "पैटर्न" } }
            ,{ "CountTab", new[] { "개수", "Count", "计数", "カウント", "Đếm", "गिनती" } }
            ,{ "NccPanelTitle", new[] { "패턴 매칭", "Pattern matching", "图案匹配", "パターン照合", "So khớp mẫu", "पैटर्न मिलान" } }
            ,{ "ModelSave", new[] { "모델 저장", "Save model", "保存模型", "モデル保存", "Lưu mô hình", "मॉडल सहेजें" } }
            ,{ "PatternInspect", new[] { "패턴 검사", "Pattern inspect", "图案检测", "パターン検査", "Kiểm tra mẫu", "पैटर्न निरीक्षण" } }
            ,{ "SaveInspectionRecipe", new[] { "검사 레시피 저장", "Save inspection recipe", "保存检测配方", "検査レシピ保存", "Lưu công thức kiểm tra", "निरीक्षण रेसिपी सहेजें" } }
            ,{ "BlobCountRun", new[] { "개수 카운트", "Blob count", "Blob 计数", "Blob カウント", "Đếm blob", "ब्लॉब गिनती" } }
            ,{ "RoiForegroundPixelRange", new[] { "ROI 전경 픽셀 범위", "ROI foreground pixel range", "ROI 前景像素范围", "ROI 前景画素範囲", "Phạm vi pixel ROI", "ROI अग्रभूमि पिक्सेल सीमा" } }
            ,{ "RangeSep", new[] { "~", "~", "~", "~", "~", "~" } }
            ,{ "AppWindowTitle", new[] { "Vision Inspect", "Vision Inspect", "Vision Inspect", "Vision Inspect", "Vision Inspect", "Vision Inspect" } }
            ,{ "HelpDockingSample", new[] { "도킹 UI 예제 도움말입니다.", "Docking UI sample help.", "停靠 UI 示例帮助。", "ドッキングUIサンプルヘルプ。", "Trợ giúp mẫu UI docking.", "डॉकिंग UI नमूना सहायता।" } }
            ,{ "InspectFlowCaption", new[] { "검사", "Inspect", "检测", "検査", "Kiểm tra", "निरीक्षण" } }
            ,{ "LoadImageChoice", new[] {
                "이미지 한 장을 열까요, 폴더 전체(목록)를 열까요?\n\n[예] 폴더\n[아니오] 이미지 파일\n[취소] 취소",
                "Open a single image or a whole folder (list)?\n\n[Yes] Folder\n[No] Image file\n[Cancel] Cancel",
                "打开单张图片还是整个文件夹（列表）？\n\n[是] 文件夹\n[否] 图片文件\n[取消] 取消",
                "1枚の画像を開きますか、フォルダ全体（一覧）を開きますか？\n\n[はい] フォルダ\n[いいえ] 画像ファイル\n[キャンセル] キャンセル",
                "Mở một ảnh hay cả thư mục (danh sách)?\n\n[Có] Thư mục\n[Không] File ảnh\n[Hủy] Hủy",
                "एक इमेज खोलें या पूरा फ़ोल्डर (सूची)?\n\n[हाँ] फ़ोल्डर\n[नहीं] इमेज फ़ाइल\n[रद्द] रद्द" } }
            ,{ "LoadStepCaption", new[] { "1) 불러오기", "1) Load", "1) 加载", "1) 読込", "1) Load", "1) लोड" } }
            ,{ "FolderBrowserInspectImages", new[] { "검사에 사용할 이미지가 있는 폴더", "Folder containing images to inspect", "包含待检测图像的文件夹", "検査画像のあるフォルダ", "Thư mục chứa ảnh kiểm tra", "निरीक्षण इमेज वाला फ़ोल्डर" } }
            ,{ "ErrFolderReadFmt", new[] { "폴더를 읽을 수 없습니다: {0}", "Cannot read folder: {0}", "无法读取文件夹: {0}", "フォルダを読めません: {0}", "Không đọc được thư mục: {0}", "फ़ोल्डर नहीं पढ़ा जा सका: {0}" } }
            ,{ "MsgNoImagesInFolder", new[] { "이 폴더에 지원하는 이미지가 없습니다.", "No supported images in this folder.", "此文件夹中没有支持的图像。", "このフォルダに対応画像がありません。", "Thư mục không có ảnh được hỗ trợ.", "इस फ़ोल्डर में कोई समर्थित इमेज नहीं।" } }
            ,{ "MsgLoadImageFirst", new[] { "먼저 Load로 이미지를 불러오세요.", "Load an image first using Load.", "请先用 Load 加载图像。", "先に Load で画像を読み込んでください。", "Hãy Load ảnh trước.", "पहले Load से इमेज लोड करें।" } }
            ,{ "MsgNoImageListForBatch", new[] { "이미지 목록이 없습니다. (파일/폴더 Load로 이미지를 열어 주세요.)", "No image list. (Open images via file/folder Load.)", "无图像列表。（请通过文件/文件夹 Load 打开图像。）", "画像リストがありません。（ファイル/フォルダの Load で開いてください。）", "Không có danh sách ảnh. (Mở bằng Load file/thư mục.)", "इमेज सूची नहीं है। (Load से फ़ोल्डर/फ़ाइल खोलें।)" } }
            ,{ "CsvDialogTitleShort", new[] { "CSV", "CSV", "CSV", "CSV", "CSV", "CSV" } }
            ,{ "MsgNoSessionRowsCsv", new[] { "저장할 세션 행이 없습니다. (먼저 검사 실행을 하세요.)", "No session rows to save. (Run inspect first.)", "没有可保存的会话行。（请先运行检测。）", "保存するセッション行がありません。（先に検査を実行してください。）", "Không có dòng phiên để lưu. (Chạy kiểm tra trước.)", "सहेजने के लिए कोई सेशन पंक्ति नहीं। (पहले निरीक्षण चलाएँ।)" } }
            ,{ "CsvSaveDialogTitle", new[] { "검사 세션 CSV 저장", "Save inspection session CSV", "保存检测会话 CSV", "検査セッションCSV保存", "Lưu CSV phiên kiểm tra", "निरीक्षण सत्र CSV सहेजें" } }
            ,{ "CsvFileFilter", new[] { "CSV|*.csv|모든 파일|*.*", "CSV|*.csv|All files|*.*", "CSV|*.csv|所有文件|*.*", "CSV|*.csv|すべてのファイル|*.*", "CSV|*.csv|Tất cả tệp|*.*", "CSV|*.csv|सभी फ़ाइलें|*.*" } }
            ,{ "ErrCsvWriteFmt", new[] { "CSV를 쓰지 못했습니다: {0}", "Could not write CSV: {0}", "无法写入 CSV: {0}", "CSVを書けません: {0}", "Không ghi được CSV: {0}", "CSV नहीं लिखा जा सका: {0}" } }
            ,{ "MsgCsvSavedFmt", new[] { "CSV를 저장했습니다.\r\n\r\n{0}", "CSV saved.\r\n\r\n{0}", "已保存 CSV。\r\n\r\n{0}", "CSVを保存しました。\r\n\r\n{0}", "Đã lưu CSV.\r\n\r\n{0}", "CSV सहेजा गया।\r\n\r\n{0}" } }
            ,{ "AutoCsvSavedLogFmt", new[] {
                "CSV 자동 저장 ({1}행): {0}",
                "CSV auto-saved ({1} rows): {0}",
                "CSV 自动保存（{1} 行）: {0}",
                "CSV自動保存（{1}行）: {0}",
                "CSV tự động ({1} dòng): {0}",
                "CSV ऑटो-सेव ({1} पंक्तियाँ): {0}" } }
            ,{ "AutoCsvFailLogFmt", new[] {
                "CSV 자동 저장 실패: {0}",
                "CSV auto-save failed: {0}",
                "CSV 自动保存失败: {0}",
                "CSV自動保存失敗: {0}",
                "Lưu CSV tự động thất bại: {0}",
                "CSV ऑटो-सेव विफल: {0}" } }
            ,{ "InspectAvgDash", new[] { "평균=—", "Avg=—", "平均=—", "平均=—", "TB=—", "औसत=—" } }
            ,{ "InspectCountFmt", new[] { "개수(n)={0}", "Count (n)={0}", "个数(n)={0}", "個数(n)={0}", "Số (n)={0}", "गिनती(n)={0}" } }
            ,{ "NoteRoiMissing", new[] { "ROI 없음", "No ROI", "无 ROI", "ROIなし", "Không có ROI", "कोई ROI नहीं" } }
            ,{ "NoteBlobZero", new[] { "ROI 내 Blob 0", "No blobs in ROI", "ROI 内无 Blob", "ROI内Blob0", "Không có blob trong ROI", "ROI में कोई ब्लॉब नहीं" } }
            ,{ "NoteBlobCountMismatchFmt", new[] { "Blob 개수 불일치: expected={0}, actual={1}", "Blob count mismatch: expected={0}, actual={1}", "Blob 数量不符: expected={0}, actual={1}", "Blob個数不一致: expected={0}, actual={1}", "Số blob không khớp: expected={0}, actual={1}", "ब्लॉब गिनती मेल नहीं: expected={0}, actual={1}" } }
            ,{ "NccNoMatchNote", new[] { "매칭 없음", "No match", "无 匹配", "一致なし", "Không khớp ", "कोई  मैच नहीं" } }
            ,{ "NccFilterZeroNoteFmt", new[] { " 필터 후 매칭 0 (raw={0})", "No matches after filter (raw={0})", " 过滤后匹配为 0 (raw={0})", "フィルタ後一致0 (raw={0})", "Sau lọc  không khớp (raw={0})", " फ़िल्टर के बाद 0 मैच (raw={0})" } }
            ,{ "NccCountMismatchFmt", new[] { "개수 불일치: expected={0}, actual={1}", "Count mismatch: expected={0}, actual={1}", "数量不符: expected={0}, actual={1}", "個数不一致: expected={0}, actual={1}", "Số không khớp: expected={0}, actual={1}", "गिनती मेल नहीं: expected={0}, actual={1}" } }
            ,{ "BatchStartFmt", new[] { "자동 사이클 시작: {0}장", "Auto cycle start: {0} image(s)", "自动循环开始: {0} 张", "自動サイクル開始: {0}枚", "Bắt đầu chu kỳ: {0} ảnh", "ऑटो साइकिल शुरू: {0} इमेज" } }
            ,{ "BatchCancelled", new[] { "자동 사이클 취소됨", "Auto cycle cancelled", "自动循环已取消", "自動サイクル取消", "Đã hủy chu kỳ", "साइकिल रद्द" } }
            ,{ "SpeedLogFmt", new[] { "속도: {0:0.0} 장/초 (avg {1:0} ms/장)", "Speed: {0:0.0} img/s (avg {1:0} ms/img)", "速度: {0:0.0} 张/秒 (平均 {1:0} ms/张)", "速度: {0:0.0} 枚/秒 (平均 {1:0} ms/枚)", "Tốc độ: {0:0.0} ảnh/giây (avg {1:0} ms/ảnh)", "गति: {0:0.0} इमेज/से (औसत {1:0} ms/इमेज)" } }
            ,{ "BatchCompleteFmt", new[] { "자동 사이클 완료: {0}장, {1:0.00}s, {2:0.0} 장/초 (avg {3:0} ms/장)", "Auto cycle done: {0} img, {1:0.00}s, {2:0.0} img/s (avg {3:0} ms/img)", "自动循环完成: {0} 张, {1:0.00}s, {2:0.0} 张/秒", "自動サイクル完了: {0}枚, {1:0.00}s, {2:0.0} 枚/秒", "Hoàn tất chu kỳ: {0} ảnh, {1:0.00}s, {2:0.0} ảnh/giây", "साइकिल पूर्ण: {0} इमेज, {1:0.00}s, {2:0.0} इमेज/से" } }
            ,{ "StartupErrorTitle", new[] { "Inspection Program - 시작 오류", "Inspection Program - Startup Error", "Inspection Program - 启动错误", "Inspection Program - 起動エラー", "Inspection Program - Lỗi khởi động", "Inspection Program - स्टार्टअप त्रुटि" } }
            ,{ "OvClrShort", new[] { "OvClr", "OvClr", "清除", "OvClr", "OvClr", "OvClr" } }
            ,{ "NccPixelFilterLogFmt", new[] { " 픽셀 필터: kept={0}/{1}, range=[{2},{3}]", " pixel filter: kept={0}/{1}, range=[{2},{3}]", " 像素过滤: kept={0}/{1}, range=[{2},{3}]", "画素フィルタ: kept={0}/{1}, range=[{2},{3}]", "Lọc pixel : kept={0}/{1}, range=[{2},{3}]", " पिक्सेल फ़िल्टर: kept={0}/{1}, range=[{2},{3}]" } }
            ,{ "MatchRateDash", new[] { "일치율 = —", "Match = —", "匹配率 = —", "一致率 = —", "Khớp = —", "मैच = —" } }
            ,{ "MatchRateFmt", new[] { "일치율 = {0:F2}%", "Match = {0:F2}%", "匹配率 = {0:F2}%", "一致率 = {0:F2}%", "Khớp = {0:F2}%", "मैच = {0:F2}%" } }
            ,{ "CountEqualsFmt", new[] { "개수 = {0}", "Count = {0}", "个数 = {0}", "個数 = {0}", "Số = {0}", "गिनती = {0}" } }
            ,{ "NccCountLogPrefix", new[] { "카운트  ", "count  ", "计数  ", "カウント  ", "Đếm  ", " गिनती  " } }
            ,{ "BlobDetectLogFmt", new[] { "볼 검출(Blob): threshold>={0}, MinArea>={1}, Count={2}, Expected={3} => {4}", "Blob detect: threshold>={0}, MinArea>={1}, Count={2}, Expected={3} => {4}", "Blob 检测: threshold>={0}, MinArea>={1}, Count={2}, Expected={3} => {4}", "Blob検出: threshold>={0}, MinArea>={1}, Count={2}, Expected={3} => {4}", "Phát hiện blob: threshold>={0}, MinArea>={1}, Count={2}, Expected={3} => {4}", "ब्लॉब: threshold>={0}, MinArea>={1}, Count={2}, Expected={3} => {4}" } }
            ,{ "RoiFgCountLogFmt", new[] { "ROI 전경 픽셀(mask): count={0}, range=[{1},{2}]", "ROI foreground (mask): count={0}, range=[{1},{2}]", "ROI 前景(mask): count={0}, range=[{1},{2}]", "ROI前景(mask): count={0}, range=[{1},{2}]", "ROI foreground (mask): count={0}, range=[{1},{2}]", "ROI अग्रभूमि (mask): count={0}, range=[{1},{2}]" } }
            ,{ "RoiFgRejectFmt", new[] { "ROI 전경 픽셀 수 범위 벗어남: count={0}, range=[{1},{2}]", "ROI foreground count out of range: count={0}, range=[{1},{2}]", "ROI 前景像素数超出范围: count={0}, range=[{1},{2}]", "ROI前景画素が範囲外: count={0}, range=[{1},{2}]", "Số pixel ROI ngoài phạm vi: count={0}, range=[{1},{2}]", "ROI पिक्सेल सीमा के बाहर: count={0}, range=[{1},{2}]" } }
            ,{ "RoiFgCalcFailAppend", new[] { "ROI 전경 픽셀(mask) 계산 실패: ", "ROI foreground (mask) calc failed: ", "ROI 前景(mask)计算失败: ", "ROI前景(mask)計算失敗: ", "Lỗi tính ROI foreground (mask): ", "ROI अग्रभूमि गणना विफल: " } }
            ,{ "CameraTabTooltipFmt", new[] {
                "Live에 쓰는 PC 카메라 번호 = {0} (첫 탭이 0번)",
                "Live uses PC camera index {0} (first tab = 0)",
                "Live 使用摄像头索引 {0}（第一个标签为 0）",
                "Live はカメラ番号 {0}（先頭タブが0）",
                "Live dùng chỉ số camera {0} (tab đầu = 0)",
                "Live कैमरा इंडेक्स {0} (पहला टैब = 0)" } }
            ,{ "CameraIndexScanMenu", new[] {
                "카메라 번호 검사 (로그)",
                "Probe camera indices (log)",
                "检测摄像头编号（日志）",
                "カメラ番号を確認（ログ）",
                "Kiểm tra số camera (log)",
                "कैमरा इंडेक्स जाँच (लॉग)" } }
            ,{ "CameraIndexScanRunning", new[] {
                "카메라 번호 검사 중… (잠시만 기다리세요)",
                "Probing camera indices…",
                "正在检测摄像头编号…",
                "カメラ番号を確認中…",
                "Đang kiểm tra camera…",
                "कैमरा जाँच चल रही है…" } }
            ,{ "CameraIndexScanNeedStopLive", new[] {
                "Live를 끈 뒤에 카메라 번호 검사를 실행하세요.",
                "Stop Live before probing camera indices.",
                "请先停止 Live 再检测摄像头编号。",
                "番号確認の前に Live を停止してください。",
                "Hãy tắt Live trước khi kiểm tra số camera.",
                "पहले Live बंद करें, फिर इंडेक्स जाँचें।" } }
            ,{ "LiveDeviceAutoFmt", new[] {
                "Live: 장치 {0}에서 영상 수신 (자동 선택)",
                "Live: video from device {0} (auto-selected)",
                "Live: 正在从设备 {0} 接收画面（自动选择）",
                "Live: デバイス {0} から映像（自動選択）",
                "Live: hình ảnh từ thiết bị {0} (tự chọn)",
                "Live: डिवाइस {0} से वीडियो (ऑटो)" } }
            ,{ "LiveNoFrameHint", new[] {
                "장치 연결 후 20초 안에 뷰어로 프레임이 오지 않았습니다. USB·다른 앱 점유·카메라 권한을 확인하거나, 하단 카메라 줄 우클릭 → 번호 검사를 실행하세요.",
                "No frame reached the viewer within 20s after the device opened. Check USB, other apps using the camera, Windows permissions, or right‑click the camera bar → Probe camera indices.",
                "设备连接后 20 秒内查看器仍无画面。请检查 USB、其他应用占用、系统权限，或在相机栏右键→检测编号。",
                "デバイス接続後 20 秒以内にビューへフレームがありません。USB・他アプリ・権限を確認するか、下のカメラ行を右クリック→番号確認。",
                "Sau khi thiết bị mở, 20s vẫn không có khung hình trên viewer. Kiểm tra USB, app khác, quyền, hoặc chuột phải thanh camera → kiểm tra số.",
                "डिवाइस खुलने के बाद 20s में viewer पर कोई फ्रेम नहीं। USB/अन्य ऐप/अनुमति जाँचें, या कैमरा बार→इंडेक्स।" } }
         };

        public static string GetText(string key, LanguageType language)
        {
            if (!TextMap.ContainsKey(key))
                return key;

            int index = (int)language;
            string[] values = TextMap[key];
            if (index < 0 || index >= values.Length)
                return values[0];

            return values[index];
        }
    }
}
