using System;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using imobileDeviceiDevice;
using MK.MobileDevice;

namespace imobileDeviceiDevice
{
    class Lockdown
    {
    	
    	public static readonly string[] domains = new string[] {
    		null,
			"com.apple.disk_usage",
			"com.apple.disk_usage.factory",
			"com.apple.mobile.battery",
		/* FIXME: For some reason lockdownd segfaults on this, works sometimes though
			"com.apple.mobile.debug",. */
			"com.apple.iqagent",
			"com.apple.purplebuddy",
			"com.apple.PurpleBuddy",
			"com.apple.mobile.chaperone",
			"com.apple.mobile.third_party_termination",
			"com.apple.mobile.lockdownd",
			"com.apple.mobile.lockdown_cache",
			"com.apple.xcode.developerdomain",
			"com.apple.international",
			"com.apple.mobile.data_sync",
			"com.apple.mobile.tethered_sync",
			"com.apple.mobile.mobile_application_usage",
			"com.apple.mobile.backup",
			"com.apple.mobile.nikita",
			"com.apple.mobile.restriction",
			"com.apple.mobile.user_preferences",
			"com.apple.mobile.sync_data_class",
			"com.apple.mobile.software_behavior",
			"com.apple.mobile.iTunes.SQLMusicLibraryPostProcessCommands",
			"com.apple.mobile.iTunes.accessories",
			"com.apple.mobile.internal", /**< iOS 4.0+ */
			"com.apple.mobile.wireless_lockdown", /**< iOS 4.0+ */
			"com.apple.fairplay",
			"com.apple.iTunes",
			"com.apple.mobile.iTunes.store",
			"com.apple.mobile.iTunes"
		};

        public struct lockdownd_pair_record_t
        {
            string device_certificate;
            string host_certificate;
            string host_id;
            string system_buid;
        }

        public enum LockdownError
        {
            LOCKDOWN_E_SUCCESS = 0,
            LOCKDOWN_E_INVALID_ARG = -1,
            LOCKDOWN_E_INVALID_CONF = -2,
            LOCKDOWN_E_PLIST_ERROR = -3,
            LOCKDOWN_E_PAIRING_FAILED = -4,
            LOCKDOWN_E_SSL_ERROR = -5,
            LOCKDOWN_E_DICT_ERROR = -6,
            LOCKDOWN_E_NOT_ENOUGH_DATA = -7,
            LOCKDOWN_E_MUX_ERROR = -8,
            LOCKDOWN_E_NO_RUNNING_SESSION = -9,
            LOCKDOWN_E_INVALID_RESPONSE = -10,
            LOCKDOWN_E_MISSING_KEY = -11,
            LOCKDOWN_E_MISSING_VALUE = -12,
            LOCKDOWN_E_GET_PROHIBITED = -13,
            LOCKDOWN_E_SET_PROHIBITED = -14,
            LOCKDOWN_E_REMOVE_PROHIBITED = -15,
            LOCKDOWN_E_IMMUTABLE_VALUE = -16,
            LOCKDOWN_E_PASSWORD_PROTECTED = -17,
            LOCKDOWN_E_USER_DENIED_PAIRING = -18,
            LOCKDOWN_E_PAIRING_DIALOG_RESPONSE_PENDING = -19,
            LOCKDOWN_E_MISSING_HOST_ID = -20,
            LOCKDOWN_E_INVALID_HOST_ID = -21,
            LOCKDOWN_E_SESSION_ACTIVE = -22,
            LOCKDOWN_E_SESSION_INACTIVE = -23,
            LOCKDOWN_E_MISSING_SESSION_ID = -24,
            LOCKDOWN_E_INVALID_SESSION_ID = -25,
            LOCKDOWN_E_MISSING_SERVICE = -26,
            LOCKDOWN_E_INVALID_SERVICE = -27,
            LOCKDOWN_E_SERVICE_LIMIT = -28,
            LOCKDOWN_E_MISSING_PAIR_RECORD = -29,
            LOCKDOWN_E_SAVE_PAIR_RECORD_FAILED = -30,
            LOCKDOWN_E_INVALID_PAIR_RECORD = -31,
            LOCKDOWN_E_MISSING_ACTIVATION_RECORD = -33,
            LOCKDOWN_E_SERVICE_PROHIBITED = -34,
            LOCKDOWN_E_ESCROW_LOCKED = -35,
            LOCKDOWN_E_UNKNOWN_ERROR = -256
        }
        const string serviceIdentifier = "com.apple.mobile.installation_proxy";

