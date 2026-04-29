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
            { "NccPatternMatching", new[] { "NCC 패턴 매칭", "NCC Pattern Matching", "NCC 模板匹配", "NCC パターンマッチング", "So khớp mẫu NCC", "NCC पैटर्न मिलान" } },
            { "NccModelSave", new[] { "NCC 모델 저장", "Save NCC Model", "保存 NCC 模型", "NCC モデル保存", "Lưu mô hình NCC", "NCC मॉडल सहेजें" } },
            { "NccInspect", new[] { "NCC 검사", "NCC Inspect", "NCC 检测", "NCC 検査", "Kiểm tra NCC", "NCC निरीक्षण" } },
            { "NccMinScore", new[] { "판정 최소 점수 (NCC)", "Min score (NCC)", "最小判定分数（NCC）", "判定最小スコア（NCC）", "Điểm tối thiểu (NCC)", "न्यूनतम स्कोर (NCC)" } },
            { "NccCount", new[] { "카운트", "Count", "计数", "カウント", "Đếm", "गिनती" } },
            { "NccCountMin", new[] { "카운트 Min (NCC 점수)", "Count Min (NCC score)", "计数最小值（NCC 分数）", "カウント最小（NCC スコア）", "Đếm Min (điểm NCC)", "गिनती न्यूनतम (NCC स्कोर)" } },
            { "NccCountMax", new[] { "카운트 Max (NCC 점수)", "Count Max (NCC score)", "计数最大值（NCC 分数）", "カウント最大（NCC スコア）", "Đếm Max (điểm NCC)", "गिनती अधिकतम (NCC स्कोर)" } },
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
            { "AvgNccScore", new[] { "Match NCC (일치율)", "Match NCC (match rate)", "Match NCC（匹配率）", "Match NCC（一致率）", "Match NCC (khớp)", "Match NCC (मैच)" } },
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
            ,{ "ZoomMode", new[] { "줌", "Zoom", "缩放", "ズーム", "Thu phóng", "ज़ूम" } }
            ,{ "ZoomIn", new[] { "확대", "Zoom In", "放大", "拡大", "Phóng to", "ज़ूम इन" } }
            ,{ "ZoomOut", new[] { "축소", "Zoom Out", "缩小", "縮小", "Thu nhỏ", "ज़ूम आउट" } }
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
            ,{ "RunInspectStep", new[] { "3) 검사 실행", "3) Run inspect", "3) 运行检测", "3) 検査実行", "3) Chạy kiểm tra", "3) निरीक्षण चलाएँ" } }
            ,{ "StopShort", new[] { "중지", "Stop", "停止", "停止", "Dừng", "रोकें" } }
            ,{ "SaveCsvStep", new[] { "4) CSV 저장", "4) Save CSV", "4) 保存CSV", "4) CSV保存", "4) Lưu CSV", "4) CSV सहेजें" } }
            ,{ "SystemReady", new[] { "시스템 준비됨.", "System ready.", "系统就绪。", "システム準備完了。", "Hệ thống sẵn sàng.", "सिस्टम तैयार है।" } }
            ,{ "InspectionUiFlow", new[] {
                "검사 UI 흐름\r\n1) Load — [예] 폴더 / [아니오] 이미지 파일, ROI+ (선택)\r\n2) Threshold — 밝기 하한(이진화) 미리보기\r\n3) Run — 티칭 «검사 레시피» 자동 적용 후 검사(NCC 있으면 전역 패턴 카운트). 자동 사이클 시 목록 전체\r\n4) CSV 저장 — 세션 검사 결과(OK/NG) 파일 쓰기\r\n상단 «로그» — Inspection Log 내용을 .txt 로 저장\r\n※ 티칭에서 «검사 레시피 저장» 후 — «3) Run» 또는 상단 «개수»로 동일 검사",
                "Inspection UI flow\r\n1) Load - [Yes] folder / [No] image file, ROI+ (optional)\r\n2) Threshold - brightness cutoff preview\r\n3) Run - apply Teaching recipe then inspect (global NCC count if model exists). Auto cycle runs all images\r\n4) Save CSV - write session results (OK/NG)\r\nTop \"Log\" saves Inspection Log to .txt\r\n* After saving recipe in Teaching, run same inspection via \"3) Run\" or top \"Count\"",
                "检测UI流程\r\n1) Load - [是] 文件夹 / [否] 单张图片，ROI+（可选）\r\n2) Threshold - 亮度阈值预览\r\n3) Run - 先应用Teaching配方再检测（有NCC模型时全图计数）\r\n4) 保存CSV - 保存会话结果（OK/NG）\r\n顶部“日志”可将Inspection Log保存为.txt\r\n* Teaching保存配方后，可通过“3) Run”或顶部“Count”执行同样检测",
                "検査UIフロー\r\n1) Load - [はい] フォルダ / [いいえ] 画像ファイル、ROI+（任意）\r\n2) Threshold - 輝度しきい値プレビュー\r\n3) Run - Teachingレシピ適用後に検査（NCCモデルがあれば全体カウント）\r\n4) CSV保存 - セッション結果（OK/NG）を保存\r\n上部「ログ」でInspection Logを.txt保存\r\n* Teachingでレシピ保存後は「3) Run」または上部「Count」で同一検査",
                "Luồng UI kiểm tra\r\n1) Load - [Có] thư mục / [Không] ảnh đơn, ROI+ (tùy chọn)\r\n2) Threshold - xem trước ngưỡng sáng\r\n3) Run - áp dụng recipe từ Teaching rồi kiểm tra (nếu có mô hình NCC sẽ đếm toàn ảnh)\r\n4) Lưu CSV - lưu kết quả phiên (OK/NG)\r\n\"Log\" phía trên lưu Inspection Log ra .txt\r\n* Sau khi lưu recipe ở Teaching, chạy lại bằng \"3) Run\" hoặc \"Count\"",
                "इंस्पेक्शन UI फ्लो\r\n1) Load - [हाँ] फ़ोल्डर / [नहीं] इमेज फ़ाइल, ROI+ (वैकल्पिक)\r\n2) Threshold - ब्राइटनेस कटऑफ प्रीव्यू\r\n3) Run - Teaching रेसिपी लागू करके निरीक्षण (NCC मॉडल हो तो ग्लोबल काउंट)\r\n4) CSV सहेजें - सेशन परिणाम (OK/NG) सहेजें\r\nऊपर \"Log\" से Inspection Log को .txt में सेव करें\r\n* Teaching में रेसिपी सेव के बाद \"3) Run\" या ऊपर \"Count\" से वही निरीक्षण चलाएँ" } }
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
