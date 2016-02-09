using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using imobileDeviceiDevice;

namespace MK.MobileDevice
{
    class iDiagnostics
    {
        public static int DIAGNOSTICS_RELAY_ACTION_FLAG_WAIT_FOR_DISCONNECT = 1;
        public static int DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_PASS = 2;
        public static int DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_FAIL = 3;

        public enum diagnostics_relay_error_t
        {
            DIAGNOSTICS_RELAY_E_SUCCESS = 0,
            DIAGNOSTICS_RELAY_E_INVALID_ARG = -1,
            DIAGNOSTICS_RELAY_E_PLIST_ERROR = -2,
            DIAGNOSTICS_RELAY_E_MUX_ERROR = -3,
            DIAGNOSTICS_RELAY_E_UNKNOWN_REQUEST = -4,
            DIAGNOSTICS_RELAY_E_UNKNOWN_ERROR = -256
        }


        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, EntryPoint = "diagnostics_relay_client_start_service", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_client_start_service(IntPtr device, out IntPtr client, out IntPtr label);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, EntryPoint = "diagnostics_relay_client_start_service", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_client_new(IntPtr device, IntPtr lockdownSvc, out IntPtr client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, EntryPoint = "diagnostics_relay_sleep", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_sleep(IntPtr client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, EntryPoint = "diagnostics_relay_request_diagnostics", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_request_diagnostics(IntPtr client, string type, out IntPtr resultPlist);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, EntryPoint = "diagnostics_relay_shutdown", CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_shutdown(IntPtr client, int flags);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_restart(IntPtr client, int flags);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern diagnostics_relay_error_t diagnostics_relay_client_free(IntPtr client);
    }
}