        // Connect
        #region Dll Imports
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern LockdownError lockdownd_client_new_with_handshake(IntPtr devicePtr, out IntPtr lockDownClient, string label);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError lockdownd_start_service(IntPtr lockDownClient, string identifier, out IntPtr service);
        
        
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError lockdownd_deactivate (IntPtr client);
        	
        #endregion

        public static LockdownError Start(IntPtr device, out IntPtr client, out IntPtr service)
        {
            LockdownError returnCode = lockdownd_client_new_with_handshake(device, out client, "MK iToolkit");
            service = IntPtr.Zero;
            if (returnCode != LockdownError.LOCKDOWN_E_SUCCESS)
            {
                return returnCode;
            }

            else if (client == IntPtr.Zero)
            {
                return LockdownError.LOCKDOWN_E_UNKNOWN_ERROR;
            }

            returnCode = lockdownd_start_service(client, serviceIdentifier, out service);
            if (service == IntPtr.Zero)
            {
                return LockdownError.LOCKDOWN_E_UNKNOWN_ERROR;
            }

            return returnCode;
        }

        // Working With Lockdown Service
        #region Dll Imports
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError lockdownd_get_value(IntPtr lockdownClient, string domain, string key, out IntPtr value);
        
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError lockdownd_set_value(IntPtr lockdownClient, string domain, string key, IntPtr value);

        /*
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError lockdownd_pair(IntPtr lockdownClient, lockdownd_pair_record_t? pairRecord);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError lockdownd_validate_pair(IntPtr lockdownClient, lockdownd_pair_record_t? pairRecord);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError lockdownd_unpair(IntPtr lockdownClient, lockdownd_pair_record_t? pairRecord);
        */
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError lockdownd_pair(IntPtr lockdownClient, IntPtr pairRecord);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError lockdownd_validate_pair(IntPtr lockdownClient, IntPtr pairRecord);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError lockdownd_unpair(IntPtr lockdownClient, IntPtr pairRecord);

        #endregion

        public static LockdownError GetProperties(IntPtr lockdownClient, out XDocument result)
        {
            IntPtr resultPlist;
            LockdownError returnCode = lockdownd_get_value(lockdownClient, null, null, out resultPlist);

            result = new XDocument();
            if (returnCode != LockdownError.LOCKDOWN_E_SUCCESS)
            {
                return returnCode;
            }

            else if (resultPlist == IntPtr.Zero)
            {
                return LockdownError.LOCKDOWN_E_UNKNOWN_ERROR;
            }

            result = LibiMobileDevice.PlistToXml(resultPlist);
            return returnCode;
        }
        public static Dictionary<string,string> XMLToDict(XDocument xmlPlist)
        {
            IEnumerable<XElement> elements = xmlPlist.Descendants("dict");
            Dictionary<string,string> keyValues = xmlPlist.Descendants("dict")
            .SelectMany(d => d.Elements("key").Zip(d.Elements().Where(e => e.Name != "key"), (k, v) => new { Key = k, Value = v }))
            .ToDictionary(i => i.Key.Value, i => i.Value.Value);
            return keyValues;
        }
        
        
        

        // Free Clients and Services
        #region Dll Imports
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, EntryPoint = "lockdownd_client_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError FreeClient(IntPtr client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, EntryPoint = "lockdownd_service_descriptor_free", CallingConvention = CallingConvention.Cdecl)]
        public static extern LockdownError FreeService(IntPtr service);
        #endregion
    }
}
