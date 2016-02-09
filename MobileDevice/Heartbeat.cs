/*

 */

using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using imobileDeviceiDevice;

namespace MK.MobileDevice
{
	class Heartbeat
    {
		public enum heartbeat_error_t { 
		  HEARTBEAT_E_SUCCESS = 0, 
		  HEARTBEAT_E_INVALID_ARG = -1, 
		  HEARTBEAT_E_PLIST_ERROR = -2, 
		  HEARTBEAT_E_MUX_ERROR = -3, 
		  HEARTBEAT_E_SSL_ERROR = -4, 
		  HEARTBEAT_E_UNKNOWN_ERROR = -256 
		}
        
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern heartbeat_error_t heartbeat_client_start_service (IntPtr device, out IntPtr client, out IntPtr label);
    	
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern heartbeat_error_t heartbeat_client_new(IntPtr device, IntPtr lockdownSvc, out IntPtr client);
    	
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern heartbeat_error_t heartbeat_client_free(IntPtr client);
        
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern heartbeat_error_t heartbeat_send (IntPtr client, IntPtr messagePlist);
        	
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        	public static extern heartbeat_error_t heartbeat_receive (IntPtr client, out IntPtr messagePlist);
        
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern heartbeat_error_t heartbeat_receive_with_timeout (IntPtr client, out IntPtr messagePlist, uint timeoutMs);
    }
}