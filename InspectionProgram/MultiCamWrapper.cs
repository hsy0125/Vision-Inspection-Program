using System;
using System.Runtime.InteropServices;

namespace InspectionProgram.Common
{
    /// <summary>
    /// Class to expose the MultiCam C API in .NET (P/Invoke) for Grablink Full Boards.
    /// Extracted from legacy MultiCam.cs
    /// </summary>
    public sealed class MC
    {
        #region Native Methods
        class NativeMethods
        {
            private NativeMethods() { }

            [DllImport("MultiCam.dll")]
            internal static extern Int32 McOpenDriver(IntPtr instanceName);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McCloseDriver();
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McCreate(UInt32 modelInstance, out UInt32 instance);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McCreateNm(String modelName, out UInt32 instance);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McDelete(UInt32 instance);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McSetParamInt(UInt32 instance, UInt32 parameterId, Int32 value);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McSetParamNmInt(UInt32 instance, String parameterName, Int32 value);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McSetParamStr(UInt32 instance, UInt32 parameterId, String value);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McSetParamNmStr(UInt32 instance, String parameterName, String value);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McGetParamInt(UInt32 instance, UInt32 parameterId, out Int32 value);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McGetParamNmInt(UInt32 instance, String parameterName, out Int32 value);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McGetParamNmPtr(UInt32 instance, String parameterName, out IntPtr value);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McGetParamStr(UInt32 instance, UInt32 parameterId, IntPtr value, UInt32 maxLength);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McGetParamNmStr(UInt32 instance, String parameterName, IntPtr value, UInt32 maxLength);
            [DllImport("MultiCam.dll")]
            internal static extern Int32 McRegisterCallback(UInt32 instance, CALLBACK callbackFunction, UInt32 context);
        }
        #endregion

        #region Private Constants
        private const Int32 MAX_VALUE_LENGTH = 1024;
        #endregion

        #region Default object instance Constants
        public const UInt32 CONFIGURATION = 0x20000000;
        public const UInt32 BOARD = 0xE0000000;
        public const UInt32 CHANNEL = 0x8000FFFF;
        #endregion

        #region Specific parameter values Constants
        public const Int32 INFINITE = -1;
        public const Int32 INDETERMINATE = -1;
        public const Int32 DISABLE = 0;
        #endregion

        #region Signal handling Constants
        public const UInt32 SignalEnable = (24 << 14);

        public const Int32 SIG_ANY = 0;
        public const Int32 SIG_SURFACE_PROCESSING = 1;
        public const Int32 SIG_SURFACE_FILLED = 2;
        public const Int32 SIG_UNRECOVERABLE_OVERRUN = 3;
        public const Int32 SIG_FRAMETRIGGER_VIOLATION = 4;
        public const Int32 SIG_START_EXPOSURE = 5;
        public const Int32 SIG_END_EXPOSURE = 6;
        public const Int32 SIG_ACQUISITION_FAILURE = 7;
        #endregion

        #region Signal handling Type Definitions
        public delegate void CALLBACK(ref SIGNALINFO signalInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct SIGNALINFO
        {
            public IntPtr Context;
            public UInt32 Instance;
            public Int32 Signal;
            public UInt32 SignalInfo;
            public UInt32 SignalContext;
        };
        #endregion

        #region Constructors
        private MC() { }
        #endregion

        #region Error handling Methods
        private static String GetErrorMessage(Int32 errorCode)
        {
            const UInt32 ErrorDesc = (98 << 14);
            String errorDescription;
            UInt32 status = (UInt32)Math.Abs(errorCode);
            IntPtr text = Marshal.AllocHGlobal(MAX_VALUE_LENGTH + 1);
            if (NativeMethods.McGetParamStr(CONFIGURATION, ErrorDesc + status, text, MAX_VALUE_LENGTH) != 0)
                errorDescription = "Unknown error";
            else
                errorDescription = Marshal.PtrToStringAnsi(text);
            Marshal.FreeHGlobal(text);
            return errorDescription;
        }

        private static void ThrowOnMultiCamError(Int32 status, String action)
        {
            if (status != 0)
            {
                String error = action + ": " + GetErrorMessage(status);
                throw new Exception(error);
            }
        }
        #endregion

        #region Driver connection Methods
        public static void OpenDriver()
        {
            ThrowOnMultiCamError(NativeMethods.McOpenDriver((IntPtr)null), "Cannot open MultiCam driver");
        }

        public static void CloseDriver()
        {
            ThrowOnMultiCamError(NativeMethods.McCloseDriver(), "Cannot close MultiCam driver");
        }
        #endregion

        #region Object creation/deletion Methods
        public static void Create(String modelName, out UInt32 instance)
        {
            ThrowOnMultiCamError(NativeMethods.McCreateNm(modelName, out instance), String.Format("Cannot create '{0}' instance", modelName));
        }

        public static void Delete(UInt32 instance)
        {
            ThrowOnMultiCamError(NativeMethods.McDelete(instance), String.Format("Cannot delete '{0}' instance", instance));
        }
        #endregion

        #region Parameter 'setter' Methods
        public static void SetParam(UInt32 instance, String parameterName, Int32 value)
        {
            ThrowOnMultiCamError(NativeMethods.McSetParamNmInt(instance, parameterName, value), String.Format("Cannot set param '{0}'", parameterName));
        }

        public static void SetParam(UInt32 instance, String parameterName, String value)
        {
            ThrowOnMultiCamError(NativeMethods.McSetParamNmStr(instance, parameterName, value), String.Format("Cannot set param '{0}'", parameterName));
        }
        
        public static void SetParam(UInt32 instance, UInt32 parameterId, String value)
        {
            ThrowOnMultiCamError(NativeMethods.McSetParamStr(instance, parameterId, value), String.Format("Cannot set param '{0}'", parameterId));
        }
        #endregion

        #region Parameter 'getter' Methods
        public static void GetParam(UInt32 instance, String parameterName, out Int32 value)
        {
            ThrowOnMultiCamError(NativeMethods.McGetParamNmInt(instance, parameterName, out value), String.Format("Cannot get param '{0}'", parameterName));
        }

        public static void GetParam(UInt32 instance, String parameterName, out IntPtr value)
        {
            ThrowOnMultiCamError(NativeMethods.McGetParamNmPtr(instance, parameterName, out value), String.Format("Cannot get param '{0}'", parameterName));
        }

        public static void GetParam(UInt32 instance, String parameterName, out String value)
        {
            IntPtr text = Marshal.AllocHGlobal(MAX_VALUE_LENGTH + 1);
            try
            {
                ThrowOnMultiCamError(NativeMethods.McGetParamNmStr(instance, parameterName, text, MAX_VALUE_LENGTH), String.Format("Cannot get param '{0}'", parameterName));
                value = Marshal.PtrToStringAnsi(text);
            }
            finally
            {
                Marshal.FreeHGlobal(text);
            }
        }
        #endregion

        #region Signal handling Methods
        public static void RegisterCallback(UInt32 instance, CALLBACK callbackFunction, UInt32 context)
        {
            ThrowOnMultiCamError(NativeMethods.McRegisterCallback(instance, callbackFunction, context), "Cannot register callback");
        }
        #endregion
    }
}
