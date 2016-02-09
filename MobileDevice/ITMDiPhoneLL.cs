//
namespace MK.MobileDevice
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using MK.MobileDevice;
    using imobileDeviceiDevice;
    using System.Linq;
    using MK.Plist;
    using System.Xml;
    using System.Xml.Linq;
    

    public class iTMDiPhoneLL : iTMDiPhone
    {
        private bool connected;
        private string current_directory;
        private ITMDDeviceNotificationCallback dnc;
        ///*
        private DeviceRestoreNotificationCallback drn1;
        private DeviceRestoreNotificationCallback drn2;
        private DeviceRestoreNotificationCallback drn3;
        private DeviceRestoreNotificationCallback drn4;
        //*/
        /*
        private DeviceRestoreNotificationCallbackEx drn1;
        private DeviceRestoreNotificationCallbackEx drn2;
        private DeviceRestoreNotificationCallbackEx drn3;
        private DeviceRestoreNotificationCallbackEx drn4;
        */
        internal unsafe void* hAFC;
        internal unsafe void* hAFC_original;
        internal IntPtr AFChandle;
        internal unsafe void* hService;
        internal unsafe void* iPhoneHandle;
        internal unsafe AMRecoveryDevice RecoveryDevice;
        private static char[] path_separators = new char[] { '/' };
        private bool wasAFC2;
        static List<iDevice> Devices = new List<iDevice>();
        static llidevice mdevice;
        private bool recovery;
        private bool attachedToHost;

        public event ITMDConnectEventHandler Connect;

        public event ITMDConnectEventHandler HostAttached;

        public event ITMDConnectEventHandler HostDetached;

        public event EventHandler DfuConnect;

        public event EventHandler DfuDisconnect;

        public event ITMDConnectEventHandler Disconnect;

        public event EventHandler RecoveryModeEnter;

        public event EventHandler RecoveryModeLeave;

        #region Enumerators
        /// <summary>
        /// Enumerates the unix file types in the file structure.
        /// <term>ftFile</term>
        /// <description>Regular file</description>
        /// <term>ftDir</term>
        /// <description>Directory</description>
        /// <term>ftBlockDevice</term>
        /// <description>Block device</description>
        /// <term>ftCharDevice</term>
        /// <description>Character device</description>
        /// <term>ftFIFO</term>
        /// <description>FIFO Device</description>
        /// <term>ftLnk</term>
        /// <description>File Link</description>
        /// <term>ftMT</term>
        /// <description>File type bit mask</description>
        /// <term>ftSock</term>
        /// <description>Socket</description>
        /// </summary>
        public enum FileTypes
        {
            File = 1,
            Folder = 2,
            BlockDevice = 3,
            CharDevice = 4,
            FIFO = 5,
            Link = 6,
            FileMask = 7,
            Socket = 8,
            Unknown = 9
        }
        #endregion Enumerators

        public iTMDiPhoneLL()
        {
            this.doConstruction();            
        }
        
        private void getDeviceHandle()
        {
        	
        }
        
        public void InitLibiMobileDevice()
        {
            updateDeviceList();
        }

        private unsafe bool ConnectToPhone()
        {
            if (MobileDevice.AMDeviceConnect(this.iPhoneHandle) == 1)
            {
                throw new Exception("Phone in recovery mode, support not yet implemented");
            }
            if (MobileDevice.AMDeviceIsPaired(this.iPhoneHandle) == 0)
            {
                return false;
            }
            
            if (MobileDevice.AMDeviceValidatePairing(this.iPhoneHandle) != 0)
            {
                return false;
            }
            if (MobileDevice.AMDeviceStartSession(this.iPhoneHandle) == 1)
            {
                return false;
            }
            
            if (0 != MobileDevice.AMDeviceStartService(this.iPhoneHandle, MobileDevice.__CFStringMakeConstantString(MobileDevice.StringToCString("com.apple.afc2")), ref this.hService, null))
            {
                if (0 != MobileDevice.AMDeviceStartService(this.iPhoneHandle, MobileDevice.__CFStringMakeConstantString(MobileDevice.StringToCString("com.apple.afc")), ref this.hService, null))
                {
                    return false;
                }
            }
            else
            {
                this.wasAFC2 = true;
            }
            if (MobileDevice.AFCConnectionOpen(this.hService, 0, ref this.hAFC) != 0)
            {
                return false;
            }
            this.connected = true;
            return true;
        }
        
        public unsafe bool EnableWiFiConnection()
        {
        	long lflags;
        	MobileDevice.AMDeviceGetWirelessBuddyFlags(this.iPhoneHandle, out lflags);
        	if (lflags == 2)
        	{
        		return true;
        	}
			return 0==MobileDevice.AMDeviceSetWirelessBuddyFlags(this.iPhoneHandle, 2);
        }
        
        public unsafe bool DisableWiFiConnection()
        {
        	long lflags;
        	MobileDevice.AMDeviceGetWirelessBuddyFlags(this.iPhoneHandle, out lflags);
        	if (lflags == 0)
        	{
        		return true;
        	}
			return 0==MobileDevice.AMDeviceSetWirelessBuddyFlags(this.iPhoneHandle, 0);
        }

        public void Copy(string sourceName, string destName)
        {
        	throw new NotImplementedException("Copy not yet implemented.");
        }
		
        public void Sleep()
        {
        	iDevice id = Devices[0];
        	IntPtr currDevice;
        	string currUdid = id.Udid;
        	LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);        		
			IntPtr ldService;
            IntPtr lockdownClient;
			Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);				        				        	
			IntPtr diagService;
			IntPtr diagClient;
			iDiagnostics.diagnostics_relay_error_t dre = iDiagnostics.diagnostics_relay_client_start_service(currDevice, out diagClient, out diagService);
			iDiagnostics.diagnostics_relay_sleep(diagClient);
			iDiagnostics.diagnostics_relay_client_free(diagClient);
			Lockdown.FreeClient(lockdownClient);
			Lockdown.FreeService(ldService);
			LibiMobileDevice.idevice_free(currDevice);
        }
        
        public void ShutdownOnDisconnect()
        {
        	iDevice id = Devices[0];
        	IntPtr currDevice;
        	string currUdid = id.Udid;
        	LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);        	
			IntPtr ldService;
            IntPtr lockdownClient;
			Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);				        				        	
			IntPtr diagService;
			IntPtr diagClient;
			iDiagnostics.diagnostics_relay_error_t dre = iDiagnostics.diagnostics_relay_client_start_service(currDevice, out diagClient, out diagService);
			iDiagnostics.diagnostics_relay_shutdown(diagClient, iDiagnostics.DIAGNOSTICS_RELAY_ACTION_FLAG_WAIT_FOR_DISCONNECT);
			iDiagnostics.diagnostics_relay_client_free(diagClient);
			Lockdown.FreeClient(lockdownClient);
			Lockdown.FreeService(ldService);
			LibiMobileDevice.idevice_free(currDevice);
        }
        
        
        public void Shutdown()
        {
        	iDevice id = Devices[0];
        	IntPtr currDevice;
        	string currUdid = id.Udid;
        	LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);        	
			IntPtr ldService;
            IntPtr lockdownClient;
			Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);				        				        	
			IntPtr diagService;
			IntPtr diagClient;
			iDiagnostics.diagnostics_relay_error_t dre = iDiagnostics.diagnostics_relay_client_start_service(currDevice, out diagClient, out diagService);
			iDiagnostics.diagnostics_relay_shutdown(diagClient, iDiagnostics.DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_PASS);
			iDiagnostics.diagnostics_relay_client_free(diagClient);
			Lockdown.FreeClient(lockdownClient);
			Lockdown.FreeService(ldService);
			LibiMobileDevice.idevice_free(currDevice);
        }
        
        public void RebootOnDisconnect()
        {
        	iDevice id = Devices[0];
        	IntPtr currDevice;
        	string currUdid = id.Udid;
        	LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);        	
			IntPtr ldService;
            IntPtr lockdownClient;
			Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);				        				        	
			IntPtr diagService;
			IntPtr diagClient;
			iDiagnostics.diagnostics_relay_error_t dre = iDiagnostics.diagnostics_relay_client_start_service(currDevice, out diagClient, out diagService);
			iDiagnostics.diagnostics_relay_restart(diagClient, iDiagnostics.DIAGNOSTICS_RELAY_ACTION_FLAG_WAIT_FOR_DISCONNECT);
			iDiagnostics.diagnostics_relay_client_free(diagClient);
			Lockdown.FreeClient(lockdownClient);
			Lockdown.FreeService(ldService);
			LibiMobileDevice.idevice_free(currDevice);
        }
        
        public void Reboot()
        {
        	iDevice id = Devices[0];
        	IntPtr currDevice;
        	string currUdid = id.Udid;
        	LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);        	
			IntPtr ldService;
            IntPtr lockdownClient;
			Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);				        				        	
			IntPtr diagService;
			IntPtr diagClient;
			iDiagnostics.diagnostics_relay_error_t dre = iDiagnostics.diagnostics_relay_client_start_service(currDevice, out diagClient, out diagService);
			iDiagnostics.diagnostics_relay_restart(diagClient, iDiagnostics.DIAGNOSTICS_RELAY_ACTION_FLAG_DISPLAY_PASS);
			iDiagnostics.diagnostics_relay_client_free(diagClient);
			Lockdown.FreeClient(lockdownClient);
			Lockdown.FreeService(ldService);
			LibiMobileDevice.idevice_free(currDevice);
        }
        
        public void Deactivate()
        {
        	if (this.IsConnected)
        	{
				iDevice id = Devices[0];
	        	IntPtr currDevice;
	        	string currUdid = id.Udid;
	        	LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);        	
	        	IntPtr ldService;
	            IntPtr lockdownClient;
				Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);				        				
				Lockdown.lockdownd_deactivate(lockdownClient);
				Lockdown.FreeService(ldService);
				Lockdown.FreeClient(lockdownClient);
				LibiMobileDevice.idevice_free(currDevice);
        	}
        	else
        	{
        		throw new iPhoneException("Device not connected.");
        	}
        }
        
        public List<iOSApplication> GetApplicationList()
        {
        	iDevice id = Devices[0];
        	IntPtr currDevice;
        	string currUdid = id.Udid;
        	LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);        	
        	IntPtr ldService;
            IntPtr lockdownClient;
			Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);				        				
        	IntPtr InstProxyClient;
        	InstallationProxy.InstproxyError ipe = InstallationProxy.Connect(currDevice, ldService, out InstProxyClient);
			List<iOSApplication> appList;
			InstallationProxy.GetApplications(InstProxyClient, out appList);
			Lockdown.FreeClient(lockdownClient);
			Lockdown.FreeService(ldService);
			if (ipe==InstallationProxy.InstproxyError.INSTPROXY_E_SUCCESS)
			{
				InstallationProxy.instproxy_client_free(InstProxyClient);
				LibiMobileDevice.idevice_free(currDevice);
				return appList;
			}
			else
			{
				throw new iPhoneException("Installation Proxy encountered an error ({0})",ipe);
			}
        }
        
        public void UninstallApplication(string applicationBundleIdentifier)
        {
        	string appId=applicationBundleIdentifier;
        	iDevice id = Devices[0];
        	IntPtr currDevice;
        	string currUdid = id.Udid;
        	LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);        	
        	IntPtr ldService;
            IntPtr lockdownClient;
			Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);				        				
        	IntPtr InstProxyClient;
        	IntPtr InstProxyServer;
        	InstallationProxy.InstproxyError ipe = InstallationProxy.instproxy_client_start_service(currDevice, out InstProxyClient, out InstProxyServer);
        	ipe = InstallationProxy.instproxy_uninstall(InstProxyClient, appId, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        	InstallationProxy.instproxy_client_free(InstProxyClient);
    		Lockdown.FreeClient(lockdownClient);
    		Lockdown.FreeService(ldService);
    		LibiMobileDevice.idevice_free(currDevice);
        	if (ipe!=InstallationProxy.InstproxyError.INSTPROXY_E_SUCCESS)
        	{
        		throw new iPhoneException("Installation Proxy encountered an error ({0})",ipe);
        	}
        	
        }
        public unsafe XDocument RequestProperties (string domain)
        {
        	iDevice id = Devices[0];
        	IntPtr currDevice;
        	string currUdid = id.Udid;
        	LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);        		
			IntPtr ldService;
            IntPtr lockdownClient;
			Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);				        				        	
			if (lockdownReturnCode==Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
			{
				IntPtr resultPlist;
				Lockdown.LockdownError lderror = Lockdown.lockdownd_get_value(lockdownClient, domain, null, out resultPlist);
				XDocument xd = LibiMobileDevice.PlistToXml(resultPlist);
				Lockdown.FreeClient(lockdownClient);
				Lockdown.FreeService(ldService);
				LibiMobileDevice.idevice_free(currDevice);	
				return xd;
			}
			else
			{
				Lockdown.FreeClient(lockdownClient);
				Lockdown.FreeService(ldService);
				LibiMobileDevice.idevice_free(currDevice);	
				throw new iPhoneException("Lockdown Encountered an Error {0}",lockdownReturnCode);
			}
        }        
        
        public bool SetProperty(string domain, string key, string value)
        {
        	bool rv = false;
        	iDevice id = Devices[0];
        	IntPtr currDevice;
        	string currUdid = id.Udid;
        	LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);        		
			IntPtr ldService;
            IntPtr lockdownClient;
			Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);				        				        	
			if (lockdownReturnCode==Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
			{
				IntPtr plistString = LibiMobileDevice.plist_new_string(value);
				Lockdown.LockdownError lderror = Lockdown.lockdownd_set_value(lockdownClient, domain, key, plistString);
				if (lderror==Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
				{
					rv = true;
				}
			}
			Lockdown.FreeClient(lockdownClient);
			Lockdown.FreeService(ldService);
			LibiMobileDevice.idevice_free(currDevice);	
        	return rv;
        }
                
        public string RequestProperty(string key)
        {
        	PList pl = new PList(this.RequestProperties(null), true);
        	return Convert.ToString(pl[key]);
        }
        public string RequestProperty(string domain, string key)
        {
        	PList pl = new PList(this.RequestProperties(domain), true);
        	return Convert.ToString(pl[key]);
        }
        
        public void ConnectViaHouseArrest()
        {
        	
        }
        
        public unsafe bool CreateDirectory(string path)
        {
            string str = this.FullPath(this.current_directory, path);
            if (MobileDevice.AFCDirectoryCreate(this.hAFC, str) != 0)
            {
                return false;
            }
            return true;
        }

        public unsafe void DeleteDirectory(string path)
        {
            string str = this.FullPath(this.current_directory, path);
            if (this.IsDirectory(str))
            {
                MobileDevice.AFCRemovePath(this.hAFC, str);
            }
        }

        public void DeleteDirectory(string path, bool recursive)
        {
            if (!recursive)
            {
                this.DeleteDirectory(path);
            }
            else
            {
                string str = this.FullPath(this.current_directory, path);
                if (this.IsDirectory(str))
                {
                    this.InternalDeleteDirectory(path);
                }
            }
        }

        public unsafe void DeleteFile(string path)
        {
            string str = this.FullPath(this.current_directory, path);
            if (this.Exists(str))
            {
                MobileDevice.AFCRemovePath(this.hAFC, str);
            }
        }

        private void DfuConnectCallback(ref AMRecoveryDevice callback)
        {
            this.OnDfuConnect(new DeviceNotificationEventArgs(callback));
        }

        private void DfuDisconnectCallback(ref AMRecoveryDevice callback)
        {
            this.OnDfuDisconnect(new DeviceNotificationEventArgs(callback));
        }

        private unsafe void doConstruction()
        {
            void* voidPtr;
            this.dnc = new ITMDDeviceNotificationCallback(this.NotifyCallback);
            this.drn1 = new DeviceRestoreNotificationCallback(this.DfuConnectCallback);
            this.drn2 = new DeviceRestoreNotificationCallback(this.RecoveryConnectCallback);
            this.drn3 = new DeviceRestoreNotificationCallback(this.DfuDisconnectCallback);
            this.drn4 = new DeviceRestoreNotificationCallback(this.RecoveryDisconnectCallback);
            int num = MobileDevice.AMDeviceNotificationSubscribe(this.dnc, 0, 0, 0, out voidPtr);
            
            if (num != 0)
            {
                throw new Exception("AMDeviceNotificationSubscribe failed with error " + num);
            }
            num = MobileDevice.AMRestoreRegisterForDeviceNotifications(this.drn1, this.drn2, this.drn3, this.drn4, 0, null);
            if (num != 0)
            {
                throw new Exception("AMRestoreRegisterForDeviceNotifications failed with error " + num);
            }
            this.current_directory = "/";
        }

        public unsafe void EnterRecovery()
        {
            MobileDevice.AMDeviceEnterRecovery(this.iPhoneHandle);
            
        }

        public unsafe bool Exists(string path)
        {
            void* dict = null;
            int num = MobileDevice.AFCFileInfoOpen(this.hAFC, path, ref dict);
            if (num == 0)
            {
                MobileDevice.AFCKeyValueClose(dict);
            }
            return (num == 0);
        }

        public ulong FileSize(string path)
        {
            ulong num;
            bool flag;
            this.GetFileInfo(path, out num, out flag);
            return num;
        }

        public unsafe void ExitRecovery()
        {
            MobileDevice.AMRecoveryModeDeviceSetAutoBoot(ref this.RecoveryDevice, 1, 1, 1, 1);
            MobileDevice.AMRecoveryModeDeviceReboot(ref this.RecoveryDevice);
        }

        internal string FullPath(string path1, string path2)
        {
            string[] strArray;
            if ((path1 == null) || (path1 == string.Empty))
            {
                path1 = "/";
            }
            if ((path2 == null) || (path2 == string.Empty))
            {
                path2 = "/";
            }
            if (path2[0] == '/')
            {
                strArray = path2.Split(path_separators);
            }
            else if (path1[0] == '/')
            {
                strArray = (path1 + "/" + path2).Split(path_separators);
            }
            else
            {
                strArray = ("/" + path1 + "/" + path2).Split(path_separators);
            }
            string[] strArray2 = new string[strArray.Length];
            int count = 0;
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i] == "..")
                {
                    if (count > 0)
                    {
                        count--;
                    }
                }
                else if (!(strArray[i] == ".") && !(strArray[i] == ""))
                {
                    strArray2[count++] = strArray[i];
                }
            }
            return ("/" + string.Join("/", strArray2, 0, count));
        }

        private string Get_st_ifmt(string path)
        {
            return this.GetFileInfo(path)["st_ifmt"];
        }

        public string GetCurrentDirectory()
        {
            return this.current_directory;
        }

        public unsafe string[] GetDirectories(string path)
        {
            if (!this.IsConnected)
            {
                throw new Exception("Not connected to phone");
            }
            void* dir = null;
            string s = this.FullPath(this.CurrentDirectory, path);
            if (MobileDevice.AFCDirectoryOpen(this.hAFC, Encoding.UTF8.GetBytes(s), ref dir) != 0)
            {
                throw new Exception("Path does not exist");
            }
            string buffer = null;
            ArrayList list = new ArrayList();
            MobileDevice.AFCDirectoryRead(this.hAFC, dir, ref buffer);
            while (buffer != null)
            {
                if (((buffer != ".") && (buffer != "..")) && this.IsDirectory(this.FullPath(s, buffer)))
                {
                    list.Add(buffer);
                }
                MobileDevice.AFCDirectoryRead(this.hAFC, dir, ref buffer);
            }
            MobileDevice.AFCDirectoryClose(this.hAFC, dir);
            return (string[]) list.ToArray(typeof(string));
        }

        public string GetDirectoryRoot(string path)
        {
            return "/";
        }

        public unsafe Dictionary<string, string> GetFileInfo(string path)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            void* dict = null;
            if ((MobileDevice.AFCFileInfoOpen(this.hAFC, path, ref dict) == 0) && (dict != null))
            {
                void* voidPtr2;
                void* voidPtr3;
                while (((MobileDevice.AFCKeyValueRead(dict, out voidPtr2, out voidPtr3) == 0) && (voidPtr2 != null)) && (voidPtr3 != null))
                {
                    string key = Marshal.PtrToStringAnsi(new IntPtr(voidPtr2));
                    string str2 = Marshal.PtrToStringAnsi(new IntPtr(voidPtr3));
                    dictionary.Add(key, str2);
                }
                MobileDevice.AFCKeyValueClose(dict);
            }
            return dictionary;
        }

        /// <summary>
        /// Returns the file type (enum FileType) of the specified file or directory.
        /// </summary>
        /// <param name="path">The file or directory for which to obtain the file type.</param>
        /// <returns>Returns fileType (enum FileTypes)</returns>
        public FileTypes FileType(string path)
        {
            int size;
            FileTypes fileType;

            GetFileInfoDetails(path, out size, out fileType);
            return fileType;
        }

        public unsafe void GetFileInfo(string path, out ulong size, out bool directory)
        {
            string str;
            Dictionary<string, string> fileInfo = this.GetFileInfo(path);
            size = fileInfo.ContainsKey("st_size") ? ulong.Parse(fileInfo["st_size"]) : ((ulong) 0L);
            bool flag = false;
            directory = false;
            if (fileInfo.ContainsKey("st_ifmt") && ((str = fileInfo["st_ifmt"]) != null))
            {
                if (!(str == "S_IFDIR"))
                {
                    if (str == "S_IFLNK")
                    {
                        flag = true;
                    }
                }
                else
                {
                    directory = true;
                }
            }
            if (flag)
            {
                void* dir = null;
                if (directory = MobileDevice.AFCDirectoryOpen(this.hAFC, Encoding.UTF8.GetBytes(path), ref dir) == 0)
                {
                    MobileDevice.AFCDirectoryClose(this.hAFC, dir);
                }
            }
        }

        /// <summary>
		/// Returns the size and type of the specified file or directory.
		/// </summary>
		/// <param name="path">The file or directory for which to retrieve information.</param>
		/// <param name="size">Returns the size of the specified file or directory</param>
        /// <param name="fileType">Returns the size of the specified file or directory</param>
        public unsafe void GetFileInfoDetails(string path, out int size, out FileTypes fileType)
        {
            IntPtr data;
            IntPtr current_data;
            uint data_size;
            uint offset;
            string name;
            string value;
            int ret;

            data = IntPtr.Zero;

            size = 0;
            fileType = FileTypes.Unknown;
            ret = MobileDevice.AFCGetFileInfo(new IntPtr(hAFC), path, ref data, out data_size);
            if (ret != 0)
            {
                return;
            }

            offset = 0;
            while (offset < data_size)
            {
                current_data = new IntPtr(data.ToInt32() + offset);
                name = Marshal.PtrToStringAnsi(current_data);
                offset += (uint)name.Length + 1;

                current_data = new IntPtr(data.ToInt32() + offset);
                value = Marshal.PtrToStringAnsi(current_data);
                offset += (uint)value.Length + 1;
                switch (name)
                {
                    case "st_size": size = Int32.Parse(value); break;
                    case "st_blocks": break;
                    case "st_ifmt":
                        //S_IFBLK  File (#rtl.baseunix.stat record) mode: Block device
                        //S_IFCHR  File (#rtl.baseunix.stat record) mode: Character device
                        //S_IFDIR  File (#rtl.baseunix.stat record) mode: Directory
                        //S_IFIFO  File (#rtl.baseunix.stat record) mode: FIFO
                        //S_IFLNK  File (#rtl.baseunix.stat record) mode: Link
                        //S_IFMT   File (#rtl.baseunix.stat record) mode: File type bit mask
                        //S_IFREG  File (#rtl.baseunix.stat record) mode: Regular file
                        //S_IFSOCK File (#rtl.baseunix.stat record) mode: Socket
                        switch (value)
                        {
                            case "S_IFDIR":
                                fileType = FileTypes.Folder;
                                break;
                            case "S_IFREG":
                                fileType = FileTypes.File;
                                break;
                            case "S_IFBLK":
                                fileType = FileTypes.BlockDevice;
                                break;
                            case "S_IFCHR":
                                fileType = FileTypes.CharDevice;
                                break;
                            case "S_IFIFO":
                                fileType = FileTypes.FIFO;
                                break;
                            case "S_IFLNK":
                                fileType = FileTypes.Link;
                                break;
                            case "S_IFMT":
                                fileType = FileTypes.FileMask;
                                break;
                            case "S_IFSOCK":
                                fileType = FileTypes.Socket;
                                break;
                        }
                        break;
                }
            }

        }

        public unsafe string[] GetFiles(string path)
        {
            if (!this.connected)
            {
                throw new Exception("Not connected to phone");
            }
            string s = this.FullPath(this.current_directory, path);
            void* dir = null;
            if (MobileDevice.AFCDirectoryOpen(this.hAFC, Encoding.UTF8.GetBytes(s), ref dir) != 0)
            {
                throw new Exception("Path does not exist");
            }
            string buffer = null;
            ArrayList list = new ArrayList();
            MobileDevice.AFCDirectoryRead(this.hAFC, dir, ref buffer);
            while (buffer != null)
            {
                if (!this.IsDirectory(this.FullPath(s, buffer)))
                {
                    list.Add(buffer);
                }
                MobileDevice.AFCDirectoryRead(this.hAFC, dir, ref buffer);
            }
            MobileDevice.AFCDirectoryClose(this.hAFC, dir);
            return (string[]) list.ToArray(typeof(string));
        }


        /// <summary>
        /// Deletes the device item specified in parameter "fullname".
        /// </summary>
        /// <param name="fullName">The full directory path and name of the item to remove.</param>
        public void DeleteFromDevice(String fullName)
        {
            if (IsDirectory(fullName))
            {
                InternalDeleteDirectory(fullName);
            }
            else
            {
                DeleteFile(fullName);
            }
        }


        private void InternalDeleteDirectory(string path)
        {
            string str = this.FullPath(this.current_directory, path);
            string[] files = this.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                this.DeleteFile(str + "/" + files[i]);
            }
            files = this.GetDirectories(path);
            for (int j = 0; j < files.Length; j++)
            {
                this.InternalDeleteDirectory(str + "/" + files[j]);
            }
            this.DeleteDirectory(path);
        }

        public bool IsDirectory(string path)
        {
            ulong num;
            bool flag;
            this.GetFileInfo(path, out num, out flag);
            return flag;
        }

        public bool IsFile(string path)
        {
            return (this.Get_st_ifmt(path) == "S_IFREG");
        }

        public bool IsLink(string path)
        {
            return (this.Get_st_ifmt(path) == "S_IFLNK");
        }

        private unsafe void NotifyCallback(ref ITMDAMDeviceNotificationCallbackInfo callback)
        {
            if (callback.msg == NotificationMessage.Connected)
            {
                this.attachedToHost = true;
                this.iPhoneHandle = (void*)callback.dev;
                this.OnHostAttached(new ITMDConnectEventArgs(callback));
                if (this.ConnectToPhone())
                {
                    this.OnConnect(new ITMDConnectEventArgs(callback));
                }
            }
            else if (callback.msg == NotificationMessage.Disconnected)
            {
                this.attachedToHost = false;
                this.connected = false;
                this.OnHostDetached(new ITMDConnectEventArgs(callback));
                this.OnDisconnect(new ITMDConnectEventArgs(callback));
            }
        }

        protected void OnHostAttached(ITMDConnectEventArgs args)
        {
            ITMDConnectEventHandler hostAttached = this.HostAttached;
            if (hostAttached != null)
            {
                hostAttached(this, args);
            }
        }

        protected void OnHostDetached(ITMDConnectEventArgs args)
        {
            ITMDConnectEventHandler hostDetached = this.HostDetached;
            if (hostDetached != null)
            {
                hostDetached(this, args);
            }
        }

        protected void OnConnect(ITMDConnectEventArgs args)
        {
            ITMDConnectEventHandler connect = this.Connect;
            if (connect != null)
            {
                connect(this, args);
            }
        }

        protected void OnDfuConnect(DeviceNotificationEventArgs args)
        {
            EventHandler dfuConnect = this.DfuConnect;
            if (dfuConnect != null)
            {
                dfuConnect(this, args);
            }
        }

        protected void OnDfuDisconnect(DeviceNotificationEventArgs args)
        {
            EventHandler dfuDisconnect = this.DfuDisconnect;
            if (dfuDisconnect != null)
            {
                dfuDisconnect(this, args);
            }
        }

        protected void OnDisconnect(ITMDConnectEventArgs args)
        {
            ITMDConnectEventHandler disconnect = this.Disconnect;
            if (disconnect != null)
            {
                disconnect(this, args);
            }
        }

        protected unsafe void OnRecoveryModeEnter(DeviceNotificationEventArgs args)
        {
            EventHandler recoveryModeEnter = this.RecoveryModeEnter;
            this.RecoveryDevice = args.Device;
            if (recoveryModeEnter != null)
            {
                recoveryModeEnter(this, args);
            }
        }

        protected void OnRecoveryModeLeave(DeviceNotificationEventArgs args)
        {
            EventHandler recoveryModeLeave = this.RecoveryModeLeave;
            if (recoveryModeLeave != null)
            {
                recoveryModeLeave(this, args);
            }
        }
		
        public unsafe void EnterDFU()
        {
            MobileDevice.AMRestoreModeDeviceReboot(this.iPhoneHandle);
        }

        public unsafe void ReConnect()
        {
            MobileDevice.AFCConnectionClose(this.hAFC);
            MobileDevice.AMDeviceStopSession(this.iPhoneHandle);
            MobileDevice.AMDeviceDisconnect(this.iPhoneHandle);
            this.ConnectToPhone();
        }

        private void RecoveryConnectCallback(ref AMRecoveryDevice callback)
        {
            this.recovery = true;
            this.OnRecoveryModeEnter(new DeviceNotificationEventArgs(callback));
        }

        private void RecoveryDisconnectCallback(ref AMRecoveryDevice callback)
        {
            this.OnRecoveryModeLeave(new DeviceNotificationEventArgs(callback));
        }

        public unsafe bool Rename(string sourceName, string destName)
        {
            return (MobileDevice.AFCRenamePath(this.hAFC, this.FullPath(this.current_directory, sourceName), this.FullPath(this.current_directory, destName)) == 0);
        }

        public unsafe void sendCommandToDevice(string Command)
        {
            MobileDevice.sendCommandToDevice(this.iPhoneHandle, MobileDevice.__CFStringMakeConstantString(MobileDevice.StringToCFString(Command)), 0);
        }

        public void sendFileToDevice(string Filename)
        {
        }

        public void SetCurrentDirectory(string path)
        {
            string str = this.FullPath(this.current_directory, path);
            if (!this.IsDirectory(str))
            {
                throw new Exception("Invalid directory specified");
            }
            this.current_directory = str;
        }

        public unsafe string ActivationState
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "ActivationState");
            }
        }

        public unsafe void* AFCHandle
        {
            get
            {
                return this.hAFC;
            }
        }

        public string CurrentDirectory
        {
            get
            {
                return this.current_directory;
            }
            set
            {
                this.current_directory = value;
            }
        }

        public unsafe void* Device
        {
            get
            {
                return this.iPhoneHandle;
            }
        }

        public unsafe string DeviceBasebandBootloaderVersion
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "BasebandBootloaderVersion");
            }
        }

        public unsafe string DeviceBasebandVersion
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "BasebandVersion");
            }
        }

        public unsafe string DeviceBuildVersion
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "BuildVersion");
            }
        }

        public unsafe string DeviceFirmwareVersion
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "FirmwareVersion");
            }
        }

        public unsafe string DeviceId
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "UniqueDeviceID");
            }
        }

        public unsafe string DeviceIntegratedCircuitCardIdentity
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "IntegratedCircuitCardIdentity");
            }
        }

        public unsafe string DeviceiTunesHasConnected
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "iTunesHasConnected");
            }
        }

        public unsafe string DeviceModelNumber
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "ModelNumber");
            }
        }
        
        public unsafe string DeviceName
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "DeviceName");
            }
            set
            {
            	MobileDevice.AMDeviceSetValue(this.iPhoneHandle, 0, "DeviceName", value);
            }
        }

        public unsafe IntPtr _DeviceHandle
        {
            get
            {
                return (IntPtr)iPhoneHandle;
            }
        }

        public string ProductName
        {
            get
            {
                return iDevice.DeviceHardware[DeviceProductType];
            }
        }
        

        public unsafe string DevicePhoneNumber
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "PhoneNumber");
            }
        }

        public unsafe string DeviceProductType
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "ProductType");
            }
        }

        public unsafe string DeviceSerial
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "SerialNumber");
            }
        }

        public unsafe string DeviceSIMStatus
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "SIMStatus");
            }
        }

        public unsafe string DeviceType
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "DeviceClass");
            }
        }

        public unsafe string DeviceVersion
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "ProductVersion");
            }
        }

        public unsafe string DeviceWiFiAddress
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "WiFiAddress");
            }
        }

        public unsafe string IInternationalMobileSubscriberIdentity
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "InternationalMobileSubscriberIdentity");
            }
        }

        public unsafe string InternationalMobileEquipmentIdentity
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "InternationalMobileEquipmentIdentity");
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.connected;
            }
        }

        public bool IsHostAttached
        {
            get
            {
                return attachedToHost;
            }
        }

        public bool IsJailbreak
        {
            get
            {
                if (this.wasAFC2)
                {
                    return true;
                }
                if (!this.connected)
                {
                    return false;
                }
                return this.Exists("/Applications");
            }
        }

        public unsafe string ProductVersion
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "ih8sn0w");
            }
        }

        public unsafe string UniqueChipID
        {
            get
            {
                return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "UniqueChipID");
            }
        }
        public string[] PropertyDomains
        {
            get
            {
            	return Lockdown.domains;
            }
        }
        
        static void updateDeviceList()
        {
            
            List<iDevice> deviceList;
            LibiMobileDevice.iDeviceError getDeviceReturnCode = LibiMobileDevice.GetDeviceList(out deviceList);

            // Setting found devices "Connected" property to "true", and the others' to "false"
            foreach (iDevice currDevice in Devices)
            {
                currDevice.Connected = false;
                foreach (iDevice currDevice2 in deviceList)
                {
                    if (currDevice.Udid == currDevice2.Udid)
                    {
                        currDevice.Connected = true;
                    }
                }
            }

            // Adding new devices to the list
            List<iDevice> noDuplicatesList = deviceList.Where(x => Devices.Where(y => y.Udid == x.Udid).Count() == 0).ToList();
            if (noDuplicatesList != null)
            {
                Devices.AddRange(noDuplicatesList);
            }

            
        }
    }
}

