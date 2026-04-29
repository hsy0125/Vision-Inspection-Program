using System;

namespace ImageViewerWinForms
{
    public static class ViewSyncManager
    {
        private static bool _isSyncEnabled;

        public static event EventHandler SyncEnabledChanged;

        public static bool IsSyncEnabled
        {
            get { return _isSyncEnabled; }
            set
            {
                if (_isSyncEnabled == value)
                    return;

                _isSyncEnabled = value;
                if (SyncEnabledChanged != null)
                    SyncEnabledChanged(null, EventArgs.Empty);
            }
        }
    }
}
