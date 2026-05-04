using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using InspectionProgram.Common;
using InspectionProgram.View;

namespace InspectionProgram.GUI
{
    /// <summary>
    /// 메인 창. 상단 레이아웃은 MainForm.Designer.cs 안의 tlpHeader / flpHeader* 로 구역이 나뉘어 있어
    /// Visual Studio 폼 디자이너에서 각 구역을 바로 열어 디버깅하기 쉽습니다.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly Timer _clockTimer = new Timer();
        // UI shell pages (기능은 추후 단계에서 붙임)
        private readonly UcAutoRunShell _autoRunPage = new UcAutoRunShell();
        private readonly UcTeachingShell _teachingPage = new UcTeachingShell();
        public SharedCameraView[] SharedCameras = new SharedCameraView[10];

        private VisionModeType _currentMode = VisionModeType.AutoRun;
        private LanguageType _currentLanguage = LanguageType.Kr;
        private bool _networkConnected = false;
        private readonly Dictionary<Button, bool> _toolbarMenuLatched = new Dictionary<Button, bool>();

        public MainForm()
        {
            InitializeComponent();
            ApplyTheme();
            InitializePages();
            InitializeSharedViews();
            InitializeTimer();
            FormClosing += FrmVisionMain_FormClosing;
            FormClosed += FrmVisionMain_FormClosed;
        }

