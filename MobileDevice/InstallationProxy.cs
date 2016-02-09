
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MK.MobileDevice;
using imobileDeviceiDevice;
using System.Xml.Linq;

namespace MK.MobileDevice
{
    class InstallationProxy
    {
        public enum InstproxyError
        {
            INSTPROXY_E_SUCCESS = 0,
            INSTPROXY_E_INVALID_ARG = -1,
            INSTPROXY_E_CONN_FAILED = -3,
            INSTPROXY_E_OP_IN_PROGRESS = -4,
            INSTPROXY_E_OP_FAILED = -5,
            INSTPROXY_E_RECEIVE_TIMEOUT = -6,
            INSTPROXY_E_ALREADY_ARCHIVED = -7,
            INSTPROXY_E_API_INTERNAL_ERROR = -8,
            INSTPROXY_E_APPLICATION_ALREADY_INSTALLED = -9,
            INSTPROXY_E_APPLICATION_MOVE_FAILED = -10,
            INSTPROXY_E_APPLICATION_SINF_CAPTURE_FAILED = -11,
            INSTPROXY_E_APPLICATION_SANDBOX_FAILED = -12,
            INSTPROXY_E_APPLICATION_VERIFICATION_FAILED = -13,
            INSTPROXY_E_ARCHIVE_DESTRUCTION_FAILED = -14,
            INSTPROXY_E_BUNDLE_VERIFICATION_FAILED = -15,
            INSTPROXY_E_CARRIER_BUNDLE_COPY_FAILED = -16,
            INSTPROXY_E_CARRIER_BUNDLE_DIRECTORY_CREATION_FAILED = -17,
            INSTPROXY_E_CARRIER_BUNDLE_MISSING_SUPPORTED_SIMS = -18,
            INSTPROXY_E_COMM_CENTER_NOTIFICATION_FAILED = -19,
            INSTPROXY_E_CONTAINER_CREATION_FAILED = -20,
            INSTPROXY_E_CONTAINER_P0WN_FAILED = -21,
            INSTPROXY_E_CONTAINER_REMOVAL_FAILED = -22,
            INSTPROXY_E_EMBEDDED_PROFILE_INSTALL_FAILED = -23,
            INSTPROXY_E_EXECUTABLE_TWIDDLE_FAILED = -24,
            INSTPROXY_E_EXISTENCE_CHECK_FAILED = -25,
            INSTPROXY_E_INSTALL_MAP_UPDATE_FAILED = -26,
            INSTPROXY_E_MANIFEST_CAPTURE_FAILED = -27,
            INSTPROXY_E_MAP_GENERATION_FAILED = -28,
            INSTPROXY_E_MISSING_BUNDLE_EXECUTABLE = -29,
            INSTPROXY_E_MISSING_BUNDLE_IDENTIFIER = -30,
            INSTPROXY_E_MISSING_BUNDLE_PATH = -31,
            INSTPROXY_E_MISSING_CONTAINER = -32,
            INSTPROXY_E_NOTIFICATION_FAILED = -33,
            INSTPROXY_E_PACKAGE_EXTRACTION_FAILED = -34,
            INSTPROXY_E_PACKAGE_INSPECTION_FAILED = -35,
            INSTPROXY_E_PACKAGE_MOVE_FAILED = -36,
            INSTPROXY_E_PATH_CONVERSION_FAILED = -37,
            INSTPROXY_E_RESTORE_CONTAINER_FAILED = -38,
            INSTPROXY_E_SEATBELT_PROFILE_REMOVAL_FAILED = -39,
            INSTPROXY_E_STAGE_CREATION_FAILED = -40,
            INSTPROXY_E_SYMLINK_FAILED = -41,
            INSTPROXY_E_UNKNOWN_COMMAND = -42,
            INSTPROXY_E_ITUNES_ARTWORK_CAPTURE_FAILED = -43,
            INSTPROXY_E_ITUNES_METADATA_CAPTURE_FAILED = -44,
            INSTPROXY_E_DEVICE_OS_VERSION_TOO_LOW = -45,
            INSTPROXY_E_DEVICE_FAMILY_NOT_SUPPORTED = -46,
            INSTPROXY_E_PACKAGE_PATCH_FAILED = -47,
            INSTPROXY_E_INCORRECT_ARCHITECTURE = -48,
            INSTPROXY_E_PLUGIN_COPY_FAILED = -49,
            INSTPROXY_E_BREADCRUMB_FAILED = -50,
            INSTPROXY_E_BREADCRUMB_UNLOCK_FAILED = -51,
            INSTPROXY_E_GEOJSON_CAPTURE_FAILED = -52,
            INSTPROXY_E_NEWSSTAND_ARTWORK_CAPTURE_FAILED = -53,
            INSTPROXY_E_MISSING_COMMAND = -54,
            INSTPROXY_E_NOT_ENTITLED = -55,
            INSTPROXY_E_MISSING_PACKAGE_PATH = -56,
            INSTPROXY_E_MISSING_CONTAINER_PATH = -57,
            INSTPROXY_E_MISSING_APPLICATION_IDENTIFIER = -58,
            INSTPROXY_E_MISSING_ATTRIBUTE_VALUE = -59,
            INSTPROXY_E_LOOKUP_FAILED = -60,
            INSTPROXY_E_DICT_CREATION_FAILED = -61,
            INSTPROXY_E_INSTALL_PROHIBITED = -62,
            INSTPROXY_E_UNINSTALL_PROHIBITED = -63,
            INSTPROXY_E_MISSING_BUNDLE_VERSION = -64,
            INSTPROXY_E_UNKNOWN_ERROR = -256
        }

