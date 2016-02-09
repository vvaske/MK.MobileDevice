
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace imobileDeviceiDevice
{
    class LibiMobileDevice
    {
        [DllImport("kernel32.dll")]
        public static extern bool WriteFile(IntPtr hFile, IntPtr lpBuffer, int NumberOfBytesToWrite, out int lpNumberOfBytesWritten, IntPtr lpOverlapped);


        public const string LibimobiledeviceDllPath = @"libimobiledevice.dll";
        public const string LibplistDllPath = @"libplist.dll";
        public enum iDeviceError
        {
            IDEVICE_E_SUCCESS = 0,
            IDEVICE_E_INVALID_ARG = -1,
            IDEVICE_E_UNKNOWN_ERROR = -2,
            IDEVICE_E_NO_DEVICE = -3,
            IDEVICE_E_NOT_ENOUGH_DATA = -4,
            IDEVICE_E_BAD_HEADER = -5,
            IDEVICE_E_SSL_ERROR = -6
        }

        // Get Devices
        #region DllImports
        
        
        [DllImport(LibplistDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void plist_to_xml(IntPtr plist, out IntPtr xml, out int length);
		
        [DllImport(LibplistDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr plist_new_string(string PlistString);
        
        [DllImport(LibimobiledeviceDllPath, EntryPoint = "idevice_new", CallingConvention = CallingConvention.Cdecl)]
        public static extern iDeviceError NewDevice(out IntPtr devicePtr, string udid);
		
        [DllImport(LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern iDeviceError idevice_get_device_list(out IntPtr devicesPtr, out int count);

        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct AMDeviceNotificationCallbackInfo
        {
            public IntPtr dev
            {
                get
                {
                    return dev_ptr;
                }
            }
            private IntPtr dev_ptr;
            public int msg;
        }
    	
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct idevice_event_t
        {
            public IntPtr connection_type
            {
                get
                {
                    return event_type;
                }
            }
            public IntPtr UniqueDeviceID
            {
            	get
            	{
            		return udid;
            	}
            }
            public IntPtr Connection
            {
            	get
            	{
            		return connection;
            	}
            }
            private IntPtr event_type;
            private IntPtr udid;
            private IntPtr connection;
        }
    	
    	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LibIMDDeviceNotificationCallback (ref idevice_event_t callback_info);
    	
    	[DllImport(LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
    	public static extern iDeviceError idevice_event_subscribe (LibIMDDeviceNotificationCallback callback , out IntPtr userData);      
        
        
        
        
        
        
        
        
        [DllImport(LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern iDeviceError idevice_get_udid(IntPtr devicePtr, out string udid);
        #endregion

        public static iDeviceError GetDeviceList(out List<iDevice> deviceList)
        {
            List<iDevice> devices = new List<iDevice>();
            IntPtr devicesPtr;
            iDeviceError returnCode = searchForDevices(out devices, out devicesPtr);

            deviceList = new List<iDevice>();
            if (returnCode != iDeviceError.IDEVICE_E_SUCCESS)
            {
                return returnCode;
            }

            foreach (iDevice currDevice in devices)
            {
                IntPtr lockdownService;
                IntPtr lockdownClient;
                Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice.Handle, out lockdownClient, out lockdownService);

                if (lockdownReturnCode != Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
                {
                    idevice_free(currDevice.Handle);
                    continue;
                }

                XDocument deviceProperties;
                lockdownReturnCode = Lockdown.GetProperties(lockdownClient, out deviceProperties);

                if (lockdownReturnCode != Lockdown.LockdownError.LOCKDOWN_E_SUCCESS || deviceProperties == default(XDocument))
                {
                    lockdownReturnCode = Lockdown.FreeService(lockdownService);
                    lockdownReturnCode = Lockdown.FreeClient(lockdownClient);
                    idevice_free(currDevice.Handle);
                    continue;
                }

                IEnumerable<XElement> keys = deviceProperties.Descendants("dict").Descendants("key");
                deviceList.Add(new iDevice(
                    IntPtr.Zero,
                    keys.Where(x => x.Value == "UniqueDeviceID").Select(x => (x.NextNode as XElement).Value).FirstOrDefault(),
                    keys.Where(x => x.Value == "SerialNumber").Select(x => (x.NextNode as XElement).Value).FirstOrDefault(),
                    keys.Where(x => x.Value == "DeviceName").Select(x => (x.NextNode as XElement).Value).FirstOrDefault(),
                    keys.Where(x => x.Value == "ProductType").Select(x => (x.NextNode as XElement).Value).FirstOrDefault()
                    ));

                Lockdown.FreeService(lockdownService);
                Lockdown.FreeClient(lockdownClient);
                idevice_free(currDevice.Handle);
            }

            return returnCode;
        }

        static iDeviceError searchForDevices(out List<iDevice> devices, out IntPtr devicesPtr)
        {
            int count;
            iDeviceError returnCode = idevice_get_device_list(out devicesPtr, out count);

            devices = new List<iDevice>();
            if (returnCode != iDeviceError.IDEVICE_E_SUCCESS)
            {
                return returnCode;
            }

            else if (devicesPtr == IntPtr.Zero)
            {
                return iDeviceError.IDEVICE_E_UNKNOWN_ERROR;
            }

            if (Marshal.ReadInt32(devicesPtr) != 0)
            {
                string currUdid;
                int i = 0;
                while ((currUdid = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(devicesPtr, i))) != null
                    && devices.Count(x => x.Udid == currUdid) == 0)
                {
                    IntPtr currDevice;
                    returnCode = NewDevice(out currDevice, currUdid);
                    devices.Add(new iDevice(currDevice, currUdid));
                    i = i + 4;
                }

                idevice_device_list_free(devicesPtr);
            }

            return returnCode;
        }

        public static XDocument PlistToXml(IntPtr plistPtr)
        {
            IntPtr xmlPtr;
            int length;
            plist_to_xml(plistPtr, out xmlPtr, out length);

            byte[] resultBytes = new byte[length];
            Marshal.Copy(xmlPtr, resultBytes, 0, length);

            string resultString = Encoding.UTF8.GetString(resultBytes);
            XDocument resultXml;
            try
            {
                resultXml = XDocument.Parse(resultString);
            }

            catch
            {
                resultXml = new XDocument();
            }

            return resultXml;
        }
        public static unsafe List<string> PtrToStringList(IntPtr listPtr, int skip)
        {
            List<string> stringList = new List<string>();
            if (Marshal.ReadInt32(listPtr) != 0)
            {
                string currString;
                int i = skip * 4;
                while ((currString = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(listPtr, i))) != null)
                {
                    stringList.Add(currString);
                    i = i + 4;
                }
            }

            return stringList;
        }

        // Free Devices
        #region Dll Imports
        [DllImport(LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern iDeviceError idevice_device_list_free(IntPtr devicesPtr);

        [DllImport(LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern iDeviceError idevice_free(IntPtr devicePtr);
        #endregion
    }
}
