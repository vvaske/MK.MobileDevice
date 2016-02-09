using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using imobileDeviceiDevice;

namespace imobileDeviceiDevice
{
	class HouseArrest
    {
		public enum house_arrest_error_t { 
		  HOUSE_ARREST_E_SUCCESS = 0, 
		  HOUSE_ARREST_E_INVALID_ARG = -1, 
		  HOUSE_ARREST_E_PLIST_ERROR = -2, 
		  HOUSE_ARREST_E_CONN_FAILED = -3, 
		  HOUSE_ARREST_E_INVALID_MODE = -4, 
		  HOUSE_ARREST_E_UNKNOWN_ERROR = -256 
		}
		
		public enum  	afc_error_t { 
		  AFC_E_SUCCESS = 0, 
		  AFC_E_UNKNOWN_ERROR = 1, 
		  AFC_E_OP_HEADER_INVALID = 2, 
		  AFC_E_NO_RESOURCES = 3, 
		  AFC_E_READ_ERROR = 4, 
		  AFC_E_WRITE_ERROR = 5, 
		  AFC_E_UNKNOWN_PACKET_TYPE = 6, 
		  AFC_E_INVALID_ARG = 7, 
		  AFC_E_OBJECT_NOT_FOUND = 8, 
		  AFC_E_OBJECT_IS_DIR = 9, 
		  AFC_E_PERM_DENIED = 10, 
		  AFC_E_SERVICE_NOT_CONNECTED = 11, 
		  AFC_E_OP_TIMEOUT = 12, 
		  AFC_E_TOO_MUCH_DATA = 13, 
		  AFC_E_END_OF_DATA = 14, 
		  AFC_E_OP_NOT_SUPPORTED = 15, 
		  AFC_E_OBJECT_EXISTS = 16, 
		  AFC_E_OBJECT_BUSY = 17, 
		  AFC_E_NO_SPACE_LEFT = 18, 
		  AFC_E_OP_WOULD_BLOCK = 19, 
		  AFC_E_IO_ERROR = 20, 
		  AFC_E_OP_INTERRUPTED = 21, 
		  AFC_E_OP_IN_PROGRESS = 22, 
		  AFC_E_INTERNAL_ERROR = 23, 
		  AFC_E_MUX_ERROR = 30, 
		  AFC_E_NO_MEM = 31, 
		  AFC_E_NOT_ENOUGH_DATA = 32, 
		  AFC_E_DIR_NOT_EMPTY = 33, 
		  AFC_E_FORCE_SIGNED_TYPE = -1 
		}
		
		
		[DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
		public static extern house_arrest_error_t house_arrest_client_start_service (IntPtr device, out IntPtr client, out IntPtr label);
		
		[DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
		public static extern afc_error_t afc_client_new_from_house_arrest_client (IntPtr client, out IntPtr afc_client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern house_arrest_error_t house_arrest_send_command(IntPtr client, string command, string appId);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern house_arrest_error_t house_arrest_get_result(IntPtr client, out IntPtr resultPlist);
        
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
		public static extern afc_error_t afc_client_free (IntPtr client);
		
		[DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
		public static extern afc_error_t house_arrest_client_free (IntPtr client);
	}
}