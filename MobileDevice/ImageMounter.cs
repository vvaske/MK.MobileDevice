using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using imobileDeviceiDevice;

namespace imobileDeviceiDevice
{
    class ImageMounter
    {
        public enum mobile_image_mounter_error_t
        {
            MOBILE_IMAGE_MOUNTER_E_SUCCESS = 0,
            MOBILE_IMAGE_MOUNTER_E_INVALID_ARG = -1,
            MOBILE_IMAGE_MOUNTER_E_PLIST_ERROR = -2,
            MOBILE_IMAGE_MOUNTER_E_CONN_FAILED = -3,
            MOBILE_IMAGE_MOUNTER_E_COMMAND_FAILED = -4,
            MOBILE_IMAGE_MOUNTER_E_UNKNOWN_ERROR = -256
        }

        public enum disk_image_upload_type_t
        {
            DISK_IMAGE_UPLOAD_TYPE_AFC,
            DISK_IMAGE_UPLOAD_TYPE_UPLOAD_IMAGE
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate uint UploadImageCallback(void* buffer, uint length, void* userData);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern mobile_image_mounter_error_t mobile_image_mounter_start_service(IntPtr device, out IntPtr client, out IntPtr label);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern mobile_image_mounter_error_t mobile_image_mounter_new(IntPtr device, IntPtr service, out IntPtr client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern mobile_image_mounter_error_t mobile_image_mounter_hangup(IntPtr client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern mobile_image_mounter_error_t mobile_image_mounter_free(IntPtr client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern mobile_image_mounter_error_t mobile_image_mounter_lookup_image(IntPtr client, string imageType, out IntPtr resultPlist);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern mobile_image_mounter_error_t mobile_image_mounter_upload_image(IntPtr client, string imageType, int image_size, byte[] signature, int signature_size, Delegate upload_callback, out IntPtr userdata);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern mobile_image_mounter_error_t mobile_image_mounter_mount_image(IntPtr client, string imagePath, /*byte[]*/string signature, int signature_size, string imageType, out IntPtr resultPlist);

    }
    class Screenshot
    {
        public enum screenshotr_error_t
        {
            SCREENSHOTR_E_SUCCESS = 0,
            SCREENSHOTR_E_INVALID_ARG = -1,
            SCREENSHOTR_E_PLIST_ERROR = -2,
            SCREENSHOTR_E_MUX_ERROR = -3,
            SCREENSHOTR_E_BAD_VERSION = -4,
            SCREENSHOTR_E_UNKNOWN_ERROR = -256
        }

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern screenshotr_error_t screenshotr_client_start_service(IntPtr device, out IntPtr client, string label);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern screenshotr_error_t screenshotr_client_new(IntPtr device, IntPtr LockdownSvc, out IntPtr screenshotr_client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern screenshotr_error_t screenshotr_client_free(IntPtr screenshotr_client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern screenshotr_error_t screenshotr_take_screenshot(IntPtr screenshotr_client, out IntPtr imgData, out ulong imageSize);
    }
}