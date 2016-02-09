using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using imobileDeviceiDevice;

namespace MK.MobileDevice
{
    class Springboard
    {
        public static int DIAGNOSTICS_RELAY_ACTION_FLAG_WAIT_FOR_DISCONNECT = 1;
        public static int DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_PASS = 2;
        public static int DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_FAIL = 3;

        public enum sbservices_error_t
        {
            SBSERVICES_E_SUCCESS = 0,
            SBSERVICES_E_INVALID_ARG = -1,
            SBSERVICES_E_PLIST_ERROR = -2,
            SBSERVICES_E_CONN_FAILED = -3,
            SBSERVICES_E_UNKNOWN_ERROR = -256
        }

        public enum sbservices_interface_orientation_t
        {
            SBSERVICES_INTERFACE_ORIENTATION_UNKNOWN = 0,
            SBSERVICES_INTERFACE_ORIENTATION_PORTRAIT = 1,
            SBSERVICES_INTERFACE_ORIENTATION_PORTRAIT_UPSIDE_DOWN = 2,
            SBSERVICES_INTERFACE_ORIENTATION_LANDSCAPE_RIGHT = 3,
            SBSERVICES_INTERFACE_ORIENTATION_LANDSCAPE_LEFT = 4
        }


        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbservices_error_t sbservices_client_start_service(IntPtr device, out IntPtr client, out IntPtr label);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbservices_error_t sbservices_client_new(IntPtr device, IntPtr lockdownSvc, out IntPtr client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbservices_error_t sbservices_client_free(IntPtr client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbservices_error_t sbservices_get_icon_state(IntPtr client, out IntPtr resultPlist, string format_version);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbservices_error_t sbservices_set_icon_state(IntPtr client, IntPtr layoutPlist);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbservices_error_t sbservices_get_interface_orientation(IntPtr client, out sbservices_interface_orientation_t interface_orientation);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbservices_error_t sbservices_get_icon_pngdata(IntPtr client, string bundleId, out IntPtr pngdata, out ulong pngsize);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern sbservices_error_t sbservices_get_home_screen_wallpaper_pngdata(IntPtr client, out IntPtr pngdata, out int pngsize);
    }
}