using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using InspectionProgram.Common;
using Cv2 = OpenCvSharp.Cv2;
using CvMat = OpenCvSharp.Mat;
using CvRect = OpenCvSharp.Rect;
using CvScalar = OpenCvSharp.Scalar;
using CvType = OpenCvSharp.MatType;

namespace ImageViewerWinForms
{
    public class ImageCanvasControl : Control
    {
        public sealed class ImageSyncState
        {
            public float Zoom;
            public float CenterRatioX;
            public float CenterRatioY;
            public bool ZoomModeEnabled;
        }

        private enum OverlayType
        {
            Text,
            Rectangle,
            Line,
            Point,
            Circle,
            Polygon
        }

        private sealed class OverlayItem
        {
            public OverlayType Type;
            public string Text;
            public string PointType;
            public PointF Point1;
            public PointF Point2;
            public RectangleF Rect;
            public PointF[] Points;
            public Circle Circle;
            public Color Color;
            public float LineWidth;
            public float FontSize;
        }

        private readonly List<OverlayItem> _overlays = new List<OverlayItem>();
        private int _overlayUpdateDepth;
        private readonly List<ROI_Manager> _rois = new List<ROI_Manager>();
        private readonly Timer _rightHoldTimer;

        private Bitmap _image;
        private Bitmap _miniMapCache;
        private Size _miniMapCacheSize = Size.Empty;
        private int _miniMapCacheVersion = -1;
        private int _imageVersion;

        private string _imagePath = string.Empty;
        private string _viewTitle = "Display View";
        private float _zoom = 1.0f;
        private PointF _offset = PointF.Empty;
        private bool _showCenterCross;
        /// <summary>십자가가 지나는 픽셀(열, 행) 인덱스. 화살표 키당 3픽셀 이동.</summary>
        private int _crossPixelX;
        private int _crossPixelY;
        private bool _showMiniMap = true;
        private bool _showPixelGrayValue;
        private bool _avgModeEnabled;
        private bool _zoomModeEnabled;
        private bool _isTargetSelected;
        private bool _isPanning;
        private Point _panStartPoint;
        private PointF _panStartOffset;
        private bool _leftButtonDown;
        private Point _leftDownPoint;
        private bool _rightButtonDown;
        private bool _contextMenuOpenedByHold;
        private Point _rightDownPoint;
        private bool _isMouseOverImage;
        private Point _currentMouseClient = Point.Empty;
        private Point _currentMouseImage = Point.Empty;
        private string _currentMouseGrayText = string.Empty;
        private bool _isAvgSelecting;
        private Point _avgSelectionStartClient;
        private Point _avgSelectionCurrentClient;
        private RectangleF _avgSelectionImageRect = RectangleF.Empty;
        private string _avgGrayText = string.Empty;
        private byte[] _grayBuffer;
        private int _grayBufferWidth;
        private int _grayBufferHeight;
        private string _statusMessage = string.Empty;
        private DateTime _statusMessageAt = DateTime.MinValue;
        private bool _suspendSyncStateChanged;
        private ROI_Manager _selectedRoi;
        private ROI_Manager _dragRoi;
        private int _dragRoiHandleIndex = -1;
        private bool _isRoiMoving;
        private bool _isRoiResizing;
        private Point _roiLastImagePoint = Point.Empty;

        private bool _blobShowBright;
        private int _blobBrightMin;
        private int _blobBrightMax = 255;
        private int _blobAreaMin;
        private int _blobAreaMax = int.MaxValue;
        private double _blobDiameterMin;
        private double _blobDiameterMax = double.MaxValue;
        private int _blobOpening;
        private bool _blobRoiTouchRemove;
        private readonly List<Rectangle> _blobResultRects = new List<Rectangle>();
        private Color _blobResultPenColor = Color.HotPink;
        private Color _blobResultFillColor = Color.FromArgb(130, 255, 105, 180);

        /// <summary>사용자 ROI(티칭 레시피 도형)를 그릴지 여부. 전역 NCC 카운트 중에는 끄면 화면 좌표 고정 ROI 착시를 줄일 수 있습니다.</summary>
        private bool _showUserRoiOverlay = true;

        // Demo preprocessing (for loaded images)
        private bool _preEnabled;
        private int _preBlurRadius;
        private int _preBgRadius;

        public event EventHandler RoiCollectionChanged;
        public event EventHandler SelectedRoiChanged;

        private const float MinZoom = 0.05f;
        private const float MaxZoom = 50.0f;
        private const int GrayValueMaxDrawCount = 30000;

        public event EventHandler ViewStateChanged;
        public event EventHandler ViewSelected;
        public event EventHandler SyncStateChanged;

        public ContextMenuStrip LongPressContextMenuStrip { get; set; }

