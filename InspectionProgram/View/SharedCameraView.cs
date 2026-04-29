using System;
using System.Drawing;
using System.IO;

namespace InspectionProgram.View
{
    public sealed class SharedCameraView : IDisposable
    {
        private Bitmap _displayImage = null;
        private string _imagePath = string.Empty;
        private readonly object _syncRoot = new object();

        public event EventHandler DisplayImageChanged;

        public SharedCameraView(int cameraIndex)
        {
            CameraIndex = cameraIndex;
        }

        public int CameraIndex { get; private set; }

        public Bitmap DisplayImage
        {
            get
            {
                lock (_syncRoot)
                {
                    return _displayImage;
                }
            }
            set
            {
                SetDisplayImage(value, _imagePath);
            }
        }

        public string ImagePath
        {
            get
            {
                lock (_syncRoot)
                {
                    return _imagePath;
                }
            }
        }

        public void LoadImageFromFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return;

                if (File.Exists(filePath) == false)
                    throw new FileNotFoundException("Image file not found.", filePath);

                using (Bitmap source = new Bitmap(filePath))
                {
                    SetDisplayImage((Bitmap)source.Clone(), filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SharedCameraView.LoadImageFromFile() : " + ex.Message, ex);
            }
        }

        public void SetDisplayImage(Bitmap bitmap, string filePath)
        {
            Bitmap oldBitmap = null;
            Bitmap newBitmap = null;

            try
            {
                if (bitmap != null)
                    newBitmap = (Bitmap)bitmap.Clone();

                lock (_syncRoot)
                {
                    oldBitmap = _displayImage;
                    _displayImage = newBitmap;
                    _imagePath = filePath ?? string.Empty;
                }

                if (oldBitmap != null)
                    oldBitmap.Dispose();

                OnDisplayImageChanged();
            }
            catch (Exception ex)
            {
                if (newBitmap != null)
                    newBitmap.Dispose();

                throw new Exception("SharedCameraView.SetDisplayImage() : " + ex.Message, ex);
            }
        }

        public Bitmap CreateDisplayImageClone()
        {
            try
            {
                lock (_syncRoot)
                {
                    if (_displayImage == null)
                        return null;

                    return (Bitmap)_displayImage.Clone();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SharedCameraView.CreateDisplayImageClone() : " + ex.Message, ex);
            }
        }

        public void ClearDisplayImage()
        {
            Bitmap oldBitmap = null;

            try
            {
                lock (_syncRoot)
                {
                    oldBitmap = _displayImage;
                    _displayImage = null;
                    _imagePath = string.Empty;
                }

                if (oldBitmap != null)
                    oldBitmap.Dispose();

                OnDisplayImageChanged();
            }
            catch (Exception ex)
            {
                throw new Exception("SharedCameraView.ClearDisplayImage() : " + ex.Message, ex);
            }
        }

        private void OnDisplayImageChanged()
        {
            try
            {
                if (DisplayImageChanged != null)
                    DisplayImageChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                throw new Exception("SharedCameraView.OnDisplayImageChanged() : " + ex.Message, ex);
            }
        }

        public void Dispose()
        {
            try
            {
                ClearDisplayImage();
            }
            catch
            {
            }
        }
    }
}
