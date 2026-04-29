using System;
using System.Drawing;
using System.Windows.Forms;
using ImageViewerWinForms;
using InspectionProgram.Common;

namespace InspectionProgram.View
{
    public partial class ViewerView : UserControl
    {
        private string _viewerTitle = "CAM 01";
        private SharedCameraView _cameraView = null;
        private readonly ImageViewPanelView1 _imageViewerPanel;

        public event EventHandler ViewerSelected;
        public event EventHandler LoadClicked;
        public event EventHandler ClearClicked;
        public event EventHandler FitClicked;

        public ViewerView()
        {
            InitializeComponent();

            try
            {
                BackColor = AppColors.Surface;
                ForeColor = AppColors.Foreground;
                Font = AppFontHelper.Create(9F);
                Padding = new Padding(0);
                Margin = new Padding(0);

                _imageViewerPanel = new ImageViewPanelView1();
                _imageViewerPanel.Dock = DockStyle.Fill;
                _imageViewerPanel.Margin = new Padding(0);
                _imageViewerPanel.ViewTitle = _viewerTitle;
                _imageViewerPanel.ViewSelected += ImageViewerPanel_ViewSelected;
                _imageViewerPanel.FileLoadRequested += ImageViewerPanel_FileLoadRequested;
                _imageViewerPanel.ClearDisplayRequested += ImageViewerPanel_ClearDisplayRequested;
                _imageViewerPanel.SetBuiltInToolbarVisible(true);
                Controls.Add(_imageViewerPanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ViewerView.ViewerView() : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string ViewerTitle
        {
            get { return _viewerTitle; }
            set
            {
                try
                {
                    _viewerTitle = string.IsNullOrWhiteSpace(value) ? "CAM" : value;
                    if (_imageViewerPanel != null)
                        _imageViewerPanel.ViewTitle = _viewerTitle;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ViewerView.ViewerTitle(set) : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public SharedCameraView CameraView
        {
            get { return _cameraView; }
        }

        public Bitmap DisplayImage
        {
            get
            {
                try
                {
                    if (_cameraView == null)
                        return null;

                    return _cameraView.DisplayImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ViewerView.DisplayImage(get) : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }

        public void BindCameraView(SharedCameraView cameraView)
        {
            try
            {
                if (_cameraView != null)
                    _cameraView.DisplayImageChanged -= CameraView_DisplayImageChanged;

                _cameraView = cameraView;

                if (_cameraView != null)
                    _cameraView.DisplayImageChanged += CameraView_DisplayImageChanged;

                RefreshFromCameraView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ViewerView.BindCameraView() : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ApplyLanguage(LanguageType language)
        {
            try
            {
                // Test_ImageViewer 기반 View는 아이콘 툴바를 사용하므로 언어별 텍스트 적용은 생략합니다.
            }
            catch (Exception ex)
            {
                MessageBox.Show("ViewerView.ApplyLanguage() : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SetStatus(string text)
        {
            try
            {
                // Test_ImageViewer 기반 View는 내부 상태바가 없으므로 확장 포인트만 유지합니다.
            }
            catch (Exception ex)
            {
                MessageBox.Show("ViewerView.SetStatus() : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SetSelected(bool isSelected)
        {
            try
            {
                if (_imageViewerPanel != null)
                    _imageViewerPanel.IsTargetView = isSelected;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ViewerView.SetSelected() : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void FitImage()
        {
            try
            {
                if (_imageViewerPanel != null && _imageViewerPanel.HasImage)
                    _imageViewerPanel.ZoomFit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ViewerView.FitImage() : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshFromCameraView()
        {
            Bitmap clone = null;
            string imagePath = string.Empty;

            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(RefreshFromCameraView));
                    return;
                }

                if (_cameraView != null)
                {
                    clone = _cameraView.CreateDisplayImageClone();
                    imagePath = _cameraView.ImagePath;
                }

                if (clone != null)
                {
                    _imageViewerPanel.CanvasControl.SetImage(clone, imagePath);
                    _imageViewerPanel.ZoomFit();
                }
                else
                {
                    _imageViewerPanel.ClearDisplay();
                }
            }
            catch (Exception ex)
            {
                if (clone != null)
                    clone.Dispose();

                MessageBox.Show("ViewerView.RefreshFromCameraView() : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CameraView_DisplayImageChanged(object sender, EventArgs e)
        {
            try
            {
                RefreshFromCameraView();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ViewerView.CameraView_DisplayImageChanged() : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImageViewerPanel_ViewSelected(object sender, EventArgs e)
        {
            try
            {
                if (ViewerSelected != null)
                    ViewerSelected(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ViewerView.ImageViewerPanel_ViewSelected() : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImageViewerPanel_FileLoadRequested(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff|All Files|*.*";
                    dialog.Multiselect = false;

                    if (dialog.ShowDialog(this) != DialogResult.OK)
                        return;

                    if (_cameraView != null)
                        _cameraView.LoadImageFromFile(dialog.FileName);

                    // patternMatching 추가
                    CurrentImagePath = dialog.FileName;
                }

                if (LoadClicked != null)
                    LoadClicked(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ViewerView.ImageViewerPanel_FileLoadRequested() : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImageViewerPanel_ClearDisplayRequested(object sender, EventArgs e)
        {
            try
            {
                if (_cameraView != null)
                    _cameraView.ClearDisplayImage();
                else if (_imageViewerPanel != null)
                    _imageViewerPanel.ClearDisplay();

                if (ClearClicked != null)
                    ClearClicked(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ViewerView.ImageViewerPanel_ClearDisplayRequested() : " + ex.Message, "Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public string CurrentImagePath { get; private set; } = string.Empty;

        internal void DrawString(Point point, string text, Color color, int fontSize)
        {
            DrawString(new PointF(point.X, point.Y), text, color, fontSize);
        }

        internal void DrawString(PointF point, string text, Color color, int fontSize)
        {
            if (_imageViewerPanel?.CanvasControl == null)
                return;

            _imageViewerPanel.CanvasControl.DrawString(point, text, color, fontSize);
        }

        internal void ClearDrawStrings()
        {
            if (_imageViewerPanel?.CanvasControl == null)
                return;

            _imageViewerPanel.CanvasControl.ClearDrawOverlays();
        }
    }
}