        private void FrmVisionMain_Load(object sender, EventArgs e)
        {
            try
            {
                AppLogger.Initialize(AppDomain.CurrentDomain.BaseDirectory);
                InspectionResultLogPaths.EnsureDirectoryExists();

                cboLanguage.SelectedIndex = 0;
                UpdateVersionText();
                UpdateTimeText();
                ChangeMode(VisionModeType.AutoRun);
                ApplyCameraCount();
                ApplyLanguage();

                // UI shell 단계: 각 UserControl이 자체 더미 데이터로 화면을 채웁니다.
                AppLogger.Write("FORM", "FrmVisionMain loaded.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain_Load() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializePages()
        {
            try
            {
                _autoRunPage.Dock = DockStyle.Fill;
                _teachingPage.Dock = DockStyle.Fill;

                pnlContentHost.Controls.Add(_autoRunPage);
                pnlContentHost.Controls.Add(_teachingPage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.InitializePages() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeSharedViews()
        {
            try
            {
                for (int i = 0; i < SharedCameras.Length; i++)
                {
                    if (SharedCameras[i] == null)
                        SharedCameras[i] = new SharedCameraView(i);
                }
                // UI shell 단계: 카메라 뷰 바인딩은 추후 구현 단계에서 연결합니다.
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.InitializeSharedViews() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeTimer()
        {
            try
            {
                _clockTimer.Interval = 1000;
                _clockTimer.Tick += ClockTimer_Tick;
                _clockTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.InitializeTimer() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Button[] GetToolbarButtons()
        {
            return new[] { btnMenu1, btnMenu2, btnMenu3, btnMenu4, btnMenu5 };
        }

        private void ApplyTheme()
        {
            try
            {
                BackColor = AppColors.Background;
                ForeColor = AppColors.Foreground;
                Font = AppFontHelper.Create(9F);

                pnlHeader.BackColor = AppColors.HeaderBar;
                pnlToolbar.BackColor = AppColors.SurfaceDark;
                pnlContentHost.BackColor = AppColors.Background;

                ApplyModeButton(btnModeAutoRun);
                ApplyModeButton(btnModeTeaching);
                ApplyModeButton(btnModeOption);

                foreach (Button button in GetToolbarButtons())
                    ApplyToolbarButton(button);
                UpdateToolbarPermissions();

                lblTimeValue.ForeColor = AppColors.Foreground;

                cboLanguage.BackColor = AppColors.SurfaceDark;
                cboLanguage.ForeColor = AppColors.Foreground;
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.ApplyTheme() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyModeButton(Button button)
        {
            button.BackColor = AppColors.SurfaceDark;
            button.ForeColor = AppColors.Foreground;
            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = AppColors.Border;
            button.Font = AppFontHelper.CreateBold(10F);
        }

        private static void SetModeTabLook(Button button, bool selected)
        {
            button.BackColor = selected ? AppColors.Accent : AppColors.SurfaceDark;
            button.ForeColor = selected ? AppColors.AccentForeground : AppColors.Foreground;
        }

        private void RefreshToolbarButtonAppearance(Button button)
        {
            if (button == null || !button.Visible)
                return;

            string actionKey = button.Tag as string;
            if (string.IsNullOrWhiteSpace(actionKey))
                return;

            button.Enabled = true;
            button.UseVisualStyleBackColor = false;
            button.Cursor = Cursors.Default;
            bool latched = _toolbarMenuLatched.TryGetValue(button, out bool on) && on;
            button.ForeColor = Color.White;
            button.BackColor = latched ? AppColors.ModeToggleHighlight : AppColors.Surface;
        }

        private void ApplyHeaderButton(Button button)
        {
            button.BackColor = AppColors.SurfaceLight;
            button.ForeColor = AppColors.Foreground;
            button.UseVisualStyleBackColor = false;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = AppColors.Border;
            button.Font = AppFontHelper.Create(8.5F);
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
        }

        private void ApplyToolbarButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = AppColors.Border;
            button.Font = AppFontHelper.Create(16.5F);
            button.Padding = new Padding(10, 4, 10, 4);
            button.TextImageRelation = TextImageRelation.ImageBeforeText;
            button.ImageAlign = ContentAlignment.MiddleLeft;
            button.TextAlign = ContentAlignment.MiddleLeft;
            button.UseVisualStyleBackColor = false;
            button.ForeColor = Color.White;
            button.BackColor = AppColors.Surface;
        }


        private void UpdateVersionText()
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.UpdateVersionText() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTimeText()
        {
            try
            {
                lblTimeValue.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.UpdateTimeText() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyLanguage()
        {
            try
            {
                Text = LocalizationService.GetText("AppWindowTitle", _currentLanguage);
                btnModeAutoRun.Text = LocalizationService.GetText("AutoRun", _currentLanguage);
                btnModeTeaching.Text = LocalizationService.GetText("Teaching", _currentLanguage);
                btnModeOption.Text = LocalizationService.GetText("Option", _currentLanguage);

                // 페이지 내부 언어도 함께 갱신
                _autoRunPage?.ApplyLanguage(_currentLanguage);
                _teachingPage?.ApplyLanguage(_currentLanguage);

                UpdateVersionText();
                UpdateTimeText();
                UpdateToolbarPermissions();
                ConfigureToolbarForMode(_currentMode);
                SetModeTabLook(btnModeAutoRun, _currentMode == VisionModeType.AutoRun);
                SetModeTabLook(btnModeTeaching, _currentMode == VisionModeType.Teaching);
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.ApplyLanguage() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleToolbarMenuLatch(Button button)
        {
            if (button == null || !button.Visible)
                return;
            string actionKey = button.Tag as string;
            if (string.IsNullOrWhiteSpace(actionKey))
                return;

            // Radio-style latch: only one menu stays highlighted.
            // Clicking a menu selects it; previously selected menus are reset.
            _toolbarMenuLatched.Clear();
            _toolbarMenuLatched[button] = true;

            foreach (Button b in GetToolbarButtons())
                RefreshToolbarButtonAppearance(b);
        }

        private void ChangeMode(VisionModeType mode)
        {
            try
            {
                if (mode == VisionModeType.Option)
                    mode = VisionModeType.AutoRun;

                _currentMode = mode;
                _autoRunPage.Visible = mode == VisionModeType.AutoRun;
                _teachingPage.Visible = mode == VisionModeType.Teaching;

                SetModeTabLook(btnModeAutoRun, mode == VisionModeType.AutoRun);
                SetModeTabLook(btnModeTeaching, mode == VisionModeType.Teaching);

                ConfigureToolbarForMode(mode);
                AppLogger.Write("MODE", mode.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.ChangeMode() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureToolbarForMode(VisionModeType mode)
        {
            try
            {
                _toolbarMenuLatched.Clear();

                if (mode == VisionModeType.AutoRun)
                {
                    // 상단 «실행(Run)»는 기능 미연결 상태였음 → 제거하고 검사/개수/로그만 표시
                    SetToolbarButton(btnMenu1, "Inspection");
                    SetToolbarButton(btnMenu2, "Count");
                    SetToolbarButton(btnMenu3, "Log");
                    SetToolbarButton(btnMenu4, string.Empty);
                    SetToolbarButton(btnMenu5, string.Empty);
                }
                else if (mode == VisionModeType.Teaching)
                {
                    SetToolbarButton(btnMenu1, "Live");
                    SetToolbarButton(btnMenu2, "Snap");
                    SetToolbarButton(btnMenu3, string.Empty);
                    SetToolbarButton(btnMenu4, string.Empty);
                    SetToolbarButton(btnMenu5, string.Empty);
                }
                else
                {
                    SetToolbarButton(btnMenu1, string.Empty);
                    SetToolbarButton(btnMenu2, string.Empty);
                    SetToolbarButton(btnMenu3, string.Empty);
                    SetToolbarButton(btnMenu4, string.Empty);
                    SetToolbarButton(btnMenu5, string.Empty);
                }

                UpdateToolbarPermissions();
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.ConfigureToolbarForMode() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetToolbarButton(Button button, string actionKey)
        {
            try
            {
                button.Tag = actionKey;
                button.Visible = !string.IsNullOrWhiteSpace(actionKey);
                button.Enabled = button.Visible;
                if (!button.Visible)
                {
                    _toolbarMenuLatched.Remove(button);
                    return;
                }

                button.Text = LocalizationService.GetText(actionKey, _currentLanguage);
                button.Image = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.SetToolbarButton() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateToolbarPermissions()
        {
            try
            {
                foreach (Button button in GetToolbarButtons())
                    RefreshToolbarButtonAppearance(button);
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.UpdateToolbarPermissions() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyCameraCount()
        {
            try
            {
                //
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.ApplyCameraCount() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SetCameraImage(int cameraIndex, Bitmap frame, string filePath = "")
        {
            try
            {
                if (cameraIndex < 0 || cameraIndex >= SharedCameras.Length)
                    return;

                if (SharedCameras[cameraIndex] == null)
                    return;

                SharedCameras[cameraIndex].SetDisplayImage(frame, filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.SetCameraImage() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ClearCameraImage(int cameraIndex)
        {
            try
            {
                if (cameraIndex < 0 || cameraIndex >= SharedCameras.Length)
                    return;

                if (SharedCameras[cameraIndex] == null)
                    return;

                SharedCameras[cameraIndex].ClearDisplayImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.ClearCameraImage() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteToolbarAction(string actionKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(actionKey))
                    return;

                string logText = DateTime.Now.ToString("HH:mm:ss") + " " + actionKey + " clicked.";
                AppLogger.Write("ACTION", logText);

                if (actionKey == "Live")
                {
                    if (_currentMode != VisionModeType.Teaching)
                        return;
                    _teachingPage.ToggleLiveFromMenu();
                }
                else if (actionKey == "Snap")
                {
                    if (_currentMode != VisionModeType.Teaching)
                        return;
                    _teachingPage.SnapFromMenu();
                }
                else if (actionKey == "DeviceSettings")
                {
                    return;
                }
                else if (actionKey == "Help")
                {
                    MessageBox.Show(
                        LocalizationService.GetText("HelpDockingSample", _currentLanguage),
                        Text,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else if (actionKey == "Inspection")
                {
                    if (_currentMode != VisionModeType.AutoRun)
                        return;
                    _autoRunPage.RunInspectFromMainToolbarCountMenu();
                }
                else if (actionKey == "Count")
                {
                    if (_currentMode != VisionModeType.AutoRun)
                        return;
                    _autoRunPage.RunInspectFromMainToolbarCountMenu();
                }
                else if (actionKey == "Log")
                {
                    if (_currentMode != VisionModeType.AutoRun)
                        return;
                    _autoRunPage.SaveInspectionLogToTextFileWithDialog();
                }

                AppLogger.Write("ACTION", actionKey);
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.ExecuteToolbarAction() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmVisionMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                try
                {
                    AppLogger.Write(
                        "FORM",
                        "MainForm closing. Reason=" + e.CloseReason + " Cancel=" + e.Cancel.ToString() + "\r\n" + Environment.StackTrace);
                }
                catch
                {
                }

                for (int i = 0; i < SharedCameras.Length; i++)
                {
                    if (SharedCameras[i] != null)
                    {
                        SharedCameras[i].Dispose();
                        SharedCameras[i] = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain_FormClosing() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmVisionMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                AppLogger.Write("FORM", "MainForm closed. Reason=" + e.CloseReason);
            }
            catch
            {
            }
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                UpdateTimeText();
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.ClockTimer_Tick() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModeAutoRun_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeMode(VisionModeType.AutoRun);
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.btnModeAutoRun_Click() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModeTeaching_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeMode(VisionModeType.Teaching);
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.btnModeTeaching_Click() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModeOption_Click(object sender, EventArgs e)
        {
            try
            {
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.btnModeOption_Click() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnApplyCameraCount_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyCameraCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.btnApplyCameraCount_Click() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!cboLanguage.Enabled && cboLanguage.Focused)
                    return;

                if (cboLanguage.SelectedIndex < 0)
                    return;

                _currentLanguage = (LanguageType)cboLanguage.SelectedIndex;
                ApplyLanguage();
                AppLogger.Write("LANG", _currentLanguage.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.cboLanguage_SelectedIndexChanged() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void btnToolbar_Click(object sender, EventArgs e)
        {
            try
            {
                Button button = sender as Button;
                if (button == null)
                    return;

                string actionKey = button.Tag as string;
                if (string.IsNullOrWhiteSpace(actionKey))
                    return;
                ToggleToolbarMenuLatch(button);
                ExecuteToolbarAction(actionKey);
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmVisionMain.btnToolbar_Click() : " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