        public string ViewTitle
        {
            get { return _viewTitle; }
            set
            {
                try
                {
                    _viewTitle = string.IsNullOrWhiteSpace(value) ? "Display View" : value;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        public bool HasImage
        {
            get { return _image != null; }
        }

        /// <summary>
        /// 티칭/레시피에서 불러온 사용자 ROI 도형 표시 여부입니다.
        /// 전역 NCC 카운트 결과만 보려면 false로 두면 Blob 검사 전까지 ROI 윤곽이 그려지지 않습니다.
        /// </summary>
        public bool ShowUserRoiOverlay
        {
            get { return _showUserRoiOverlay; }
            set
            {
                try
                {
                    if (_showUserRoiOverlay == value)
                        return;
                    _showUserRoiOverlay = value;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>새 이미지가 <see cref="SetImage"/>로 설정된 직후 발생합니다.</summary>
        public event EventHandler ImageLoaded;

        public string ImagePath
        {
            get { return _imagePath; }
        }

        public bool ShowCenterCross
        {
            get { return _showCenterCross; }
        }

        public bool ShowMiniMap
        {
            get { return _showMiniMap; }
        }

        public bool ShowPixelGrayValue
        {
            get { return _showPixelGrayValue; }
        }

        public bool AvgModeEnabled
        {
            get { return _avgModeEnabled; }
        }

        public bool ZoomModeEnabled
        {
            get { return _zoomModeEnabled; }
            set
            {
                try
                {
                    SetZoomModeEnabledInternal(value, true, true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        public bool IsTargetSelected
        {
            get { return _isTargetSelected; }
            set
            {
                try
                {
                    _isTargetSelected = value;
                    Invalidate();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        public Size ImagePixelSize
        {
            get
            {
                try
                {
                    return _image == null ? Size.Empty : _image.Size;
                }
                catch
                {
                    return Size.Empty;
                }
            }
        }

        public ROI_Manager SelectedRoi
        {
            get { return _selectedRoi; }
        }

        public IList<ROI_Manager> ROIItems
        {
            get { return _rois.AsReadOnly(); }
        }

        /// <summary>캔버스에 등록된 사용자 ROI(도형) 개수.</summary>
        public int RoiItemCount
        {
            get { return _rois != null ? _rois.Count : 0; }
        }

        /// <summary>마지막 <see cref="FindBlobObjectsInSelectedRoi"/> 이후 검출된 Blob 사각형 개수.</summary>
        public int BlobResultCount
        {
            get { return _blobResultRects != null ? _blobResultRects.Count : 0; }
        }

        /// <summary>현재 Blob 검사에 쓰이는 밝기 하한(<see cref="ShowBlob_Bright"/>/<see cref="SetBlobOptions"/>).</summary>
        public int BlobBrightMinEffective
        {
            get { return _blobBrightMin; }
        }

        /// <summary>현재 Blob 최소 면적(<see cref="SetBlobOptions"/>).</summary>
        public int BlobAreaMinEffective
        {
            get { return _blobAreaMin; }
        }

        public ImageCanvasControl()
        {
            try
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.Selectable, true);

                DoubleBuffered = true;
                BackColor = Color.FromArgb(35, 35, 35);
                ForeColor = Color.White;
                Cursor = Cursors.SizeAll;
                TabStop = true;

                _rightHoldTimer = new Timer();
                _rightHoldTimer.Interval = 2000;
                _rightHoldTimer.Tick += RightHoldTimer_Tick;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #region Public Methods

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_rightHoldTimer != null)
                    {
                        _rightHoldTimer.Stop();
                        _rightHoldTimer.Dispose();
                    }

                    DisposeImage();
                    DisposeMiniMapCache();
                    ClearGrayBuffer();
                }
            }
            catch
            {
            }

            base.Dispose(disposing);
        }

        public void LoadImage(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || File.Exists(filePath) == false)
                    throw new FileNotFoundException("Image file not found.", filePath);

                try
                {
                    AppLogger.Write("IMG", "LoadImage start: " + filePath);
                }
                catch { }

                Bitmap bitmap = LoadBitmapUnlocked(filePath);
                SetImage(bitmap, filePath);
            }
            catch (Exception ex)
            {
                // "절대 죽지 않는" 목표: 여기서는 예외를 던지지 말고 상태 메시지만 표시한다.
                try
                {
                    AppLogger.Write("IMG-EX", "LoadImage failed: " + (filePath ?? "(null)") + "\r\n" + ex);
                }
                catch { }

                SafeClearImageState();
                SetStatusMessage("이미지 로드 실패: " + ex.Message);
            }
        }

        public void SetImage(Bitmap bitmap, string filePath)
        {
            try
            {
                if (bitmap == null)
                    throw new ArgumentNullException("bitmap");

                DisposeImage();
                _image = bitmap;
                _imagePath = filePath ?? string.Empty;
                _statusMessage = string.Empty;
                _zoom = 1.0f;
                _offset = PointF.Empty;
                _showCenterCross = false;
                UpdateCrossDefaultPosition();
                _showMiniMap = true;
                _showPixelGrayValue = false;
                _avgModeEnabled = false;
                _isMouseOverImage = false;
                _currentMouseGrayText = string.Empty;
                _isAvgSelecting = false;
                _avgSelectionImageRect = RectangleF.Empty;
                _avgGrayText = string.Empty;
                _overlays.Clear();
                _rois.Clear();
                _selectedRoi = null;
                _dragRoi = null;
                _dragRoiHandleIndex = -1;
                _isRoiMoving = false;
                _isRoiResizing = false;
                _blobResultRects.Clear();
                SetZoomModeEnabledInternal(false, false, false);
                BuildGrayBuffer();
                BumpImageVersion();
                FitToWindow();
                RaiseViewStateChanged();
                RaiseSelectedRoiChanged();
                RaiseRoiCollectionChanged();
                RaiseSyncStateChanged();
                Invalidate();
                ImageLoaded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                try
                {
                    AppLogger.Write("IMG-EX", "SetImage failed: " + (_imagePath ?? string.Empty) + "\r\n" + ex);
                }
                catch { }

                // bitmap은 호출자가 넘겨준 것이므로, SetImage 실패 시 누수 방지
                try { bitmap.Dispose(); } catch { }
                SafeClearImageState();
                SetStatusMessage("이미지 적용 실패: " + ex.Message);
            }
        }

        /// <summary>
        /// Live 스트리밍처럼 프레임이 자주 바뀌는 경우에 사용합니다.
        /// ROI(검사 영역)와 줌/오프셋은 유지하고, 이미지·그레이 버퍼만 갱신합니다.
        /// </summary>
        public void UpdateImageFrame(Bitmap bitmap)
        {
            try
            {
                if (bitmap == null)
                    return;

                DisposeImage();
                _image = bitmap;
                _imagePath = string.Empty;
                UpdateCrossDefaultPosition();

                // 프레임마다 결과를 유지하면 잔상이 남아 보이므로, 오버레이/Blob 결과만 초기화
                _overlays.Clear();
                _blobResultRects.Clear();
                _isAvgSelecting = false;
                _avgSelectionImageRect = RectangleF.Empty;
                _avgGrayText = string.Empty;

                BuildGrayBuffer();
                BumpImageVersion();
                RaiseViewStateChanged();
                RaiseSyncStateChanged();
                Invalidate();
                ImageLoaded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.UpdateImageFrame() : " + ex.Message, ex);
            }
        }

        public void ClearDisplay()
        {
            try
            {
                DisposeImage();
                _imagePath = string.Empty;
                _zoom = 1.0f;
                _offset = PointF.Empty;
                _showCenterCross = false;
                _showMiniMap = true;
                _showPixelGrayValue = false;
                _avgModeEnabled = false;
                _isMouseOverImage = false;
                _currentMouseGrayText = string.Empty;
                _isAvgSelecting = false;
                _avgSelectionImageRect = RectangleF.Empty;
                _avgGrayText = string.Empty;
                _overlays.Clear();
                _rois.Clear();
                _blobResultRects.Clear();
                SetZoomModeEnabledInternal(false, false, false);
                ClearGrayBuffer();
                BumpImageVersion();
                RaiseViewStateChanged();
                RaiseSyncStateChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ClearDisplay() : " + ex.Message, ex);
            }
        }

        public void ClearOverlay()
        {
            try
            {
                _overlays.Clear();
                _rois.Clear();
                _selectedRoi = null;
                _dragRoi = null;
                _dragRoiHandleIndex = -1;
                _isRoiMoving = false;
                _isRoiResizing = false;
                _blobResultRects.Clear();
                _isAvgSelecting = false;
                _avgSelectionImageRect = RectangleF.Empty;
                _avgGrayText = string.Empty;
                RaiseViewStateChanged();
                RaiseSelectedRoiChanged();
                RaiseRoiCollectionChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ClearOverlay() : " + ex.Message, ex);
            }
        }

        /// <summary>
        /// DrawString / DrawLine 등으로 추가한 오버레이만 제거합니다. ROI·Blob 결과는 유지합니다.
        /// </summary>
        public void ClearDrawOverlays()
        {
            try
            {
                _overlays.Clear();
                RaiseViewStateChanged();
                InvalidateIfNotBatchingOverlays();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ClearDrawOverlays() : " + ex.Message, ex);
            }
        }

        /// <summary>DrawString/DrawRect 등으로 오버레이를 연속으로 넣을 때, 중간 <see cref="Invalidate"/>을 생략하고 마지막에 한 번만 갱신합니다.</summary>
        public void BeginOverlayUpdate()
        {
            _overlayUpdateDepth++;
        }

        public void EndOverlayUpdate()
        {
            if (_overlayUpdateDepth > 0)
                _overlayUpdateDepth--;
            if (_overlayUpdateDepth == 0)
                Invalidate();
        }

        private void InvalidateIfNotBatchingOverlays()
        {
            if (_overlayUpdateDepth == 0)
                Invalidate();
        }

        public void SetBlobOptions(bool showBright, int brightMin, int brightMax, int areaMin, int areaMax, double diaMin, double diaMax, int opening, bool roiTouchRemove)
        {
            try
            {
                _blobShowBright = showBright;
                _blobBrightMin = Math.Max(0, Math.Min(255, brightMin));
                _blobBrightMax = Math.Max(_blobBrightMin, Math.Min(255, brightMax));
                _blobAreaMin = Math.Max(0, areaMin);
                _blobAreaMax = areaMax <= 0 ? int.MaxValue : areaMax;
                _blobDiameterMin = Math.Max(0.0, diaMin);
                _blobDiameterMax = diaMax <= 0 ? double.MaxValue : diaMax;
                _blobOpening = Math.Max(0, opening);
                _blobRoiTouchRemove = roiTouchRemove;
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.SetBlobOptions() : " + ex.Message, ex);
            }
        }

        public void ShowBlob_Bright(bool isShow, int brightMin, int brightMax)
        {
            try
            {
                _blobShowBright = isShow;
                _blobBrightMin = Math.Max(0, Math.Min(255, brightMin));
                _blobBrightMax = Math.Max(_blobBrightMin, Math.Min(255, brightMax));
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ShowBlob_Bright() : " + ex.Message, ex);
            }
        }

        public void RefreshBlobPreview()
        {
            try
            {
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.RefreshBlobPreview() : " + ex.Message, ex);
            }
        }

        public void FindBlobObjectsInSelectedRoi()
        {
            try
            {
                _blobResultRects.Clear();

                ROI_Manager roi = GetBlobTargetRoi();
                if (_image == null || _grayBuffer == null || roi == null)
                {
                    Invalidate();
                    return;
                }

                Rectangle bounds = NormalizeImageRectangle(roi.GetBounds());
                FindBlobConnectedComponents(bounds, (x, y) => IsBlobCandidatePixel(roi, x, y));
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.FindBlobObjectsInSelectedRoi() : " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 선택된 사각형 ROI 내부에서 threshold 구간(밝기 min~max)의 전경 픽셀 수를
        /// OpenCV mask + CountNonZero로 계산합니다.
        /// </summary>
        public bool TryCountForegroundPixelsInSelectedRectRoiUsingMask(int brightMin, int brightMax, out int foregroundCount, out string error)
        {
            foregroundCount = 0;
            error = string.Empty;
            try
            {
                if (_grayBuffer == null || _grayBufferWidth <= 0 || _grayBufferHeight <= 0)
                {
                    error = "이미지 버퍼가 없습니다.";
                    return false;
                }

                ROI_Manager roi = GetBlobTargetRoi();
                ROIRectangle rr = roi as ROIRectangle;
                if (rr == null)
                {
                    error = "사각형 ROI가 필요합니다.";
                    return false;
                }

                Rectangle bounds = NormalizeImageRectangle(rr.GetBounds());
                if (bounds.Width <= 0 || bounds.Height <= 0)
                {
                    error = "유효한 ROI가 아닙니다.";
                    return false;
                }

                return TryCountForegroundPixelsInRectUsingMask(bounds, brightMin, brightMax, out foregroundCount, out error);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 지정한 사각형 영역에서 threshold 구간(밝기 min~max)의 전경 픽셀 수를
        /// OpenCV mask + CountNonZero로 계산합니다.
        /// </summary>
        public bool TryCountForegroundPixelsInRectUsingMask(Rectangle rect, int brightMin, int brightMax, out int foregroundCount, out string error)
        {
            foregroundCount = 0;
            error = string.Empty;
            try
            {
                if (_grayBuffer == null || _grayBufferWidth <= 0 || _grayBufferHeight <= 0)
                {
                    error = "이미지 버퍼가 없습니다.";
                    return false;
                }

                Rectangle bounds = NormalizeImageRectangle(rect);
                if (bounds.Width <= 0 || bounds.Height <= 0)
                {
                    error = "유효한 영역이 아닙니다.";
                    return false;
                }

                int minV = Math.Max(0, Math.Min(255, brightMin));
                int maxV = Math.Max(minV, Math.Min(255, brightMax));

                using (CvMat gray = new CvMat(_grayBufferHeight, _grayBufferWidth, CvType.CV_8UC1))
                {
                    Marshal.Copy(_grayBuffer, 0, gray.Data, _grayBuffer.Length);

                    using (CvMat binary = new CvMat())
                    using (CvMat mask = CvMat.Zeros(_grayBufferHeight, _grayBufferWidth, CvType.CV_8UC1))
                    using (CvMat masked = new CvMat())
                    {
                        Cv2.InRange(gray, new CvScalar(minV), new CvScalar(maxV), binary);
                        Cv2.Rectangle(mask, new CvRect(bounds.X, bounds.Y, bounds.Width, bounds.Height), CvScalar.White, -1);
                        Cv2.BitwiseAnd(binary, mask, masked);
                        foregroundCount = Cv2.CountNonZero(masked);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 현재 밝기 구간(<see cref="ShowBlob_Bright"/> / <see cref="SetBlobOptions"/>)으로
        /// 이미지 전체에서 4-연결 Blob을 찾아 <see cref="BlobResultCount"/>에 반영합니다. (티칭용)
        /// </summary>
        public void FindBlobObjectsInFullImage()
        {
            try
            {
                _blobResultRects.Clear();

                if (_image == null || _grayBuffer == null)
                {
                    Invalidate();
                    return;
                }

                Rectangle bounds = new Rectangle(0, 0, _grayBufferWidth, _grayBufferHeight);
                List<Rectangle> rects = ComputeBlobRectsForGrayBuffer(
                    _grayBuffer,
                    _grayBufferWidth,
                    _grayBufferHeight,
                    bounds,
                    _blobBrightMin,
                    _blobBrightMax,
                    _blobAreaMin,
                    _blobAreaMax,
                    _blobDiameterMin,
                    _blobDiameterMax,
                    _blobOpening,
                    _blobRoiTouchRemove,
                    int.MaxValue);
                _blobResultRects.AddRange(rects);
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.FindBlobObjectsInFullImage() : " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 전체 이미지 Blob 검출을 백그라운드에서 수행합니다. UI 스레드는 막히지 않습니다.
        /// </summary>
        /// <param name="maxRects">최대 검출 사각형 개수(초과 시 중단). 미리보기 성능·메모리 보호용.</param>
        /// <param name="completed">UI 스레드에서 호출됩니다. (검출 개수, 예외)</param>
        public void BeginFindBlobObjectsInFullImage(int maxRects, Action<int, Exception> completed)
        {
            if (maxRects <= 0)
                maxRects = 25000;

            if (_image == null || _grayBuffer == null)
            {
                completed?.Invoke(0, null);
                return;
            }

            int w = _grayBufferWidth;
            int h = _grayBufferHeight;
            long pixels = (long)w * h;
            if (pixels > MaxFullImageBlobScanPixels)
            {
                completed?.Invoke(0, new InvalidOperationException(
                    "이미지가 너무 커서 자동 Blob 검출을 건너뜁니다. 해상도를 낮추거나 ROI 기반 검출을 사용하세요."));
                return;
            }

            byte[] grayCopy;
            try
            {
                grayCopy = (byte[])_grayBuffer.Clone();
            }
            catch (Exception ex)
            {
                completed?.Invoke(0, ex);
                return;
            }

            int bMin = _blobBrightMin;
            int bMax = _blobBrightMax;
            int aMin = _blobAreaMin;
            int aMax = _blobAreaMax;
            double dMin = _blobDiameterMin;
            double dMax = _blobDiameterMax;
            int op = _blobOpening;
            bool touchRm = _blobRoiTouchRemove;

            Task.Run(() =>
            {
                Exception err = null;
                List<Rectangle> rects = null;
                try
                {
                    Rectangle bounds = new Rectangle(0, 0, w, h);
                    rects = ComputeBlobRectsForGrayBuffer(
                        grayCopy,
                        w,
                        h,
                        bounds,
                        bMin,
                        bMax,
                        aMin,
                        aMax,
                        dMin,
                        dMax,
                        op,
                        touchRm,
                        maxRects);
                }
                catch (Exception ex)
                {
                    err = ex;
                }

                void ApplyUi()
                {
                    try
                    {
                        if (IsDisposed)
                            return;

                        _blobResultRects.Clear();
                        if (rects != null)
                            _blobResultRects.AddRange(rects);
                        Invalidate();
                        completed?.Invoke(_blobResultRects.Count, err);
                    }
                    catch (Exception ex)
                    {
                        completed?.Invoke(0, ex);
                    }
                }

                if (IsDisposed)
                    return;

                if (InvokeRequired)
                    BeginInvoke((MethodInvoker)ApplyUi);
                else
                    ApplyUi();
            });
        }

        /// <summary>visited 배열 크기 상한(대략 48M픽셀 ≈ 48MB bool).</summary>
        private const long MaxFullImageBlobScanPixels = 48_000_000L;

        /// <summary>
        /// 선택된 ROI(없으면 첫 ROI) 안에서만 Blob 검출을 백그라운드에서 수행합니다.
        /// 전체 이미지 스캔을 피하기 위해 티칭/시연에서는 이 경로를 권장합니다.
        /// </summary>
        public void BeginFindBlobObjectsInTargetRoi(int maxRects, Action<int, Exception> completed)
        {
            ROI_Manager roi = null;
            try
            {
                roi = GetBlobTargetRoi();
            }
            catch
            {
                roi = null;
            }

            BeginFindBlobObjectsInRoi(roi, maxRects, completed);
        }

        /// <summary>
        /// 지정 ROI 내부에서만 Blob 검출을 백그라운드에서 수행합니다.
        /// </summary>
        public void BeginFindBlobObjectsInRoi(ROI_Manager roi, int maxRects, Action<int, Exception> completed)
        {
            if (maxRects <= 0)
                maxRects = 25000;

            if (_image == null || _grayBuffer == null)
            {
                completed?.Invoke(0, null);
                return;
            }

            if (roi == null || roi.Visible == false)
            {
                completed?.Invoke(0, new InvalidOperationException("ROI가 없습니다. 먼저 ROI(검사 영역)를 그려주세요."));
                return;
            }

            // ROI 모양/좌표 스냅샷 (백그라운드에서 ROI 객체를 직접 참조하지 않기 위함)
            Rectangle rectRoi = Rectangle.Empty;
            Circle circleRoi = null;
            Point[] polygonPts = null;

            if (roi is ROIRectangle)
            {
                rectRoi = ((ROIRectangle)roi).GetBounds();
            }
            else if (roi is ROICircle)
            {
                circleRoi = ((ROICircle)roi).Circle;
            }
            else if (roi is ROIPolygon)
            {
                polygonPts = ((ROIPolygon)roi).Points;
            }
            else
            {
                // Line/Point는 면적 ROI가 아니므로 범위 검사에 부적합
                completed?.Invoke(0, new InvalidOperationException("현재 ROI 타입은 Blob 검사영역으로 사용할 수 없습니다. Rectangle/Circle/Polygon ROI를 사용하세요."));
                return;
            }

            int w = _grayBufferWidth;
            int h = _grayBufferHeight;
            Rectangle bounds = NormalizeImageRectangle(roi.GetBounds());
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                completed?.Invoke(0, new InvalidOperationException("ROI 영역이 비어 있습니다."));
                return;
            }

            byte[] grayCopy;
            try
            {
                grayCopy = (byte[])_grayBuffer.Clone();
            }
            catch (Exception ex)
            {
                completed?.Invoke(0, ex);
                return;
            }

            int bMin = _blobBrightMin;
            int bMax = _blobBrightMax;
            int aMin = _blobAreaMin;
            int aMax = _blobAreaMax;
            double dMin = _blobDiameterMin;
            double dMax = _blobDiameterMax;
            int op = _blobOpening;
            bool touchRm = _blobRoiTouchRemove;

            Task.Run(() =>
            {
                Exception err = null;
                List<Rectangle> rects = null;
                try
                {
                    Func<int, int, bool> inside;
                    if (circleRoi != null)
                    {
                        int cx = circleRoi.Center.X;
                        int cy = circleRoi.Center.Y;
                        long r2 = (long)circleRoi.Radius * circleRoi.Radius;
                        inside = (x, y) =>
                        {
                            long dx = x - cx;
                            long dy = y - cy;
                            return (dx * dx) + (dy * dy) <= r2;
                        };
                    }
                    else if (polygonPts != null && polygonPts.Length >= 3)
                    {
                        Point[] pts = (Point[])polygonPts.Clone();
                        inside = (x, y) => IsPointInPolygon(pts, x, y);
                    }
                    else
                    {
                        Rectangle rr = rectRoi;
                        inside = (x, y) => rr.Contains(x, y);
                    }

                    rects = ComputeBlobRectsForGrayBuffer(
                        grayCopy,
                        w,
                        h,
                        bounds,
                        bMin,
                        bMax,
                        aMin,
                        aMax,
                        dMin,
                        dMax,
                        op,
                        touchRm,
                        maxRects,
                        inside);
                }
                catch (Exception ex)
                {
                    err = ex;
                }

                void ApplyUi()
                {
                    try
                    {
                        if (IsDisposed)
                            return;

                        _blobResultRects.Clear();
                        if (rects != null)
                            _blobResultRects.AddRange(rects);
                        Invalidate();
                        completed?.Invoke(_blobResultRects.Count, err);
                    }
                    catch (Exception ex)
                    {
                        completed?.Invoke(0, ex);
                    }
                }

                if (IsDisposed)
                    return;

                if (InvokeRequired)
                    BeginInvoke((MethodInvoker)ApplyUi);
                else
                    ApplyUi();
            });
        }

        private static bool IsPointInPolygon(Point[] poly, int x, int y)
        {
            // Ray casting (even-odd rule). Boundary is treated as inside.
            bool inside = false;
            int n = poly.Length;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                int xi = poly[i].X, yi = poly[i].Y;
                int xj = poly[j].X, yj = poly[j].Y;

                // Check if point is on edge (fast-ish)
                if (IsPointOnSegment(xj, yj, xi, yi, x, y))
                    return true;

                bool intersect = ((yi > y) != (yj > y)) &&
                                 (x < (long)(xj - xi) * (y - yi) / (yj - yi + 0.0) + xi);
                if (intersect)
                    inside = !inside;
            }
            return inside;
        }

        private static bool IsPointOnSegment(int x1, int y1, int x2, int y2, int px, int py)
        {
            long cross = (long)(px - x1) * (y2 - y1) - (long)(py - y1) * (x2 - x1);
            if (cross != 0)
                return false;
            int minX = Math.Min(x1, x2), maxX = Math.Max(x1, x2);
            int minY = Math.Min(y1, y2), maxY = Math.Max(y1, y2);
            return px >= minX && px <= maxX && py >= minY && py <= maxY;
        }

        private static List<Rectangle> ComputeBlobRectsForGrayBuffer(
            byte[] gray,
            int bufW,
            int bufH,
            Rectangle bounds,
            int brightMin,
            int brightMax,
            int areaMin,
            int areaMax,
            double diaMin,
            double diaMax,
            int opening,
            bool roiTouchRemove,
            int maxRectCount,
            Func<int, int, bool> inside = null)
        {
            List<Rectangle> blobs = new List<Rectangle>();
            if (gray == null || bufW <= 0 || bufH <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
                return blobs;

            if (bounds.Right > bufW || bounds.Bottom > bufH)
                return blobs;

            int width = bounds.Width;
            int height = bounds.Height;
            BitArray visited;
            try
            {
                checked
                {
                    long cells = (long)width * height;
                    if (cells < 0 || cells > int.MaxValue)
                        return blobs;
                    visited = new BitArray((int)cells);
                }
            }
            catch
            {
                // 가장 흔한 케이스: OutOfMemoryException
                return blobs;
            }
            Queue<Point> queue = new Queue<Point>();
            int[] dx4 = new int[] { 1, -1, 0, 0 };
            int[] dy4 = new int[] { 0, 0, 1, -1 };

            bool IsCand(int x, int y)
            {
                if (inside != null && inside(x, y) == false)
                    return false;
                int gv = gray[(y * bufW) + x];
                return gv >= brightMin && gv <= brightMax;
            }

            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                for (int x = bounds.Left; x < bounds.Right; x++)
                {
                    if (blobs.Count >= maxRectCount)
                        return blobs;

                    int localIndex = ((y - bounds.Top) * width) + (x - bounds.Left);
                    if (visited[localIndex])
                        continue;

                    visited[localIndex] = true;
                    if (IsCand(x, y) == false)
                        continue;

                    queue.Clear();
                    queue.Enqueue(new Point(x, y));
                    int area = 0;
                    int minX = x;
                    int minY = y;
                    int maxX = x;
                    int maxY = y;
                    bool touchesRoiBounds = false;

                    while (queue.Count > 0)
                    {
                        Point pt = queue.Dequeue();
                        area++;
                        if (pt.X < minX) minX = pt.X;
                        if (pt.Y < minY) minY = pt.Y;
                        if (pt.X > maxX) maxX = pt.X;
                        if (pt.Y > maxY) maxY = pt.Y;
                        if (pt.X <= bounds.Left || pt.Y <= bounds.Top || pt.X >= bounds.Right - 1 || pt.Y >= bounds.Bottom - 1)
                            touchesRoiBounds = true;

                        for (int i = 0; i < 4; i++)
                        {
                            int nx = pt.X + dx4[i];
                            int ny = pt.Y + dy4[i];
                            if (nx < bounds.Left || ny < bounds.Top || nx >= bounds.Right || ny >= bounds.Bottom)
                                continue;

                            int nIndex = ((ny - bounds.Top) * width) + (nx - bounds.Left);
                            if (visited[nIndex])
                                continue;

                            visited[nIndex] = true;
                            if (IsCand(nx, ny))
                                queue.Enqueue(new Point(nx, ny));
                        }
                    }

                    Rectangle blobRect = Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
                    double diameter = Math.Max(blobRect.Width, blobRect.Height);
                    if (area < Math.Max(areaMin, opening))
                        continue;
                    if (area > areaMax)
                        continue;
                    if (diameter < diaMin || diameter > diaMax)
                        continue;
                    if (roiTouchRemove && touchesRoiBounds)
                        continue;

                    blobs.Add(blobRect);
                }
            }

            return blobs;
        }

        /// <summary>Blob 검출 사각형 스타일(Auto Run 기본: 핫핑크, 티칭 등에서 녹색으로 변경 가능).</summary>
        public void SetBlobResultOverlayColors(Color penColor, Color fillColor)
        {
            try
            {
                _blobResultPenColor = penColor;
                _blobResultFillColor = fillColor;
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.SetBlobResultOverlayColors() : " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 데모용 전처리 옵션을 설정합니다. (그레이버퍼는 다음 이미지 설정/프레임 갱신 시 적용됩니다.)
        /// </summary>
        public void SetPreprocessOptions(bool enabled, int blurRadius, int backgroundRadius)
        {
            try
            {
                _preEnabled = enabled;
                _preBlurRadius = Math.Max(0, Math.Min(20, blurRadius));
                _preBgRadius = Math.Max(0, Math.Min(80, backgroundRadius));
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.SetPreprocessOptions() : " + ex.Message, ex);
            }
        }

        private void FindBlobConnectedComponents(Rectangle bounds, Func<int, int, bool> isCandidate)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                Invalidate();
                return;
            }

            int width = bounds.Width;
            int height = bounds.Height;
            BitArray visited;
            try
            {
                checked
                {
                    long cells = (long)width * height;
                    if (cells < 0 || cells > int.MaxValue)
                    {
                        Invalidate();
                        return;
                    }
                    visited = new BitArray((int)cells);
                }
            }
            catch
            {
                Invalidate();
                return;
            }
            Queue<Point> queue = new Queue<Point>();
            int[] dx4 = new int[] { 1, -1, 0, 0 };
            int[] dy4 = new int[] { 0, 0, 1, -1 };

            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                for (int x = bounds.Left; x < bounds.Right; x++)
                {
                    int localIndex = ((y - bounds.Top) * width) + (x - bounds.Left);
                    if (visited[localIndex])
                        continue;

                    visited[localIndex] = true;
                    if (isCandidate(x, y) == false)
                        continue;

                    queue.Clear();
                    queue.Enqueue(new Point(x, y));
                    int area = 0;
                    int minX = x;
                    int minY = y;
                    int maxX = x;
                    int maxY = y;
                    bool touchesRoiBounds = false;

                    while (queue.Count > 0)
                    {
                        Point pt = queue.Dequeue();
                        area++;
                        if (pt.X < minX) minX = pt.X;
                        if (pt.Y < minY) minY = pt.Y;
                        if (pt.X > maxX) maxX = pt.X;
                        if (pt.Y > maxY) maxY = pt.Y;
                        if (pt.X <= bounds.Left || pt.Y <= bounds.Top || pt.X >= bounds.Right - 1 || pt.Y >= bounds.Bottom - 1)
                            touchesRoiBounds = true;

                        for (int i = 0; i < 4; i++)
                        {
                            int nx = pt.X + dx4[i];
                            int ny = pt.Y + dy4[i];
                            if (nx < bounds.Left || ny < bounds.Top || nx >= bounds.Right || ny >= bounds.Bottom)
                                continue;

                            int nIndex = ((ny - bounds.Top) * width) + (nx - bounds.Left);
                            if (visited[nIndex])
                                continue;

                            visited[nIndex] = true;
                            if (isCandidate(nx, ny))
                                queue.Enqueue(new Point(nx, ny));
                        }
                    }

                    Rectangle blobRect = Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
                    double diameter = Math.Max(blobRect.Width, blobRect.Height);
                    if (area < Math.Max(_blobAreaMin, _blobOpening))
                        continue;
                    if (area > _blobAreaMax)
                        continue;
                    if (diameter < _blobDiameterMin || diameter > _blobDiameterMax)
                        continue;
                    if (_blobRoiTouchRemove && touchesRoiBounds)
                        continue;

                    _blobResultRects.Add(blobRect);
                }
            }

            Invalidate();
        }

        private bool IsBlobCandidateByGrayOnly(int x, int y)
        {
            try
            {
                int grayValue = GetGrayValue(x, y);
                return grayValue >= _blobBrightMin && grayValue <= _blobBrightMax;
            }
            catch
            {
                return false;
            }
        }

        public void ToggleCenterCross()
        {
            try
            {
                if (_image == null)
                    return;

                _showCenterCross = !_showCenterCross;
                if (_showCenterCross)
                    ClampCrossToImage();
                RaiseViewStateChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ToggleCenterCross() : " + ex.Message, ex);
            }
        }

        public void ToggleMiniMap()
        {
            try
            {
                if (_image == null)
                    return;

                _showMiniMap = !_showMiniMap;
                RaiseViewStateChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ToggleMiniMap() : " + ex.Message, ex);
            }
        }

        public void ToggleZoomMode()
        {
            try
            {
                if (_image == null)
                    return;

                SetZoomModeEnabledInternal(!_zoomModeEnabled, true, true);
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ToggleZoomMode() : " + ex.Message, ex);
            }
        }

        public void TogglePixelGrayValue()
        {
            try
            {
                if (_image == null)
                    return;

                _showPixelGrayValue = !_showPixelGrayValue;
                if (_showPixelGrayValue)
                {
                    UpdateCurrentMouseGrayInfo(PointToClient(Cursor.Position));
                }
                else
                {
                    _isMouseOverImage = false;
                    _currentMouseGrayText = string.Empty;
                }
                RaiseViewStateChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.TogglePixelGrayValue() : " + ex.Message, ex);
            }
        }

        public void ToggleAverageGrayMode()
        {
            try
            {
                if (_image == null)
                    return;

                _avgModeEnabled = !_avgModeEnabled;

                if (_avgModeEnabled == false)
                {
                    _isAvgSelecting = false;
                    _avgSelectionImageRect = RectangleF.Empty;
                    _avgGrayText = string.Empty;
                }

                RaiseViewStateChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ToggleAverageGrayMode() : " + ex.Message, ex);
            }
        }

        public void FitToWindow()
        {
            try
            {
                if (_image == null || ClientSize.Width <= 0 || ClientSize.Height <= 0)
                {
                    _zoom = 1.0f;
                    _offset = PointF.Empty;
                    RaiseViewStateChanged();
                    RaiseSyncStateChanged();
                    Invalidate();
                    return;
                }

                float scaleX = (float)ClientSize.Width / Math.Max(1, _image.Width);
                float scaleY = (float)ClientSize.Height / Math.Max(1, _image.Height);
                _zoom = Math.Min(scaleX, scaleY);
                if (_zoom < MinZoom) _zoom = MinZoom;
                if (_zoom > MaxZoom) _zoom = MaxZoom;

                UpdateOffsetToCenter();
                RaiseViewStateChanged();
                RaiseSyncStateChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.FitToWindow() : " + ex.Message, ex);
            }
        }

        public void ZoomIn()
        {
            try
            {
                if (_image == null)
                    return;

                ApplyZoom(1.25f, new PointF(ClientSize.Width / 2.0f, ClientSize.Height / 2.0f));
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ZoomIn() : " + ex.Message, ex);
            }
        }

        public void ZoomOut()
        {
            try
            {
                if (_image == null)
                    return;

                ApplyZoom(0.8f, new PointF(ClientSize.Width / 2.0f, ClientSize.Height / 2.0f));
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ZoomOut() : " + ex.Message, ex);
            }
        }

        public void ZoomInAt(Point clientPoint)
        {
            try
            {
                if (_image == null)
                    return;

                ApplyZoom(1.25f, clientPoint);
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ZoomInAt() : " + ex.Message, ex);
            }
        }

        public void ZoomOutAt(Point clientPoint)
        {
            try
            {
                if (_image == null)
                    return;

                ApplyZoom(0.8f, clientPoint);
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ZoomOutAt() : " + ex.Message, ex);
            }
        }

        public ImageSyncState ExportSyncState()
        {
            try
            {
                if (_image == null)
                    return null;

                RectangleF visibleRect = GetVisibleImageRectInImageCoord();
                float centerX = visibleRect.X + (visibleRect.Width / 2.0f);
                float centerY = visibleRect.Y + (visibleRect.Height / 2.0f);

                ImageSyncState state = new ImageSyncState();
                state.Zoom = _zoom;
                state.CenterRatioX = _image.Width <= 0 ? 0.5f : Math.Max(0.0f, Math.Min(1.0f, centerX / Math.Max(1, _image.Width)));
                state.CenterRatioY = _image.Height <= 0 ? 0.5f : Math.Max(0.0f, Math.Min(1.0f, centerY / Math.Max(1, _image.Height)));
                state.ZoomModeEnabled = _zoomModeEnabled;
                return state;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        public void ApplySyncState(ImageSyncState state)
        {
            try
            {
                if (_image == null || state == null)
                    return;

                _suspendSyncStateChanged = true;
                SetZoomModeEnabledInternal(state.ZoomModeEnabled, false, false);

                _zoom = state.Zoom;
                if (_zoom < MinZoom) _zoom = MinZoom;
                if (_zoom > MaxZoom) _zoom = MaxZoom;

                float imageCenterX = Math.Max(0.0f, Math.Min(_image.Width, state.CenterRatioX * _image.Width));
                float imageCenterY = Math.Max(0.0f, Math.Min(_image.Height, state.CenterRatioY * _image.Height));
                PointF clientCenter = new PointF(ClientSize.Width / 2.0f, ClientSize.Height / 2.0f);
                _offset = new PointF(
                    clientCenter.X - (imageCenterX * _zoom),
                    clientCenter.Y - (imageCenterY * _zoom));

                NormalizeOffset();
                RaiseViewStateChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ApplySyncState() : " + ex.Message, ex);
            }
            finally
            {
                _suspendSyncStateChanged = false;
            }
        }

        // 주석: 신규 Draw 함수
        public void DrawString(Point _pt, string _strMsg, Color _color, int _size)
        {
            DrawString(new PointF(_pt.X, _pt.Y), _strMsg, _color, _size);
        }

        public void DrawString(PointF _pt, string _strMsg, Color _color, int _size)
        {
            try
            {
                if (_image == null)
                    return;

                // 같은 좌표에 텍스트가 반복 누적되면(프레임 갱신/반복 검사) 글자가 겹쳐 보임.
                // 동일 좌표(픽셀 기준)로 들어오는 텍스트는 "교체"하도록 기존 항목을 제거한다.
                // (ROI/도형 오버레이는 별도 타입이므로 영향 없음)
                for (int i = _overlays.Count - 1; i >= 0; i--)
                {
                    OverlayItem prev = _overlays[i];
                    if (prev == null || prev.Type != OverlayType.Text)
                        continue;

                    if (Math.Abs(prev.Point1.X - _pt.X) < 0.01f && Math.Abs(prev.Point1.Y - _pt.Y) < 0.01f)
                    {
                        _overlays.RemoveAt(i);
                    }
                }

                OverlayItem item = new OverlayItem();
                item.Type = OverlayType.Text;
                item.Point1 = _pt;
                item.Text = _strMsg ?? string.Empty;
                item.Color = _color;
                item.FontSize = _size <= 0 ? 12 : _size;
                item.LineWidth = 1.0f;
                _overlays.Add(item);
                InvalidateIfNotBatchingOverlays();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.DrawString() : " + ex.Message, ex);
            }
        }

        public void DrawRect(Rectangle _rect, Color _color, int _Width)
        {
            try
            {
                if (_image == null || _rect.Width <= 0 || _rect.Height <= 0)
                    return;

                OverlayItem item = new OverlayItem();
                item.Type = OverlayType.Rectangle;
                item.Rect = _rect;
                item.Color = _color;
                item.LineWidth = _Width <= 0 ? 1 : _Width;
                _overlays.Add(item);
                InvalidateIfNotBatchingOverlays();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.DrawRect() : " + ex.Message, ex);
            }
        }

        public void DrawLine(Point _1stPt, Point _2ndPt, Color _color, int _Width)
        {
            try
            {
                if (_image == null)
                    return;

                OverlayItem item = new OverlayItem();
                item.Type = OverlayType.Line;
                item.Point1 = _1stPt;
                item.Point2 = _2ndPt;
                item.Color = _color;
                item.LineWidth = _Width <= 0 ? 1 : _Width;
                _overlays.Add(item);
                InvalidateIfNotBatchingOverlays();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.DrawLine() : " + ex.Message, ex);
            }
        }

        public void DrawPoint(Point _pt, string _type, Color _color, int _Width)
        {
            try
            {
                if (_image == null)
                    return;

                OverlayItem item = new OverlayItem();
                item.Type = OverlayType.Point;
                item.Point1 = _pt;
                item.PointType = string.IsNullOrWhiteSpace(_type) ? "cross" : _type.ToLowerInvariant();
                item.Color = _color;
                item.LineWidth = _Width <= 0 ? 1 : _Width;
                _overlays.Add(item);
                InvalidateIfNotBatchingOverlays();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.DrawPoint() : " + ex.Message, ex);
            }
        }

        public void DrawCircle(Circle _circle, Color _color, int _Width)
        {
            try
            {
                if (_image == null || _circle == null || _circle.Radius <= 0)
                    return;

                OverlayItem item = new OverlayItem();
                item.Type = OverlayType.Circle;
                item.Circle = new Circle(_circle.Center, _circle.Radius);
                item.Color = _color;
                item.LineWidth = _Width <= 0 ? 1 : _Width;
                _overlays.Add(item);
                InvalidateIfNotBatchingOverlays();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.DrawCircle() : " + ex.Message, ex);
            }
        }

        public void DrawPolygon(Point[] _pois, Color _color, int _Width)
        {
            try
            {
                if (_image == null || _pois == null || _pois.Length < 2)
                    return;

                OverlayItem item = new OverlayItem();
                item.Type = OverlayType.Polygon;
                item.Points = Array.ConvertAll(_pois, x => new PointF(x.X, x.Y));
                item.Color = _color;
                item.LineWidth = _Width <= 0 ? 1 : _Width;
                _overlays.Add(item);
                InvalidateIfNotBatchingOverlays();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.DrawPolygon() : " + ex.Message, ex);
            }
        }

        // 주석: 기존 함수 호환용
        public void DrawStringOverlay(string text, PointF imagePoint, Color color, float fontSize)
        {
            DrawString(imagePoint, text, color, (int)Math.Round(fontSize));
        }

        public void DrawCrossLineOverlay(PointF imagePoint, float size, Color color, float lineWidth)
        {
            int half = (int)Math.Max(4, Math.Round(size));
            Point center = Point.Round(imagePoint);
            DrawLine(new Point(center.X - half, center.Y), new Point(center.X + half, center.Y), color, (int)Math.Round(lineWidth));
            DrawLine(new Point(center.X, center.Y - half), new Point(center.X, center.Y + half), color, (int)Math.Round(lineWidth));
        }

        public void DrawRectOverlay(RectangleF imageRect, Color color, float lineWidth, bool fill)
        {
            DrawRect(Rectangle.Round(imageRect), color, (int)Math.Round(lineWidth));
        }

        public void DrawCircleOverlay(PointF imageCenter, float radius, Color color, float lineWidth, bool fill)
        {
            DrawCircle(new Circle(Point.Round(imageCenter), (int)Math.Round(radius)), color, (int)Math.Round(lineWidth));
        }

        public void AddROI(ROI_Manager roi)
        {
            try
            {
                if (_image == null || roi == null)
                    return;

                _rois.Add(roi);
                SelectROIInternal(roi, false);
                RaiseRoiCollectionChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.AddROI() : " + ex.Message, ex);
            }
        }

        public void ClearROI()
        {
            try
            {
                _rois.Clear();
                _selectedRoi = null;
                RaiseSelectedRoiChanged();
                RaiseRoiCollectionChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ClearROI() : " + ex.Message, ex);
            }
        }

        public void RemoveSelectedROI()
        {
            try
            {
                if (_selectedRoi == null)
                    return;

                _rois.Remove(_selectedRoi);
                _selectedRoi = null;
                RaiseSelectedRoiChanged();
                RaiseRoiCollectionChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.RemoveSelectedROI() : " + ex.Message, ex);
            }
        }

        public void SelectROIByName(string roiName)
        {
            try
            {
                ROI_Manager roi = null;
                foreach (ROI_Manager item in _rois)
                {
                    if (item != null && string.Equals(item.Name, roiName, StringComparison.OrdinalIgnoreCase))
                    {
                        roi = item;
                        break;
                    }
                }

                SelectROIInternal(roi, true);
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.SelectROIByName() : " + ex.Message, ex);
            }
        }

        public void RefreshROI()
        {
            try
            {
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.RefreshROI() : " + ex.Message, ex);
            }
        }

        public Bitmap CreateRenderedBitmap()
        {
            try
            {
                if (_image == null)
                    return null;

                Bitmap result = new Bitmap(_image.Width, _image.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.Clear(Color.Black);
                    g.DrawImage(_image, 0, 0, _image.Width, _image.Height);
                    DrawROIItemsForBitmap(g);
                    DrawOverlayItemsForBitmap(g);
                    DrawPixelGrayValueForBitmap(g);
                    DrawAverageGrayForBitmap(g);
                    DrawCenterCrossForBitmap(g);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.CreateRenderedBitmap() : " + ex.Message, ex);
            }
        }

        public void SaveRenderedImage(string filePath)
        {
            try
            {
                if (_image == null)
                    return;

                using (Bitmap result = CreateRenderedBitmap())
                {
                    if (result == null)
                        return;

                    result.Save(filePath, ResolveImageFormat(filePath));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.SaveRenderedImage() : " + ex.Message, ex);
            }
        }

        /// <summary>
        /// ROI·오버레이 없이 현재 이미지 픽셀만 저장합니다(Halcon NCC 등). 라이브 프레임처럼 경로가 없을 때 임시 파일용.
        /// </summary>
        public bool TrySaveSourceImageToFile(string filePath)
        {
            try
            {
                if (_image == null || string.IsNullOrWhiteSpace(filePath))
                    return false;

                string dir = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(dir) == false)
                    Directory.CreateDirectory(dir);

                using (Bitmap copy = new Bitmap(_image))
                {
                    copy.Save(filePath, ResolveImageFormat(filePath));
                }

                return File.Exists(filePath);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Mouse Events

        protected override void OnResize(EventArgs e)
        {
            try
            {
                base.OnResize(e);
                DisposeMiniMapCache();

                if (_image != null)
                {
                    NormalizeOffset();
                    Invalidate();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            try
            {
                base.OnMouseDown(e);
                Focus();
                RaiseViewSelected();

                if (e.Button == MouseButtons.Right)
                {
                    _rightButtonDown = true;
                    _contextMenuOpenedByHold = false;
                    _rightDownPoint = e.Location;
                    _rightHoldTimer.Stop();
                    _rightHoldTimer.Start();
                    Capture = true;
                }

                if (_image == null)
                    return;

                if (e.Button == MouseButtons.Left)
                {
                    _leftButtonDown = true;
                    _leftDownPoint = e.Location;

                    if (TryBeginRoiInteraction(e.Location))
                    {
                        Capture = true;
                        return;
                    }

                    if (_zoomModeEnabled == false)
                    {
                        _isPanning = true;
                        _panStartPoint = e.Location;
                        _panStartOffset = _offset;
                        Capture = true;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (_avgModeEnabled)
                    {
                        _isAvgSelecting = true;
                        _avgSelectionStartClient = e.Location;
                        _avgSelectionCurrentClient = e.Location;
                        _avgSelectionImageRect = RectangleF.Empty;
                        _avgGrayText = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                base.OnMouseMove(e);

                if (_showPixelGrayValue)
                {
                    UpdateCurrentMouseGrayInfo(e.Location);
                }

                if (_image != null && HandleRoiMouseMove(e.Location))
                {
                    Invalidate();
                    return;
                }

                if (_image != null && _isPanning && _zoomModeEnabled == false)
                {
                    _offset = new PointF(
                        _panStartOffset.X + (e.X - _panStartPoint.X),
                        _panStartOffset.Y + (e.Y - _panStartPoint.Y));
                    NormalizeOffset();
                    RaiseSyncStateChanged();
                    Invalidate();
                }

                if (_isAvgSelecting)
                {
                    _avgSelectionCurrentClient = e.Location;
                    Invalidate();
                }

                if (_rightButtonDown)
                {
                    if (Math.Abs(e.X - _rightDownPoint.X) > 4 || Math.Abs(e.Y - _rightDownPoint.Y) > 4)
                    {
                        _rightHoldTimer.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            try
            {
                base.OnMouseUp(e);

                if (e.Button == MouseButtons.Left)
                {
                    bool moved = Math.Abs(e.X - _leftDownPoint.X) > 3 || Math.Abs(e.Y - _leftDownPoint.Y) > 3;

                    if (_isRoiMoving || _isRoiResizing)
                    {
                        EndRoiInteraction();
                    }

                    if (_isPanning)
                        _isPanning = false;

                    if (_image != null && _leftButtonDown && _zoomModeEnabled && moved == false && _isRoiMoving == false && _isRoiResizing == false)
                    {
                        ZoomInAt(e.Location);
                    }

                    _leftButtonDown = false;
                    Capture = false;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    _rightHoldTimer.Stop();

                    if (_isAvgSelecting)
                    {
                        _avgSelectionCurrentClient = e.Location;
                        bool avgMoved = Math.Abs(e.X - _rightDownPoint.X) > 4 || Math.Abs(e.Y - _rightDownPoint.Y) > 4;

                        if (avgMoved)
                        {
                            UpdateAverageGraySelection();
                        }
                        else
                        {
                            _avgSelectionImageRect = RectangleF.Empty;
                            _avgGrayText = string.Empty;
                        }

                        _isAvgSelecting = false;
                        _rightButtonDown = false;
                        _contextMenuOpenedByHold = false;
                        Capture = false;
                        Invalidate();
                        return;
                    }

                    bool zoomMoved = Math.Abs(e.X - _rightDownPoint.X) > 3 || Math.Abs(e.Y - _rightDownPoint.Y) > 3;

                    if (_image != null && _contextMenuOpenedByHold == false && _zoomModeEnabled && zoomMoved == false)
                    {
                        ZoomOutAt(e.Location);
                    }

                    _rightButtonDown = false;
                    _contextMenuOpenedByHold = false;
                    Capture = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            try
            {
                base.OnMouseWheel(e);

                if (_image == null)
                    return;

                if (e.Delta > 0)
                    ZoomInAt(e.Location);
                else if (e.Delta < 0)
                    ZoomOutAt(e.Location);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            try
            {
                base.OnMouseLeave(e);

                if (Capture)
                    return;

                _isPanning = false;
                EndRoiInteraction();
                _leftButtonDown = false;
                _rightButtonDown = false;
                _rightHoldTimer.Stop();
                _isMouseOverImage = false;
                _currentMouseGrayText = string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region Keyboard (십자 기준선 화살표 3픽셀 이동)

        /// <summary>
        /// 십자 표시 중이면 화살표 키로 기준점을 3픽셀 이동하고 true를 반환합니다.
        /// AutoRun 등 부모 <see cref="UserControl.ProcessCmdKey"/>에서 호출해 포커스가 툴바에 있어도 동작시킬 수 있습니다.
        /// </summary>
        public bool TryProcessCenterCrossArrowKeys(Keys keyData)
        {
            try
            {
                if (_showCenterCross == false || _image == null)
                    return false;

                switch (keyData)
                {
                    case Keys.Left:
                        NudgeCrossByPixels(-3, 0);
                        return true;
                    case Keys.Right:
                        NudgeCrossByPixels(3, 0);
                        return true;
                    case Keys.Up:
                        NudgeCrossByPixels(0, -3);
                        return true;
                    case Keys.Down:
                        NudgeCrossByPixels(0, 3);
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>탭/부모로 화살표가 넘기기 전에 십자 이동을 잡습니다.</summary>
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (Focused && TryProcessCenterCrossArrowKeys(keyData))
                    return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Paint

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);

                Graphics g = e.Graphics;
                g.Clear(BackColor);
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;

                if (_image == null)
                {
                    DrawBackgroundTitle(g);
                    DrawStatusMessage(g);
                    DrawSelectionBorder(g);
                    return;
                }

                RectangleF imageRect = GetDisplayedImageRect();
                g.DrawImage(_image, imageRect);
                DrawBlobBrightPreview(g, imageRect);
                DrawROIItems(g);
                DrawBlobObjectRects(g);
                DrawOverlayItems(g);
                DrawPixelGrayValue(g, imageRect);
                DrawAverageGray(g);
                DrawCenterCross(g, imageRect);
                DrawMiniMap(g);
                DrawSelectionBorder(g);
                DrawCurrentMouseGrayInfo(g);
                DrawStatusMessage(g);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void SetStatusMessage(string message)
        {
            try
            {
                _statusMessage = message ?? string.Empty;
                _statusMessageAt = DateTime.Now;
                Invalidate();
            }
            catch
            {
            }
        }

        private void DrawStatusMessage(Graphics g)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_statusMessage))
                    return;

                // 너무 오래된 메시지는 자연히 사라지게(화면이 계속 경고로 덮이지 않도록)
                if (_statusMessageAt != DateTime.MinValue && (DateTime.Now - _statusMessageAt).TotalSeconds > 10)
                    return;

                RectangleF rect = _image != null ? GetDisplayedImageRect() : ClientRectangle;
                DrawInfoText(g, rect, _statusMessage);
            }
            catch
            {
            }
        }

        private void SafeClearImageState()
        {
            try
            {
                DisposeImage();
                _imagePath = string.Empty;
                _overlays.Clear();
                _rois.Clear();
                _selectedRoi = null;
                _dragRoi = null;
                _dragRoiHandleIndex = -1;
                _isRoiMoving = false;
                _isRoiResizing = false;
                _blobResultRects.Clear();
                ClearGrayBuffer();
                DisposeMiniMapCache();
                _showUserRoiOverlay = true;
                Invalidate();
            }
            catch
            {
            }
        }

        private void DrawBackgroundTitle(Graphics g)
        {
            try
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(160, 220, 220, 220)))
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString(ViewTitle, Font, brush, ClientRectangle, sf);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawSelectionBorder(Graphics g)
        {
            try
            {
                Color borderColor = _isTargetSelected ? ViewerUiStyle.SelectedViewBorderColor : ViewerUiStyle.NormalViewBorderColor;
                float borderThickness = _isTargetSelected ? ViewerUiStyle.SelectedViewBorderThickness : ViewerUiStyle.NormalViewBorderThickness;
                float inset = Math.Max(0.5f, borderThickness / 2.0f);
                float drawWidth = Math.Max(0.0f, Width - borderThickness);
                float drawHeight = Math.Max(0.0f, Height - borderThickness);

                using (Pen pen = new Pen(borderColor, borderThickness))
                {
                    pen.Alignment = PenAlignment.Center;
                    g.DrawRectangle(pen, inset, inset, drawWidth, drawHeight);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawBlobBrightPreview(Graphics g, RectangleF imageRect)
        {
            try
            {
                if (_image == null || _blobShowBright == false)
                    return;

                Rectangle bounds;
                ROI_Manager roi = GetBlobTargetRoi();
                if (roi != null)
                    bounds = NormalizeImageRectangle(roi.GetBounds());
                else
                    bounds = new Rectangle(0, 0, _image.Width, _image.Height);

                if (bounds.Width <= 0 || bounds.Height <= 0)
                    return;

                GraphicsState state = g.Save();
                g.SetClip(imageRect);

                using (SolidBrush brush = new SolidBrush(Color.FromArgb(140, 255, 0, 0)))
                {
                    for (int y = bounds.Top; y < bounds.Bottom; y++)
                    {
                        for (int x = bounds.Left; x < bounds.Right; x++)
                        {
                            if (roi != null)
                            {
                                if (IsBlobCandidatePixel(roi, x, y) == false)
                                    continue;
                            }
                            else
                            {
                                int grayValue = GetGrayValue(x, y);
                                if (grayValue < _blobBrightMin || grayValue > _blobBrightMax)
                                    continue;
                            }

                            RectangleF pixelRect = ImagePixelToClientRectangle(x, y);
                            if (pixelRect.Width < 1.0f || pixelRect.Height < 1.0f)
                            {
                                float clientX = _offset.X + (x * _zoom);
                                float clientY = _offset.Y + (y * _zoom);
                                float drawX = (float)Math.Floor(clientX);
                                float drawY = (float)Math.Floor(clientY);
                                g.FillRectangle(brush, drawX, drawY, 1.0f, 1.0f);
                                continue;
                            }

                            g.FillRectangle(brush, pixelRect);
                        }
                    }
                }

                g.Restore(state);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawBlobObjectRects(Graphics g)
        {
            try
            {
                if (_image == null || _blobResultRects.Count == 0)
                    return;

                GraphicsState state = g.Save();
                g.TranslateTransform(_offset.X, _offset.Y);
                g.ScaleTransform(_zoom, _zoom);

                using (Pen pen = new Pen(_blobResultPenColor, Math.Max(1.0f, 2.0f / Math.Max(_zoom, 0.01f))))
                using (SolidBrush brush = new SolidBrush(_blobResultFillColor))
                {
                    foreach (Rectangle rect in _blobResultRects)
                    {
                        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                        g.FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                }

                g.Restore(state);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawROIItems(Graphics g)
        {
            try
            {
                if (_image == null || _rois.Count == 0 || _showUserRoiOverlay == false)
                    return;

                GraphicsState state = g.Save();
                g.TranslateTransform(_offset.X, _offset.Y);
                g.ScaleTransform(_zoom, _zoom);

                foreach (ROI_Manager roi in _rois)
                {
                    DrawSingleROI(g, roi, true);
                }

                g.Restore(state);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawROIItemsForBitmap(Graphics g)
        {
            try
            {
                if (_showUserRoiOverlay == false)
                    return;
                foreach (ROI_Manager roi in _rois)
                {
                    DrawSingleROI(g, roi, false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawOverlayItems(Graphics g)
        {
            try
            {
                if (_image == null || _overlays.Count == 0)
                    return;

                GraphicsState state = g.Save();
                g.TranslateTransform(_offset.X, _offset.Y);
                g.ScaleTransform(_zoom, _zoom);

                foreach (OverlayItem item in _overlays)
                {
                    DrawSingleOverlay(g, item, true);
                }

                g.Restore(state);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawOverlayItemsForBitmap(Graphics g)
        {
            try
            {
                foreach (OverlayItem item in _overlays)
                {
                    DrawSingleOverlay(g, item, false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawSingleOverlay(Graphics g, OverlayItem item, bool useZoomTransform)
        {
            try
            {
                if (item == null)
                    return;

                float width = useZoomTransform ? item.LineWidth / Math.Max(_zoom, 0.01f) : item.LineWidth;
                if (width < 1.0f) width = 1.0f;

                using (Pen pen = new Pen(item.Color, width))
                using (SolidBrush brush = new SolidBrush(item.Color))
                {
                    pen.LineJoin = LineJoin.Round;
                    switch (item.Type)
                    {
                        case OverlayType.Text:
                            float fontSize = useZoomTransform ? item.FontSize / Math.Max(_zoom, 0.01f) : item.FontSize;
                            if (fontSize < 4) fontSize = 4;
                            string drawText = item.Text ?? string.Empty;
                            using (Font font = new Font("Arial", fontSize, FontStyle.Bold))
                            using (StringFormat sf = new StringFormat())
                            {
                                sf.Alignment = StringAlignment.Center;
                                sf.LineAlignment = StringAlignment.Center;
                                SizeF sz = g.MeasureString(drawText, font);
                                RectangleF layout = new RectangleF(
                                    item.Point1.X - sz.Width / 2f,
                                    item.Point1.Y - sz.Height / 2f,
                                    sz.Width,
                                    sz.Height);
                                g.DrawString(drawText, font, brush, layout, sf);
                            }
                            break;

                        case OverlayType.Rectangle:
                            g.DrawRectangle(pen, item.Rect.X, item.Rect.Y, item.Rect.Width, item.Rect.Height);
                            break;

                        case OverlayType.Line:
                            g.DrawLine(pen, item.Point1, item.Point2);
                            break;

                        case OverlayType.Point:
                            DrawPointShape(g, pen, brush, item.Point1, item.PointType);
                            break;

                        case OverlayType.Circle:
                            if (item.Circle != null)
                            {
                                RectangleF circleRect = new RectangleF(
                                    item.Circle.Center.X - item.Circle.Radius,
                                    item.Circle.Center.Y - item.Circle.Radius,
                                    item.Circle.Radius * 2,
                                    item.Circle.Radius * 2);
                                g.DrawEllipse(pen, circleRect);
                            }
                            break;

                        case OverlayType.Polygon:
                            if (item.Points != null && item.Points.Length >= 2)
                            {
                                g.DrawPolygon(pen, item.Points);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawPointShape(Graphics g, Pen pen, Brush brush, PointF point, string pointType)
        {
            try
            {
                float size = Math.Max(4.0f, pen.Width * 4.0f);
                string type = string.IsNullOrWhiteSpace(pointType) ? "cross" : pointType.ToLowerInvariant();

                if (type == "point")
                {
                    RectangleF rect = new RectangleF(point.X - (size / 2.0f), point.Y - (size / 2.0f), size, size);
                    g.FillEllipse(brush, rect);
                    return;
                }

                if (type == "star")
                {
                    g.DrawLine(pen, point.X - size, point.Y, point.X + size, point.Y);
                    g.DrawLine(pen, point.X, point.Y - size, point.X, point.Y + size);
                    g.DrawLine(pen, point.X - (size * 0.75f), point.Y - (size * 0.75f), point.X + (size * 0.75f), point.Y + (size * 0.75f));
                    g.DrawLine(pen, point.X - (size * 0.75f), point.Y + (size * 0.75f), point.X + (size * 0.75f), point.Y - (size * 0.75f));
                    return;
                }

                g.DrawLine(pen, point.X - size, point.Y, point.X + size, point.Y);
                g.DrawLine(pen, point.X, point.Y - size, point.X, point.Y + size);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawSingleROI(Graphics g, ROI_Manager roi, bool useZoomTransform)
        {
            try
            {
                if (roi == null)
                    return;

                float width = useZoomTransform ? roi.Width / Math.Max(_zoom, 0.01f) : roi.Width;
                if (width < 1.0f) width = 1.0f;

                using (Pen pen = new Pen(roi.Color, width))
                using (SolidBrush brush = new SolidBrush(roi.Color))
                {
                    pen.LineJoin = LineJoin.Round;

                    if (roi is ROIRectangle)
                    {
                        ROIRectangle rectRoi = (ROIRectangle)roi;
                        g.DrawRectangle(pen, rectRoi.Rect.X, rectRoi.Rect.Y, rectRoi.Rect.Width, rectRoi.Rect.Height);
                    }
                    else if (roi is ROILine)
                    {
                        ROILine lineRoi = (ROILine)roi;
                        g.DrawLine(pen, lineRoi.FirstPoint, lineRoi.SecondPoint);
                    }
                    else if (roi is ROIPoint)
                    {
                        ROIPoint pointRoi = (ROIPoint)roi;
                        DrawPointShape(g, pen, brush, pointRoi.Point, pointRoi.PointType);
                    }
                    else if (roi is ROICircle)
                    {
                        ROICircle circleRoi = (ROICircle)roi;
                        if (circleRoi.Circle != null)
                        {
                            RectangleF rect = new RectangleF(
                                circleRoi.Circle.Center.X - circleRoi.Circle.Radius,
                                circleRoi.Circle.Center.Y - circleRoi.Circle.Radius,
                                circleRoi.Circle.Radius * 2,
                                circleRoi.Circle.Radius * 2);
                            g.DrawEllipse(pen, rect);
                        }
                    }
                    else if (roi is ROIPolygon)
                    {
                        ROIPolygon polygonRoi = (ROIPolygon)roi;
                        if (polygonRoi.Points != null && polygonRoi.Points.Length >= 2)
                        {
                            g.DrawPolygon(pen, polygonRoi.Points);
                        }
                    }

                    if (roi.Selected)
                    {
                        DrawRoiHandles(g, roi);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawPixelGrayValue(Graphics g, RectangleF imageRect)
        {
            try
            {
                if (_image == null || _showPixelGrayValue == false)
                    return;

                Rectangle visibleRect = GetVisiblePixelBounds();
                int pixelCount = visibleRect.Width * visibleRect.Height;
                if (pixelCount <= 0)
                    return;

                if (pixelCount > GrayValueMaxDrawCount)
                {
                    return;
                }

                GraphicsState state = g.Save();
                g.SetClip(imageRect);
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                using (StringFormat sf = new StringFormat())
                using (SolidBrush lowBrush = new SolidBrush(Color.Yellow))
                using (SolidBrush midBrush = new SolidBrush(Color.OrangeRed))
                using (SolidBrush highBrush = new SolidBrush(Color.DeepSkyBlue))
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    for (int y = visibleRect.Top; y < visibleRect.Bottom; y++)
                    {
                        for (int x = visibleRect.Left; x < visibleRect.Right; x++)
                        {
                            RectangleF pixelRect = ImagePixelToClientRectangle(x, y);
                            if (pixelRect.Width < 3.0f || pixelRect.Height < 3.0f)
                                continue;

                            int grayValue = GetGrayValue(x, y);
                            float fontSize = Math.Max(3.0f, Math.Min(pixelRect.Width, pixelRect.Height) * 0.25f);

                            using (Font font = new Font("Arial", fontSize, FontStyle.Bold))
                            {
                                g.DrawString(grayValue.ToString(), font, ResolveGrayTextBrush(grayValue, lowBrush, midBrush, highBrush), pixelRect, sf);
                            }
                        }
                    }
                }

                g.Restore(state);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawPixelGrayValueForBitmap(Graphics g)
        {
            try
            {
                if (_image == null || _showPixelGrayValue == false)
                    return;

                if (_image.Width * _image.Height > 2500)
                    return;

                using (StringFormat sf = new StringFormat())
                using (SolidBrush lowBrush = new SolidBrush(Color.Yellow))
                using (SolidBrush midBrush = new SolidBrush(Color.OrangeRed))
                using (SolidBrush highBrush = new SolidBrush(Color.DeepSkyBlue))
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    for (int y = 0; y < _image.Height; y++)
                    {
                        for (int x = 0; x < _image.Width; x++)
                        {
                            RectangleF pixelRect = new RectangleF(x, y, 1, 1);
                            int grayValue = GetGrayValue(x, y);

                            using (Font font = new Font("Arial", 0.33f, FontStyle.Bold, GraphicsUnit.Pixel))
                            {
                                g.DrawString(grayValue.ToString(), font, ResolveGrayTextBrush(grayValue, lowBrush, midBrush, highBrush), pixelRect, sf);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawAverageGray(Graphics g)
        {
            try
            {
                if (_image == null)
                    return;

                RectangleF drawRectImage = _isAvgSelecting ? GetCurrentAverageSelectionRect() : _avgSelectionImageRect;
                if (drawRectImage.Width <= 0 || drawRectImage.Height <= 0)
                    return;

                RectangleF drawRectClient = ImageRectToClientRect(drawRectImage);

                using (Pen pen = new Pen(Color.Orange, 1.6f))
                using (SolidBrush fillBrush = new SolidBrush(Color.FromArgb(45, Color.Orange)))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.FillRectangle(fillBrush, drawRectClient);
                    g.DrawRectangle(pen, drawRectClient.X, drawRectClient.Y, drawRectClient.Width, drawRectClient.Height);
                }

                string displayText = _isAvgSelecting ? BuildAverageGrayText(drawRectImage) : _avgGrayText;
                if (string.IsNullOrWhiteSpace(displayText))
                    return;

                RectangleF labelRect = new RectangleF(drawRectClient.X + 4.0f, drawRectClient.Y + 4.0f, 220.0f, 24.0f);
                using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                using (Font font = new Font("Arial", 10.0f, FontStyle.Bold))
                {
                    g.FillRectangle(backBrush, labelRect);
                    g.DrawString(displayText, font, textBrush, labelRect.Location);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawAverageGrayForBitmap(Graphics g)
        {
            try
            {
                if (_image == null)
                    return;

                if (_avgSelectionImageRect.Width <= 0 || _avgSelectionImageRect.Height <= 0)
                    return;

                using (Pen pen = new Pen(Color.Orange, 1.0f))
                using (SolidBrush fillBrush = new SolidBrush(Color.FromArgb(45, Color.Orange)))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                using (Font font = new Font("Arial", 12.0f, FontStyle.Bold))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.FillRectangle(fillBrush, _avgSelectionImageRect);
                    g.DrawRectangle(pen, _avgSelectionImageRect.X, _avgSelectionImageRect.Y, _avgSelectionImageRect.Width, _avgSelectionImageRect.Height);

                    RectangleF labelRect = new RectangleF(_avgSelectionImageRect.X + 4.0f, _avgSelectionImageRect.Y + 4.0f, 220.0f, 24.0f);
                    g.FillRectangle(backBrush, labelRect);
                    g.DrawString(_avgGrayText, font, textBrush, labelRect.Location);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawCurrentMouseGrayInfo(Graphics g)
        {
            try
            {
                if (_image == null || _showPixelGrayValue == false)
                    return;

                if (_isMouseOverImage == false || string.IsNullOrWhiteSpace(_currentMouseGrayText))
                    return;

                RectangleF drawRect = new RectangleF(8.0f, 8.0f, 180.0f, 24.0f);
                using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                using (Font font = new Font("Arial", 9.0f, FontStyle.Bold))
                {
                    g.FillRectangle(backBrush, drawRect);
                    g.DrawString(_currentMouseGrayText, font, textBrush, drawRect.Location);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private Brush ResolveGrayTextBrush(int grayValue, Brush lowBrush, Brush midBrush, Brush highBrush)
        {
            try
            {
                if (grayValue <= 80)
                    return lowBrush;

                if (grayValue <= 160)
                    return midBrush;

                return highBrush;
            }
            catch
            {
                return lowBrush;
            }
        }

        private void UpdateCurrentMouseGrayInfo(Point clientPoint)
        {
            try
            {
                if (_image == null || _showPixelGrayValue == false)
                {
                    _isMouseOverImage = false;
                    _currentMouseGrayText = string.Empty;
                    return;
                }

                PointF imagePoint = ScreenToImage(clientPoint);
                int imgX = (int)Math.Floor(imagePoint.X);
                int imgY = (int)Math.Floor(imagePoint.Y);
                bool inside = imgX >= 0 && imgY >= 0 && imgX < _grayBufferWidth && imgY < _grayBufferHeight;

                _currentMouseClient = clientPoint;
                _currentMouseImage = new Point(imgX, imgY);
                _isMouseOverImage = inside;

                if (inside)
                {
                    int grayValue = GetGrayValue(imgX, imgY);
                    _currentMouseGrayText = "X:" + imgX.ToString() + " Y:" + imgY.ToString() + " G:" + grayValue.ToString();
                }
                else
                {
                    _currentMouseGrayText = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void UpdateAverageGraySelection()
        {
            try
            {
                RectangleF rect = GetCurrentAverageSelectionRect();
                if (rect.Width <= 0 || rect.Height <= 0)
                {
                    _avgSelectionImageRect = RectangleF.Empty;
                    _avgGrayText = string.Empty;
                    Invalidate();
                    return;
                }

                _avgSelectionImageRect = rect;
                _avgGrayText = BuildAverageGrayText(rect);
                Invalidate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private RectangleF GetCurrentAverageSelectionRect()
        {
            try
            {
                PointF start = ScreenToImage(_avgSelectionStartClient);
                PointF end = ScreenToImage(_avgSelectionCurrentClient);

                Rectangle rawRect = Rectangle.FromLTRB(
                    (int)Math.Floor(Math.Min(start.X, end.X)),
                    (int)Math.Floor(Math.Min(start.Y, end.Y)),
                    (int)Math.Ceiling(Math.Max(start.X, end.X)),
                    (int)Math.Ceiling(Math.Max(start.Y, end.Y)));

                Rectangle safeRect = NormalizeImageRectangle(rawRect);
                return new RectangleF(safeRect.X, safeRect.Y, safeRect.Width, safeRect.Height);
            }
            catch
            {
                return RectangleF.Empty;
            }
        }

        private string BuildAverageGrayText(RectangleF rect)
        {
            try
            {
                Rectangle intRect = Rectangle.Round(rect);
                intRect = NormalizeImageRectangle(intRect);
                if (intRect.Width <= 0 || intRect.Height <= 0)
                    return string.Empty;

                double avg = GetAverageGrayValue(intRect);
                return "Avg : " + avg.ToString("0.00");
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>이미지 로드 시 십자 중심을 이미지 중앙 픽셀로 둡니다.</summary>
        private void UpdateCrossDefaultPosition()
        {
            try
            {
                if (_image == null)
                    return;

                _crossPixelX = Math.Max(0, _image.Width / 2);
                _crossPixelY = Math.Max(0, _image.Height / 2);
                ClampCrossToImage();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void ClampCrossToImage()
        {
            try
            {
                if (_image == null)
                    return;

                int maxX = Math.Max(0, _image.Width - 1);
                int maxY = Math.Max(0, _image.Height - 1);
                if (_crossPixelX < 0) _crossPixelX = 0;
                if (_crossPixelX > maxX) _crossPixelX = maxX;
                if (_crossPixelY < 0) _crossPixelY = 0;
                if (_crossPixelY > maxY) _crossPixelY = maxY;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void NudgeCrossByPixels(int deltaX, int deltaY)
        {
            try
            {
                if (_image == null || _showCenterCross == false)
                    return;

                _crossPixelX += deltaX;
                _crossPixelY += deltaY;
                ClampCrossToImage();
                RaiseViewStateChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawCenterCross(Graphics g, RectangleF imageRect)
        {
            try
            {
                if (_showCenterCross == false || _image == null)
                    return;

                int w = Math.Max(1, _image.Width);
                int h = Math.Max(1, _image.Height);
                float u = (_crossPixelX + 0.5f) / w;
                float v = (_crossPixelY + 0.5f) / h;
                float centerX = imageRect.Left + u * imageRect.Width;
                float centerY = imageRect.Top + v * imageRect.Height;

                using (Pen pen = new Pen(Color.Lime, 1.2f))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawLine(pen, imageRect.Left, centerY, imageRect.Right, centerY);
                    g.DrawLine(pen, centerX, imageRect.Top, centerX, imageRect.Bottom);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawCenterCrossForBitmap(Graphics g)
        {
            try
            {
                if (_showCenterCross == false || _image == null)
                    return;

                float centerX = _crossPixelX + 0.5f;
                float centerY = _crossPixelY + 0.5f;
                using (Pen pen = new Pen(Color.Lime, 1.0f))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawLine(pen, 0, centerY, _image.Width, centerY);
                    g.DrawLine(pen, centerX, 0, centerX, _image.Height);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawMiniMap(Graphics g)
        {
            try
            {
                if (_image == null || _showMiniMap == false)
                    return;

                int mapWidth = Math.Min(180, Math.Max(120, Width / 5));
                int mapHeight = Math.Min(130, Math.Max(90, Height / 5));
                Rectangle mapRect = new Rectangle(Width - mapWidth - 12, 12, mapWidth, mapHeight);
                if (mapRect.Width <= 10 || mapRect.Height <= 10)
                    return;

                using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(220, 0, 0, 0)))
                using (Pen borderPen = new Pen(Color.Gold, 1.0f))
                {
                    g.FillRectangle(backBrush, mapRect);
                    g.DrawRectangle(borderPen, mapRect);
                }

                Rectangle innerRect = Rectangle.Inflate(mapRect, -4, -4);
                if (innerRect.Width <= 4 || innerRect.Height <= 4)
                    return;

                float scaleX = (float)innerRect.Width / Math.Max(1, _image.Width);
                float scaleY = (float)innerRect.Height / Math.Max(1, _image.Height);
                float scale = Math.Min(scaleX, scaleY);
                int dispW = Math.Max(1, (int)Math.Round(_image.Width * scale));
                int dispH = Math.Max(1, (int)Math.Round(_image.Height * scale));
                int startX = innerRect.Left + ((innerRect.Width - dispW) / 2);
                int startY = innerRect.Top + ((innerRect.Height - dispH) / 2);
                Rectangle imageBox = new Rectangle(startX, startY, dispW, dispH);

                EnsureMiniMapCache(imageBox.Size);
                if (_miniMapCache != null)
                {
                    g.DrawImage(_miniMapCache, imageBox);
                }

                using (Pen imgPen = new Pen(Color.Yellow, 1.0f))
                {
                    g.DrawRectangle(imgPen, imageBox);
                }

                RectangleF visibleRect = GetVisibleImageRectInImageCoord();
                RectangleF miniVisible = new RectangleF(
                    imageBox.Left + (visibleRect.X * scale),
                    imageBox.Top + (visibleRect.Y * scale),
                    Math.Max(1.0f, visibleRect.Width * scale),
                    Math.Max(1.0f, visibleRect.Height * scale));

                using (Pen visiblePen = new Pen(Color.DeepPink, 1.2f))
                {
                    g.DrawRectangle(visiblePen, miniVisible.X, miniVisible.Y, miniVisible.Width, miniVisible.Height);
                }

                if (_showCenterCross)
                {
                    int iw = Math.Max(1, _image.Width);
                    int ih = Math.Max(1, _image.Height);
                    float u = (_crossPixelX + 0.5f) / iw;
                    float v = (_crossPixelY + 0.5f) / ih;
                    float centerX = imageBox.Left + u * imageBox.Width;
                    float centerY = imageBox.Top + v * imageBox.Height;
                    using (Pen crossPen = new Pen(Color.Lime, 1.0f))
                    {
                        crossPen.DashStyle = DashStyle.Dash;
                        g.DrawLine(crossPen, imageBox.Left, centerY, imageBox.Right, centerY);
                        g.DrawLine(crossPen, centerX, imageBox.Top, centerX, imageBox.Bottom);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region Internal Helper

        private void EnsureMiniMapCache(Size targetSize)
        {
            try
            {
                if (_image == null || targetSize.Width <= 0 || targetSize.Height <= 0)
                    return;

                if (_miniMapCache != null && _miniMapCacheVersion == _imageVersion && _miniMapCacheSize == targetSize)
                    return;

                DisposeMiniMapCache();
                _miniMapCache = new Bitmap(targetSize.Width, targetSize.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(_miniMapCache))
                {
                    g.Clear(Color.Black);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.DrawImage(_image, new Rectangle(0, 0, targetSize.Width, targetSize.Height));
                }

                _miniMapCacheSize = targetSize;
                _miniMapCacheVersion = _imageVersion;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private RectangleF GetVisibleImageRectInImageCoord()
        {
            try
            {
                if (_image == null || _zoom <= 0.0f)
                    return RectangleF.Empty;

                float left = Math.Max(0.0f, (0.0f - _offset.X) / _zoom);
                float top = Math.Max(0.0f, (0.0f - _offset.Y) / _zoom);
                float right = Math.Min(_image.Width, (ClientSize.Width - _offset.X) / _zoom);
                float bottom = Math.Min(_image.Height, (ClientSize.Height - _offset.Y) / _zoom);
                if (right < left) right = left;
                if (bottom < top) bottom = top;
                return RectangleF.FromLTRB(left, top, right, bottom);
            }
            catch
            {
                return RectangleF.Empty;
            }
        }

        private RectangleF GetDisplayedImageRect()
        {
            try
            {
                if (_image == null)
                    return RectangleF.Empty;

                return new RectangleF(_offset.X, _offset.Y, _image.Width * _zoom, _image.Height * _zoom);
            }
            catch
            {
                return RectangleF.Empty;
            }
        }

        private Rectangle GetVisiblePixelBounds()
        {
            try
            {
                RectangleF visible = GetVisibleImageRectInImageCoord();
                int left = Math.Max(0, (int)Math.Floor(visible.Left));
                int top = Math.Max(0, (int)Math.Floor(visible.Top));
                int right = Math.Min(_image.Width, (int)Math.Ceiling(visible.Right));
                int bottom = Math.Min(_image.Height, (int)Math.Ceiling(visible.Bottom));

                if (right <= left || bottom <= top)
                    return Rectangle.Empty;

                return Rectangle.FromLTRB(left, top, right, bottom);
            }
            catch
            {
                return Rectangle.Empty;
            }
        }

        private RectangleF ImagePixelToClientRectangle(int x, int y)
        {
            try
            {
                return new RectangleF(
                    _offset.X + (x * _zoom),
                    _offset.Y + (y * _zoom),
                    _zoom,
                    _zoom);
            }
            catch
            {
                return RectangleF.Empty;
            }
        }

        private RectangleF ImageRectToClientRect(RectangleF imageRect)
        {
            try
            {
                return new RectangleF(
                    _offset.X + (imageRect.X * _zoom),
                    _offset.Y + (imageRect.Y * _zoom),
                    imageRect.Width * _zoom,
                    imageRect.Height * _zoom);
            }
            catch
            {
                return RectangleF.Empty;
            }
        }

        private void UpdateOffsetToCenter()
        {
            try
            {
                if (_image == null)
                    return;

                _offset = new PointF(
                    (ClientSize.Width - (_image.Width * _zoom)) / 2.0f,
                    (ClientSize.Height - (_image.Height * _zoom)) / 2.0f);
                NormalizeOffset();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void NormalizeOffset()
        {
            try
            {
                if (_image == null)
                {
                    _offset = PointF.Empty;
                    return;
                }

                float dispW = _image.Width * _zoom;
                float dispH = _image.Height * _zoom;

                if (dispW <= ClientSize.Width)
                {
                    _offset.X = (ClientSize.Width - dispW) / 2.0f;
                }
                else
                {
                    if (_offset.X > 0) _offset.X = 0;
                    if (_offset.X < ClientSize.Width - dispW) _offset.X = ClientSize.Width - dispW;
                }

                if (dispH <= ClientSize.Height)
                {
                    _offset.Y = (ClientSize.Height - dispH) / 2.0f;
                }
                else
                {
                    if (_offset.Y > 0) _offset.Y = 0;
                    if (_offset.Y < ClientSize.Height - dispH) _offset.Y = ClientSize.Height - dispH;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void ApplyZoom(float factor, PointF clientPoint)
        {
            try
            {
                if (_image == null)
                    return;

                float oldZoom = _zoom;
                float newZoom = _zoom * factor;
                if (newZoom < MinZoom) newZoom = MinZoom;
                if (newZoom > MaxZoom) newZoom = MaxZoom;
                if (Math.Abs(newZoom - oldZoom) < 0.0001f)
                    return;

                PointF imgPoint = ScreenToImage(clientPoint);
                _zoom = newZoom;
                _offset = new PointF(
                    clientPoint.X - (imgPoint.X * _zoom),
                    clientPoint.Y - (imgPoint.Y * _zoom));
                NormalizeOffset();
                RaiseViewStateChanged();
                RaiseSyncStateChanged();
                Invalidate();
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.ApplyZoom() : " + ex.Message, ex);
            }
        }

        private PointF ScreenToImage(PointF clientPoint)
        {
            try
            {
                if (_zoom <= 0.0f)
                    return PointF.Empty;

                return new PointF((clientPoint.X - _offset.X) / _zoom, (clientPoint.Y - _offset.Y) / _zoom);
            }
            catch
            {
                return PointF.Empty;
            }
        }

        private void SetZoomModeEnabledInternal(bool value, bool raiseViewState, bool raiseSyncState)
        {
            try
            {
                _zoomModeEnabled = value;
                Cursor = _zoomModeEnabled ? Cursors.Cross : Cursors.SizeAll;

                if (raiseViewState)
                    RaiseViewStateChanged();

                if (raiseSyncState)
                    RaiseSyncStateChanged();

                Invalidate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void RightHoldTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                _rightHoldTimer.Stop();

                if (_rightButtonDown == false)
                    return;

                if (LongPressContextMenuStrip != null)
                {
                    _contextMenuOpenedByHold = true;
                    Point screenPoint = PointToScreen(_rightDownPoint);
                    LongPressContextMenuStrip.Show(screenPoint);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private Bitmap LoadBitmapUnlocked(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (Image src = Image.FromStream(fs, false, false))
                {
                    // 이미 32bpp ARGB면 변환 없이 복사만(디코딩·LockBits 경로와 포맷 일치)
                    if (src is Bitmap srcBm && srcBm.PixelFormat == PixelFormat.Format32bppArgb)
                        return new Bitmap(srcBm);

                    Bitmap converted = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppArgb);
                    using (Graphics g = Graphics.FromImage(converted))
                    {
                        g.DrawImage(src, 0, 0, src.Width, src.Height);
                    }

                    return converted;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ImageCanvasControl.LoadBitmapUnlocked() : " + ex.Message, ex);
            }
        }

        private ImageFormat ResolveImageFormat(string filePath)
        {
            try
            {
                string ext = Path.GetExtension(filePath).ToLowerInvariant();
                switch (ext)
                {
                    case ".bmp": return ImageFormat.Bmp;
                    case ".jpg":
                    case ".jpeg": return ImageFormat.Jpeg;
                    case ".gif": return ImageFormat.Gif;
                    case ".tif":
                    case ".tiff": return ImageFormat.Tiff;
                    default: return ImageFormat.Png;
                }
            }
            catch
            {
                return ImageFormat.Png;
            }
        }

        private void BuildGrayBuffer()
        {
            try
            {
                ClearGrayBuffer();

                if (_image == null)
                    return;

                _grayBufferWidth = _image.Width;
                _grayBufferHeight = _image.Height;
                long pixels = (long)_grayBufferWidth * _grayBufferHeight;
                // 매우 큰 이미지는 (Bitmap + GrayBuffer + 임시 버퍼)로 메모리 폭주가 나면서 프로세스가 종료될 수 있음.
                // "절대 죽지 않는" 목표를 위해, 여기서는 안전하게 그레이 버퍼 생성을 건너뛰고 기능을 제한한다.
                // (Blob/픽셀값/평균값 기능은 _grayBuffer == null 일 때 자동으로 비활성화됨)
                const long MaxGrayBufferPixels = 48_000_000L; // ~48MB(byte) + 여유. (기존 Blob full scan 상한과 맞춤)
                if (pixels <= 0 || pixels > MaxGrayBufferPixels)
                {
                    _grayBuffer = null;
                    _grayBufferWidth = 0;
                    _grayBufferHeight = 0;
                    SetStatusMessage("이미지가 너무 커서(픽셀 " + pixels.ToString() + ") 분석용 버퍼를 만들지 않습니다.");
                    return;
                }

                _grayBuffer = new byte[(int)pixels];

                Rectangle rect = new Rectangle(0, 0, _image.Width, _image.Height);
                BitmapData data = _image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                try
                {
                    int stride = data.Stride;
                    int w = _image.Width;
                    int h = _image.Height;
                    // 메모리 안정성: 전체 rawBuffer(4bpp * pixels)를 만들지 않고, 한 줄씩만 복사하여 계산한다.
                    // (대형 이미지에서 rawBuffer가 OOM을 유발할 수 있음)
                    byte[] row = new byte[stride];
                    for (int y = 0; y < h; y++)
                    {
                        IntPtr src = IntPtr.Add(data.Scan0, y * stride);
                        System.Runtime.InteropServices.Marshal.Copy(src, row, 0, stride);
                        int rowGray = y * _grayBufferWidth;
                        for (int x = 0; x < w; x++)
                        {
                            int baseIndex = x * 4;
                            byte b = row[baseIndex + 0];
                            byte g = row[baseIndex + 1];
                            byte r = row[baseIndex + 2];
                            int gray = (299 * r + 587 * g + 114 * b) / 1000;
                            _grayBuffer[rowGray + x] = (byte)gray;
                        }
                    }
                }
                finally
                {
                    _image.UnlockBits(data);
                }

                if (_preEnabled)
                    ApplyPreprocessInPlace(_grayBuffer, _grayBufferWidth, _grayBufferHeight, _preBlurRadius, _preBgRadius);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ImageCanvasControl.BuildGrayBuffer() : " + ex);
                ClearGrayBuffer();
            }
        }

        private static void ApplyPreprocessInPlace(byte[] gray, int w, int h, int blurRadius, int bgRadius)
        {
            if (gray == null || w <= 0 || h <= 0)
                return;

            // 1) small blur
            if (blurRadius > 0)
            {
                byte[] blurred = BoxBlur(gray, w, h, blurRadius);
                Buffer.BlockCopy(blurred, 0, gray, 0, gray.Length);
            }

            // 2) background flattening: large blur then subtract (top-hat like)
            if (bgRadius > 0)
            {
                byte[] bg = BoxBlur(gray, w, h, bgRadius);
                for (int i = 0; i < gray.Length; i++)
                {
                    int v = gray[i] - bg[i] + 128; // re-center
                    if (v < 0) v = 0;
                    if (v > 255) v = 255;
                    gray[i] = (byte)v;
                }
            }
        }

        private static byte[] BoxBlur(byte[] src, int w, int h, int radius)
        {
            int r = Math.Max(1, radius);
            byte[] tmp = new byte[src.Length];
            byte[] dst = new byte[src.Length];
            int win = (r * 2) + 1;

            // horizontal pass
            for (int y = 0; y < h; y++)
            {
                int row = y * w;
                int sum = 0;

                // init window
                for (int x = -r; x <= r; x++)
                {
                    int xx = x < 0 ? 0 : (x >= w ? w - 1 : x);
                    sum += src[row + xx];
                }

                for (int x = 0; x < w; x++)
                {
                    tmp[row + x] = (byte)(sum / win);
                    int xRemove = x - r;
                    int xAdd = x + r + 1;
                    int xr = xRemove < 0 ? 0 : xRemove;
                    int xa = xAdd >= w ? w - 1 : xAdd;
                    sum += src[row + xa] - src[row + xr];
                }
            }

            // vertical pass
            for (int x = 0; x < w; x++)
            {
                int sum = 0;
                for (int y = -r; y <= r; y++)
                {
                    int yy = y < 0 ? 0 : (y >= h ? h - 1 : y);
                    sum += tmp[(yy * w) + x];
                }

                for (int y = 0; y < h; y++)
                {
                    dst[(y * w) + x] = (byte)(sum / win);
                    int yRemove = y - r;
                    int yAdd = y + r + 1;
                    int yr = yRemove < 0 ? 0 : yRemove;
                    int ya = yAdd >= h ? h - 1 : yAdd;
                    sum += tmp[(ya * w) + x] - tmp[(yr * w) + x];
                }
            }

            return dst;
        }

        private void ClearGrayBuffer()
        {
            try
            {
                _grayBuffer = null;
                _grayBufferWidth = 0;
                _grayBufferHeight = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private bool TryBeginRoiInteraction(Point clientPoint)
        {
            try
            {
                Point imagePoint = Point.Round(ScreenToImage(clientPoint));
                int tolerance = Math.Max(4, (int)Math.Round(8.0f / Math.Max(0.1f, _zoom)));

                for (int i = _rois.Count - 1; i >= 0; i--)
                {
                    ROI_Manager roi = _rois[i];
                    if (roi == null || roi.Visible == false)
                        continue;

                    int handleIndex = roi.HitTestHandle(imagePoint, tolerance);
                    if (handleIndex >= 0)
                    {
                        SelectROIInternal(roi, true);
                        _dragRoi = roi;
                        _dragRoiHandleIndex = handleIndex;
                        _isRoiResizing = true;
                        _isRoiMoving = false;
                        _roiLastImagePoint = imagePoint;
                        return true;
                    }
                }

                for (int i = _rois.Count - 1; i >= 0; i--)
                {
                    ROI_Manager roi = _rois[i];
                    if (roi == null || roi.Visible == false)
                        continue;

                    if (roi.HitTest(imagePoint, tolerance))
                    {
                        SelectROIInternal(roi, true);
                        _dragRoi = roi;
                        _dragRoiHandleIndex = -1;
                        _isRoiMoving = true;
                        _isRoiResizing = false;
                        _roiLastImagePoint = imagePoint;
                        return true;
                    }
                }

                if (_selectedRoi != null)
                    SelectROIInternal(null, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return false;
        }

        private bool HandleRoiMouseMove(Point clientPoint)
        {
            try
            {
                if (_dragRoi == null)
                    return false;

                Point imagePoint = Point.Round(ScreenToImage(clientPoint));
                if (_isRoiMoving)
                {
                    _dragRoi.Move(imagePoint.X - _roiLastImagePoint.X, imagePoint.Y - _roiLastImagePoint.Y);
                    _roiLastImagePoint = imagePoint;
                    return true;
                }

                if (_isRoiResizing)
                {
                    _dragRoi.ResizeByHandle(_dragRoiHandleIndex, imagePoint);
                    _roiLastImagePoint = imagePoint;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return false;
        }

        private void EndRoiInteraction()
        {
            try
            {
                bool hadDrag = _isRoiMoving || _isRoiResizing;
                _isRoiMoving = false;
                _isRoiResizing = false;
                _dragRoi = null;
                _dragRoiHandleIndex = -1;
                if (hadDrag)
                    RaiseRoiCollectionChanged();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void SelectROIInternal(ROI_Manager roi, bool invalidate)
        {
            try
            {
                if (ReferenceEquals(_selectedRoi, roi))
                    return;

                foreach (ROI_Manager item in _rois)
                {
                    if (item != null)
                        item.Selected = false;
                }

                _selectedRoi = roi;
                if (_selectedRoi != null)
                    _selectedRoi.Selected = true;

                RaiseSelectedRoiChanged();
                if (invalidate)
                    Invalidate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawRoiHandles(Graphics g, ROI_Manager roi)
        {
            try
            {
                Point[] handles = roi.GetHandlePoints();
                if (handles == null || handles.Length == 0)
                    return;

                float handleSize = Math.Max(4.0f, 8.0f / Math.Max(0.2f, _zoom));
                using (SolidBrush fill = new SolidBrush(Color.MediumPurple))
                using (Pen pen = new Pen(Color.White, Math.Max(0.8f, 1.0f / Math.Max(0.2f, _zoom))))
                {
                    foreach (Point pt in handles)
                    {
                        RectangleF rc = new RectangleF(pt.X - (handleSize / 2.0f), pt.Y - (handleSize / 2.0f), handleSize, handleSize);
                        g.FillRectangle(fill, rc);
                        g.DrawRectangle(pen, rc.X, rc.Y, rc.Width, rc.Height);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void RaiseRoiCollectionChanged()
        {
            try
            {
                EventHandler handler = RoiCollectionChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void RaiseSelectedRoiChanged()
        {
            try
            {
                EventHandler handler = SelectedRoiChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private ROI_Manager GetBlobTargetRoi()
        {
            try
            {
                if (_selectedRoi != null && _selectedRoi.Visible)
                    return _selectedRoi;

                foreach (ROI_Manager roi in _rois)
                {
                    if (roi != null && roi.Visible)
                        return roi;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return null;
        }

        private bool IsBlobCandidatePixel(ROI_Manager roi, int x, int y)
        {
            try
            {
                if (roi == null || IsPointInsideRoi(roi, x, y) == false)
                    return false;

                int grayValue = GetGrayValue(x, y);
                return grayValue >= _blobBrightMin && grayValue <= _blobBrightMax;
            }
            catch
            {
                return false;
            }
        }

        private bool IsPointInsideRoi(ROI_Manager roi, int x, int y)
        {
            try
            {
                if (roi is ROIRectangle)
                {
                    return ((ROIRectangle)roi).Rect.Contains(x, y);
                }
                if (roi is ROICircle)
                {
                    ROICircle circleRoi = (ROICircle)roi;
                    if (circleRoi.Circle == null)
                        return false;
                    float dx = x - circleRoi.Circle.Center.X;
                    float dy = y - circleRoi.Circle.Center.Y;
                    return ((dx * dx) + (dy * dy)) <= (circleRoi.Circle.Radius * circleRoi.Circle.Radius);
                }
                if (roi is ROIPolygon)
                {
                    ROIPolygon polygonRoi = (ROIPolygon)roi;
                    if (polygonRoi.Points == null || polygonRoi.Points.Length < 3)
                        return false;
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddPolygon(polygonRoi.Points);
                        return path.IsVisible(new Point(x, y));
                    }
                }
                if (roi is ROILine)
                {
                    return roi.HitTest(new Point(x, y), 2);
                }
                if (roi is ROIPoint)
                {
                    return roi.HitTest(new Point(x, y), 3);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return false;
        }

        private Rectangle NormalizeImageRectangle(Rectangle rect)
        {
            try
            {
                int imageWidth = _grayBufferWidth > 0 ? _grayBufferWidth : (_image != null ? _image.Width : 0);
                int imageHeight = _grayBufferHeight > 0 ? _grayBufferHeight : (_image != null ? _image.Height : 0);
                if (imageWidth <= 0 || imageHeight <= 0)
                    return Rectangle.Empty;

                int left = Math.Min(rect.Left, rect.Right);
                int right = Math.Max(rect.Left, rect.Right);
                int top = Math.Min(rect.Top, rect.Bottom);
                int bottom = Math.Max(rect.Top, rect.Bottom);

                left = Math.Max(0, Math.Min(imageWidth, left));
                right = Math.Max(0, Math.Min(imageWidth, right));
                top = Math.Max(0, Math.Min(imageHeight, top));
                bottom = Math.Max(0, Math.Min(imageHeight, bottom));

                if (right <= left || bottom <= top)
                    return Rectangle.Empty;

                return Rectangle.FromLTRB(left, top, right, bottom);
            }
            catch
            {
                return Rectangle.Empty;
            }
        }

        private int GetGrayValue(int x, int y)
        {
            try
            {
                if (_grayBuffer == null)
                    return 0;

                if (x < 0 || y < 0 || x >= _grayBufferWidth || y >= _grayBufferHeight)
                    return 0;

                return _grayBuffer[(y * _grayBufferWidth) + x];
            }
            catch
            {
                return 0;
            }
        }

        private double GetAverageGrayValue(Rectangle rect)
        {
            try
            {
                rect = NormalizeImageRectangle(rect);
                if (rect.Width <= 0 || rect.Height <= 0)
                    return 0.0;

                long sum = 0;
                long count = 0;

                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    int rowIndex = y * _grayBufferWidth;
                    for (int x = rect.Left; x < rect.Right; x++)
                    {
                        sum += _grayBuffer[rowIndex + x];
                        count++;
                    }
                }

                if (count <= 0)
                    return 0.0;

                return (double)sum / (double)count;
            }
            catch
            {
                return 0.0;
            }
        }

        private void DrawInfoText(Graphics g, RectangleF imageRect, string message)
        {
            try
            {
                RectangleF messageRect = new RectangleF(
                    imageRect.Left + 10.0f,
                    imageRect.Top + 10.0f,
                    Math.Min(280.0f, imageRect.Width - 20.0f),
                    28.0f);

                using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                using (Font font = new Font("Arial", 10.0f, FontStyle.Bold))
                {
                    g.FillRectangle(backBrush, messageRect);
                    g.DrawString(message, font, textBrush, messageRect.Location);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DisposeImage()
        {
            try
            {
                if (_image != null)
                {
                    _image.Dispose();
                    _image = null;
                }
            }
            catch
            {
            }
        }

        private void DisposeMiniMapCache()
        {
            try
            {
                if (_miniMapCache != null)
                {
                    _miniMapCache.Dispose();
                    _miniMapCache = null;
                }
                _miniMapCacheSize = Size.Empty;
                _miniMapCacheVersion = -1;
            }
            catch
            {
            }
        }

        private void BumpImageVersion()
        {
            try
            {
                _imageVersion++;
                DisposeMiniMapCache();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void RaiseViewStateChanged()
        {
            try
            {
                if (ViewStateChanged != null)
                    ViewStateChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void RaiseViewSelected()
        {
            try
            {
                if (ViewSelected != null)
                    ViewSelected(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void RaiseSyncStateChanged()
        {
            try
            {
                if (_suspendSyncStateChanged)
                    return;

                if (SyncStateChanged != null)
                    SyncStateChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #endregion
    }
}
