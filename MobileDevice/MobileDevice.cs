namespace MK.MobileDevice
{
    using Microsoft.Win32;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;


    internal class MobileDevice
    {
        private static readonly DirectoryInfo ApplicationSupportDirectory = new DirectoryInfo(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Apple Inc.\Apple Mobile Device Support", "InstallDir", Environment.CurrentDirectory).ToString());
        private const string iTMDDLLPath = "iTunesMobileDevice.dll";
        private static readonly FileInfo iTunesMobileDeviceFile = new FileInfo(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Apple Inc.\Apple Mobile Device Support\", "iTunesMobileDeviceDLL", iTMDDLLPath).ToString());

        static MobileDevice()
        {
            string directoryName = iTunesMobileDeviceFile.DirectoryName;

            if (!iTunesMobileDeviceFile.Exists)
            {
                directoryName = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + @"\Apple\Mobile Device Support\bin";
            }
            Environment.SetEnvironmentVariable("Path", string.Join(";", new string[] { Environment.GetEnvironmentVariable("Path"), directoryName, ApplicationSupportDirectory.FullName }));
            bool foundITMD = File.Exists(iTMDDLLPath);
        }



        [DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void* __CFStringMakeConstantString(byte[] s);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCConnectionClose(void* conn);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCConnectionInvalidate(void* conn);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCConnectionIsValid(void* conn);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCConnectionOpen(void* handle, uint io_timeout, ref void* conn);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCDirectoryClose(void* conn, void* dir);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCDirectoryCreate(void* conn, string path);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public extern static int AFCGetFileInfo(IntPtr conn, string path, ref IntPtr buffer, out uint length);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCDirectoryOpen(void* conn, byte[] path, ref void* dir);
        public static unsafe int AFCDirectoryRead(void* conn, void* dir, ref string buffer)
        {

            void* dirent = null;

            int num = AFCDirectoryRead(conn, dir, ref dirent);
            if ((num == 0) && (dirent != null))
            {
                buffer = Marshal.PtrToStringAnsi(new IntPtr(dirent));
                return num;
            }
            buffer = null;
            return num;
        }

        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCDirectoryRead(void* conn, void* dir, ref void* dirent);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCFileInfoOpen(void* conn, string path, ref void* dict);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCFileRefClose(void* conn, long handle);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCFileRefOpen(void* conn, string path, int mode, int unknown, out long handle);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCFileRefRead(void* conn, long handle, byte[] buffer, ref uint len);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCFileRefSeek(void* conn, long handle, uint pos, uint origin);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCFileRefSetFileSize(void* conn, long handle, uint size);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCFileRefTell(void* conn, long handle, ref uint position);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCFileRefWrite(void* conn, long handle, byte[] buffer, uint len);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCFlushData(void* conn, long handle);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCKeyValueClose(void* dict);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCKeyValueRead(void* dict, out void* key, out void* val);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCRemovePath(void* conn, string path);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AFCRenamePath(void* conn, string old_path, string new_path);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMDeviceConnect(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe IntPtr AMDeviceCopyDeviceIdentifier(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMDeviceGetWirelessBuddyFlags(void* device, out long flags);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMDeviceSetWirelessBuddyFlags(void* device, long flags);

        //BatteryCurrentCapacity
        /*
         * I'm using MobileDevice.framework in an OS X application and I've been able 
* to get how charged the battery of an iOS device connected over USB is using the 
* AMDeviceCopyValue(device, domain, key) function with the arguments "com.apple.mobile.battery" for the domain and "BatteryCurrentCapacity" for the key.
* 
* Battery
 
 CycleCount: 489
 DesignCapacity: 11560
 FullChargeCapacity: 9600
 Status: Success
 BatteryCurrentCapacity: 35
 BatteryIsCharging: false
 ExternalChargeCapable: false
 ExternalConnected: true
 FullyCharged: false
 GasGaugeCapability: true
Disk Usage
 AmountDataAvailable: 13519458304
 AmountDataReserved: 209715200
 CalculateDiskUsage: OkilyDokily
 NANDInfo: 
 TotalDataAvailable: 13729173504
 TotalDataCapacity: 29198647296
 TotalDiskCapacity: 31708938240
 TotalSystemAvailable: 335544320
 TotalSystemCapacity: 2510290944
Disk Usage (Factory)
 AmountDataAvailable: 13519458304
 AmountDataReserved: 209715200
 CalculateDiskUsage: OkilyDokily
 CalendarUsage: 905216
 CameraUsage: 0
 MediaCacheUsage: 4096
 NANDInfo: 
 NotesUsage: 217088
 PhotoUsage: 0
 TotalDataAvailable: 13729173504
 TotalDataCapacity: 29198647296
 TotalDiskCapacity: 31708938240
 TotalSystemAvailable: 335544320
 TotalSystemCapacity: 2510290944
 VoicemailUsage: 0
 WebAppCacheUsage: 0
Backup Settings
 CloudBackupEnabled: true
 LastCloudBackupDate: 463987288
 LastCloudBackupTZ: PDT
 RequiresEncryption: 0
 Version: 2.0
         * */
        public static unsafe string AMDeviceCopyValue(void* device, uint unknown, string name)
        {
            void* cfstring = __CFStringMakeConstantString(StringToCString(name));
            IntPtr ptr = AMDeviceCopyValue_Int(device, unknown, cfstring);
            if (ptr != IntPtr.Zero)
            {
                byte len = Marshal.ReadByte(ptr, 8);

                bool _flag0 = len > 0;
                if (_flag0)
                {
                    return Marshal.PtrToStringAnsi(new IntPtr(ptr.ToInt64() + 9L), len);
                }
            }
            return string.Empty;
        }

        public static unsafe string AMDeviceCopyValue(void* device, string domain, string name)
        {
            void* cfstring = __CFStringMakeConstantString(StringToCString(name));
            void* domaincfstring = __CFStringMakeConstantString(StringToCString(domain));
            IntPtr ptr = AMDeviceCopyValue_Int(device, domaincfstring, cfstring);
            if (ptr != IntPtr.Zero)
            {
                byte len = Marshal.ReadByte(ptr, 8);

                bool _flag0 = len > 0;
                if (_flag0)
                {
                    //return CFStringToString(StringToCFString(Marshal.PtrToStringAnsi(new IntPtr(ptr.ToInt64() + 9L), len)));
                    //return Marshal.PtrToStringUni(new IntPtr(ptr.ToInt64()), len);
                    return Marshal.PtrToStringAnsi(new IntPtr(ptr.ToInt64() + 9L), len);
                }
            }
            return "DEVICE_E_ERROR";
            //return string.Empty;
        }


        [DllImport(iTMDDLLPath, EntryPoint = "AMDeviceCopyValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe IntPtr AMDeviceCopyValue_Int(void* device, uint unknown, void* cfstring);

        [DllImport(iTMDDLLPath, EntryPoint = "AMDeviceCopyValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe IntPtr AMDeviceCopyValue_Int(void* device, void* domaincfstring, void* cfstring);


        public static unsafe string AMDeviceSetValue(void* device, uint unknown, string name, string value)
        {
            void* cfstring = __CFStringMakeConstantString(StringToCString(name));
            void* cfvalue = __CFStringMakeConstantString(StringToCString(value));
            IntPtr ptr = AMDeviceSetValue_Int(device, unknown, cfstring, cfvalue);
            byte val = 7;
            if (ptr != IntPtr.Zero)
            {
                Marshal.WriteByte(ptr, 8, val);

            }
            return string.Empty;
        }

        [DllImport(iTMDDLLPath, EntryPoint = "AMDeviceSetValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe IntPtr AMDeviceSetValue_Int(void* device, uint unknown, void* cfstring, void* value);


        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMDeviceDisconnect(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void AMDeviceEnterRecovery(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMDeviceGetConnectionID(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMDeviceIsPaired(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMDeviceNotificationSubscribe(ITMDDeviceNotificationCallback callback, uint unused1, uint unused2, uint unused3, out void* am_device_notification_ptr);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AMDeviceSendMessage(string Message);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMDeviceStartService(void* device, void* service_name, ref void* handle, void* unknown);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMDeviceStartSession(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMDeviceStopSession(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMDeviceValidatePairing(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void AMRecoveryModeDeviceReboot(ref AMRecoveryDevice device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void AMRecoveryModeDeviceReboot(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void AMRecoveryModeDeviceSetAutoBoot(ref AMRecoveryDevice device, uint a1, uint a2, uint a3, uint a4);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern AMRecoveryDevice AMRestoreModeDeviceCreate(uint unknown0, int connection_id, uint unknown1);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void AMRestoreModeDeviceReboot(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMRestorePerformDFURestore(out AMRecoveryDevice recovery_device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AMRestoreRegisterForDeviceNotifications(DeviceRestoreNotificationCallback dfu_connect, DeviceRestoreNotificationCallback recovery_connect, DeviceRestoreNotificationCallback dfu_disconnect, DeviceRestoreNotificationCallback recovery_disconnect, uint unknown0, void* user_info);
        //[DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        //public static extern unsafe void AMDeviceReboot(void* device);
        [DllImport(iTMDDLLPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int sendCommandToDevice(void* conn, void* cfs, int block);
        internal static byte[] StringToCFString(string value)
        {
            byte[] bytes = new byte[value.Length + 10];
            bytes[4] = 140;
            bytes[5] = 7;
            bytes[6] = 1;
            bytes[8] = (byte)value.Length;
            Encoding.ASCII.GetBytes(value, 0, value.Length, bytes, 9);
            return bytes;
        }

        public static string CFStringToString(byte[] value)
        {
            return Encoding.UTF8.GetString(value, 9, value[9]);
        }

        public static byte[] StringToCString(string value)
        {
            byte[] bytes = new byte[value.Length + 1];
            Encoding.ASCII.GetBytes(value, 0, value.Length, bytes, 0);
            return bytes;
        }
    }
}