        // Connect
        #region DllImports
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, EntryPoint = "instproxy_client_new", CallingConvention = CallingConvention.Cdecl)]
        public static extern InstproxyError Connect(IntPtr devicePtr, IntPtr lockdownService, out IntPtr instProxyClient);
        
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern InstproxyError instproxy_client_start_service(IntPtr device, out IntPtr client, out IntPtr server);
        #endregion

        // Working With Installation Proxy
        #region DllImports
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern InstproxyError instproxy_browse(IntPtr instProxyClient, IntPtr clientOptions, out IntPtr result);
		
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern InstproxyError instproxy_uninstall(IntPtr instProxyClient, string applicationId, IntPtr clientOptions, IntPtr nullArg, IntPtr nullArg2);
        
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern InstproxyError instproxy_install(IntPtr instProxyClient, string pkgPath, IntPtr clientOptions, IntPtr callback, IntPtr nullArg);        
  
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern InstproxyError instproxy_archive(IntPtr instProxyClient, string bundleId, IntPtr clientOptions, IntPtr callback, IntPtr zero);        

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern InstproxyError instproxy_remove_archive(IntPtr instProxyClient, string bundleId, IntPtr clientOptions, IntPtr callback, IntPtr zero);        
        
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr instproxy_client_options_new();

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void instproxy_client_options_add(IntPtr clientOptions, string key, string value, IntPtr zero);
        
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void instproxy_client_options_add(IntPtr clientOptions, string key, IntPtr value, IntPtr zero);
        #endregion

        public static InstproxyError GetApplications(IntPtr installProxyClient, out List<iOSApplication> appList)
        {
            IntPtr clientOptions = instproxy_client_options_new();
            instproxy_client_options_add(clientOptions, "ApplicationType", "Any", IntPtr.Zero);

            

            IntPtr resultPlist;
            InstproxyError returnCode = instproxy_browse(installProxyClient, clientOptions, out resultPlist);
            instproxy_client_options_free(clientOptions);

            XDocument resultXml = LibiMobileDevice.PlistToXml(resultPlist);
            appList = new List<iOSApplication>();
            if (returnCode != InstproxyError.INSTPROXY_E_SUCCESS)
            {
                return returnCode;
            }

            else if (resultPlist == IntPtr.Zero || resultXml == default(XDocument))
            {
                return InstproxyError.INSTPROXY_E_UNKNOWN_ERROR;
            }

            

            List<XElement> appElementList = resultXml.Descendants("dict").Where(x => x.Parent.Parent.Name == "plist").ToList();
            appList = new List<iOSApplication>();
            foreach (XElement currElement in appElementList)
            {
                string version = getAttribute(currElement, "CFBundleShortVersionString");
                if (version == null || version == "")
                {
                    version = getAttribute(currElement, "CFBundleVersion");
                }

                string name = getAttribute(currElement, "CFBundleName");
                string executableName = getAttribute(currElement, "CFBundleExecutable");
                if (name == null || name == "")
                {
                    name = executableName;
                }

                string type = getAttribute(currElement, "ApplicationType");
                string applicationIdentifier = getAttribute(currElement, "ApplicationIdentifier");
                string identifier = getAttribute(currElement, "CFBundleIdentifier");
				string staticsize = getAttribute(currElement, "StaticDiskUsage");
				string dynsize = getAttribute(currElement, "DynamicDiskUsage");
                iOSApplication newApp = new iOSApplication(type, name, version, identifier, executableName, applicationIdentifier, staticsize, dynsize);
                appList.Add(newApp);
            }

            return returnCode;
        }

        static string getAttribute(XElement rootElement, string key)
        {
            return rootElement.Descendants("key").Where(x => x.Value == key).Select(x => (x.NextNode as XElement).Value).FirstOrDefault();
        }

        // Free
        #region DllImports
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern InstproxyError instproxy_client_free(IntPtr instProxyClient);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        static extern InstproxyError instproxy_client_options_free(IntPtr clientOptions);
        #endregion
    }
}
