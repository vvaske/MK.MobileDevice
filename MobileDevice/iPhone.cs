//dd
using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.ExceptionServices;
using System.Text;
using MK.MobileDevice;
using imobileDeviceiDevice;
using System.Linq;
using MK.Plist;
using System.Xml;
using System.Security;
using System.Xml.Linq;
using recoverysaver;
using rebootpwn;
namespace MK.MobileDevice
{
    public class iPhone
    {
        private bool connected;
        private string current_directory;
        private LibiMobileDevice.LibIMDDeviceNotificationCallback dnc;
        //private DeviceRestoreNotificationCallback drn1;
        //private DeviceRestoreNotificationCallback drn2;
        //private DeviceRestoreNotificationCallback drn3;
        //private DeviceRestoreNotificationCallback drn4;
        IntPtr afcSvc;
        IntPtr afc2Svc;
        internal unsafe IntPtr hAFC;
        internal unsafe IntPtr hAFC_original;
        internal IntPtr AFChandle;
        internal unsafe IntPtr hService;
        internal IntPtr iPhoneHandle;
        private static char[] path_separators = new char[] { '/' };
        private bool wasAFC2;
        static List<iDevice> Devices = new List<iDevice>();
        static llidevice mdevice;
        string lastError;

        LibiMobileDevice.LibIMDDeviceNotificationCallback IMDDeviceSubscribeCallbackDelegate;
        private bool developer;
        public bool attachedToHost;

        public event ConnectEventHandler Connect;

        public event DeviceAttachedHandler HostAttached;

        public event DeviceAttachedHandler HostDisconnected;

        public event EventHandler DfuConnect;

        public event EventHandler DfuDisconnect;

        public event ConnectEventHandler Disconnect;

        public event EventHandler RecoveryModeEnter;

        public event EventHandler RecoveryModeLeave;

        public iPhone()
        {
            this.doConstruction();
        }

        private void getDeviceHandle()
        {

        }

        public string getLastError()
        {
            return lastError;
        }

        public void InitLibiMobileDevice()
        {
            updateDeviceList();
        }
        /*
        public unsafe void EnterRecovery()
        {
            //MobileDevice.AMDeviceEnterRecovery(this.iPhoneHandle);
            rebootpwn rbp = librecoverysaver.HijackIDevice();
            librecoverysaver.EnterRecovery(rbp);
        }

        public unsafe void ExitRecovery()
        {
            rebootpwn rbp = librecoverysaver.HijackIDevice();
            librecoverysaver.ExitRecovery(rbp);
        }
        */

        public int Pair()
        {
            IntPtr currDevice = this.iPhoneHandle;
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lde = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            if (lde != Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
            {
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                return (int)lde;
            }
            lde = Lockdown.lockdownd_pair(lockdownClient, IntPtr.Zero);
            Lockdown.FreeClient(lockdownClient);
            Lockdown.FreeService(ldService);
            return (int)lde;
        }

        public int Unpair()
        {
            IntPtr currDevice = this.iPhoneHandle;
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lde = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            if (lde != Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
            {
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                return (int)lde;
            }
            lde = Lockdown.lockdownd_unpair(lockdownClient, IntPtr.Zero);
            Lockdown.FreeClient(lockdownClient);
            Lockdown.FreeService(ldService);
            return (int)lde;
        }

        public int ValidatePairing()
        {
            IntPtr currDevice = this.iPhoneHandle;
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lde = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            if (lde != Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
            {
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                return (int)lde;
            }
            lde = Lockdown.lockdownd_validate_pair(lockdownClient, IntPtr.Zero);
            Lockdown.FreeClient(lockdownClient);
            Lockdown.FreeService(ldService);
            return (int)lde;
        }

        private unsafe bool ConnectToPhone()
        {
            string UDID;
            updateDeviceList();
            int numDev = Devices.Count;
            if (numDev == 0)
            {
                return false;
            }
            iDevice idv = Devices[0];
            this.iPhoneHandle = idv.Handle;
            UDID = idv.Udid;
            LibiMobileDevice.NewDevice(out this.iPhoneHandle, UDID);
            /*
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
            */
            /*
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
            */

            //Start AFC/AFC2
            IntPtr ldSvc;
            IntPtr ldCli;
            Lockdown.LockdownError lde = Lockdown.Start(this.iPhoneHandle, out ldCli, out ldSvc);

            lde = Lockdown.lockdownd_start_service(ldCli, "com.apple.afc2", out this.afc2Svc);
            if (lde != Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
            {
                lde = Lockdown.lockdownd_start_service(ldCli, "com.apple.afc", out this.afcSvc);
                if (lde != Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
                {
                    return false;
                }
            }
            else
            {
                this.wasAFC2 = true;
            }

            var afce = AFC.afc_client_start_service(this.iPhoneHandle, out this.hAFC, "MK.MobileDevice");
            Lockdown.FreeClient(ldCli);
            Lockdown.FreeService(ldSvc);
            this.hAFC_original = this.hAFC;
            this.connected = true;
            return true;
        }

        public void Copy(string sourceName, string destName)
        {
            throw new NotImplementedException("Copy not yet implemented.");
        }

        public bool Base64Decode(string input, out string result)
        {
            try
            {
                byte[] data = Convert.FromBase64String(input);
                result = Encoding.UTF8.GetString(data);
                return true;
            }
            catch
            {
                result = "";
                return false;
            }
        }

        public PList RequestDiagnostics()
        {
            IntPtr currDevice = this.iPhoneHandle;
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            IntPtr diagClient;
            IntPtr diagService;
            iDiagnostics.diagnostics_relay_error_t ide = iDiagnostics.diagnostics_relay_client_start_service(currDevice, out diagClient, out diagService);
            IntPtr resultPlist;
            ide = iDiagnostics.diagnostics_relay_request_diagnostics(diagClient, "All", out resultPlist);
            XDocument xd = LibiMobileDevice.PlistToXml(resultPlist);
            PList pl = new PList(xd, true);
            iDiagnostics.diagnostics_relay_client_free(diagClient);
            Lockdown.FreeClient(lockdownClient);
            Lockdown.FreeService(ldService);
            return pl;
        }

        public Dictionary<string, dynamic> RequestBatteryInfo()
        {
            Dictionary<string, dynamic> battInfo = new Dictionary<string, dynamic>();
            PList gasGaugeInfo = RequestDiagnostics()["GasGauge"];
            battInfo = gasGaugeInfo;
            PList battDomainInfo = new PList(RequestProperties("com.apple.mobile.battery"), true);
            battDomainInfo.ToList().ForEach(x => battInfo.Add(x.Key, x.Value));
            return gasGaugeInfo;
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
                this.lastError = "Device not connected.";
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
            if (ipe == InstallationProxy.InstproxyError.INSTPROXY_E_SUCCESS)
            {
                InstallationProxy.instproxy_client_free(InstProxyClient);
                LibiMobileDevice.idevice_free(currDevice);
                return appList;
            }
            else
            {
                throw new iPhoneException("Installation Proxy encountered an error ({0})", ipe);
            }
        }

        public string ArchiveApplicationOnDevice(string bundleId)
        {
            string arcPath = null;
            iDevice id = Devices[0];
            IntPtr currDevice;
            string currUdid = id.Udid;
            //Console.Write("Opening new device handle...");
            LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);
            //Console.WriteLine(returnCode);
            IntPtr ldService;
            IntPtr lockdownClient;
            //Console.Write("Opening new lockdown handle...");
            Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            //Console.WriteLine(lockdownReturnCode);
            //Console.Write("Starting Installation Proxy...");
            IntPtr ipxClient;
            IntPtr ipxSvc;
            var ipe = InstallationProxy.instproxy_client_start_service(currDevice, out ipxClient, out ipxSvc);
            //Console.WriteLine(ipe);
            string APPARCH_PATH = "ApplicationArchives";
            //Console.Write("Preparing AFC...");
            IntPtr afcC;
            var afce = AFC.afc_client_start_service(currDevice, out afcC, "MK-iMD");
            //Console.WriteLine(afce);
            //Console.Write("Querying AFC...");
            IntPtr afcInfo;
            afce = AFC.afc_get_file_info(afcC, APPARCH_PATH, out afcInfo);
            //Console.WriteLine(afce);
            if (afce == AFC.AFCError.AFC_E_OBJECT_NOT_FOUND)
            {
                //Console.Write("Creating Directory...");
                afce = AFC.afc_make_directory(afcC, APPARCH_PATH);
                //Console.WriteLine(afce);
            }

            IntPtr client_opts = InstallationProxy.instproxy_client_options_new();
            InstallationProxy.instproxy_client_options_add(client_opts, "ArchiveType", "ApplicationOnly", IntPtr.Zero);
            string outputFn = APPARCH_PATH + "/" + bundleId + ".zip";
            //Console.Write("Archiving...");
            ipe = InstallationProxy.instproxy_archive(ipxClient, bundleId, client_opts, IntPtr.Zero, IntPtr.Zero);
            //Console.WriteLine(ipe);
            if (ipe == InstallationProxy.InstproxyError.INSTPROXY_E_SUCCESS)
                arcPath = outputFn;
            Lockdown.FreeClient(lockdownClient);
            LibiMobileDevice.idevice_free(currDevice);
            return arcPath;
        }

        public void RemoveApplicationArchive(string bundleId)
        {
            iDevice id = Devices[0];
            IntPtr currDevice;
            string currUdid = id.Udid;
            //Console.Write("Opening new device handle...");
            LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);
            //Console.WriteLine(returnCode);
            IntPtr ldService;
            IntPtr lockdownClient;
            //Console.Write("Opening new lockdown handle...");
            Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            //Console.WriteLine(lockdownReturnCode);
            //Console.Write("Starting Installation Proxy...");
            IntPtr ipxClient;
            IntPtr ipxSvc;
            var ipe = InstallationProxy.instproxy_client_start_service(currDevice, out ipxClient, out ipxSvc);
            //Console.WriteLine(ipe);
            IntPtr r_client_opts = InstallationProxy.instproxy_client_options_new();
            //Console.Write("Removing Archive...");
            ipe = InstallationProxy.instproxy_archive(ipxClient, bundleId, r_client_opts, IntPtr.Zero, IntPtr.Zero);
            //Console.WriteLine(ipe);
            Lockdown.FreeClient(lockdownClient);
            LibiMobileDevice.idevice_free(currDevice);
        }

        public bool InstallApplication(string ipaFile)
        {
            iDevice id = Devices[0];
            IntPtr currDevice;
            string currUdid = id.Udid;
            //Console.Write("Opening new device handle...");
            LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);
            //Console.WriteLine(returnCode);
            IntPtr ldService;
            IntPtr lockdownClient;
            //Console.Write("Opening new lockdown handle...");
            Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            //Console.WriteLine(lockdownReturnCode);
            //Console.Write("Starting Installation Proxy...");
            IntPtr ipxClient;
            IntPtr ipxSvc;
            var ipe = InstallationProxy.instproxy_client_start_service(currDevice, out ipxClient, out ipxSvc);
            //Console.WriteLine(ipe);

            string PKG_PATH = "PublicStaging";
            string APPARCH_PATH = "ApplicationArchives";

            //Console.Write("Preparing AFC...");
            IntPtr afcC;
            var afce = AFC.afc_client_start_service(currDevice, out afcC, "MK-iMD");
            //Console.WriteLine(afce);
            //Console.Write("Querying AFC...");
            IntPtr afcInfo;
            afce = AFC.afc_get_file_info(afcC, PKG_PATH, out afcInfo);
            //Console.WriteLine(afce);
            if (afce == AFC.AFCError.AFC_E_OBJECT_NOT_FOUND)
            {
                //Console.Write("Creating Directory...");
                afce = AFC.afc_make_directory(afcC, PKG_PATH);
                //Console.WriteLine(afce);
            }

            IntPtr clientOpts = InstallationProxy.instproxy_client_options_new();
            int errp = 0;
            var ipa = File.Open(ipaFile, FileMode.Open, FileAccess.ReadWrite);
            var zarc = new System.IO.Compression.ZipArchive(ipa);
            string appName = zarc.Entries.Where(zae => zae.FullName.EndsWith(".app/")).ToList()[0].FullName;
            //Console.WriteLine("Found {0} in IPA archive",appName);
            string infoPlistPath = appName + "Info.plist";

            var iPlsSt = ReadBytesOfStream(zarc.GetEntry(infoPlistPath).Open());
            var infoPlistPtr = LibPlist.GetPtrPlistFromData(iPlsSt);

            var infoPlistXdoc = LibiMobileDevice.PlistToXml(infoPlistPtr);
            var infoPlist = new PList(infoPlistXdoc, true);
            //get data

            string bundleX = infoPlist["CFBundleExecutable"];
            string bundleid = infoPlist["CFBundleIdentifier"];

            //Console.WriteLine("Found {0} iOS binary in app {1}",bundleX, bundleid);

            string sinfname = "Payload/" + bundleX + ".app/SC_Info/" + bundleid + ".sinf";
            var sinfS = zarc.GetEntry(sinfname);
            bool hasSinf = sinfS != null;
            if (hasSinf)
            {
                var sinfSt = sinfS.Open();
                var sinf = LibPlist.plist_new_data(ReadBytesOfStream(sinfSt), (ulong)sinfSt.Length);
                //Console.WriteLine("Found SINF inside app.");
            }
            IntPtr meta = IntPtr.Zero; ;
            var iMTD = zarc.GetEntry("Payload/iTunesMetadata.plist");
            bool hasiMTD = iMTD != null;
            if (hasiMTD)
            {
                var fSt = iMTD.Open();
                meta = LibPlist.plist_new_data(ReadBytesOfStream(fSt), (ulong)fSt.Length);
            }
            //copy archive
            zarc.Dispose();
            //Console.Write("Uploading IPA...");
            string pkgname = PKG_PATH + "/" + bundleid;
            AFC.copyToDevice(afcC, ipaFile, pkgname);
            //Console.WriteLine("Done.");

            InstallationProxy.instproxy_client_options_add(clientOpts, "CFBundleIdentifier", bundleid, IntPtr.Zero);
            if (hasiMTD)
                InstallationProxy.instproxy_client_options_add(clientOpts, "iTunesMetadata", meta, IntPtr.Zero);
            //Console.Write("Attempting to install...");
            ipe = InstallationProxy.instproxy_install(ipxClient, pkgname, clientOpts, IntPtr.Zero, IntPtr.Zero);
            //Console.WriteLine(ipe);
            Lockdown.FreeClient(lockdownClient);
            return ipe == InstallationProxy.InstproxyError.INSTPROXY_E_SUCCESS;
        }

        public void UninstallApplication(string applicationBundleIdentifier)
        {
            string appId = applicationBundleIdentifier;
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
            if (ipe != InstallationProxy.InstproxyError.INSTPROXY_E_SUCCESS)
            {
                throw new iPhoneException("Installation Proxy encountered an error ({0})", ipe);
            }
        }
        public unsafe XDocument RequestProperties(string domain)
        {
            iDevice id = Devices[0];
            IntPtr currDevice;
            string currUdid = id.Udid;
            LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            if (lockdownReturnCode == Lockdown.LockdownError.LOCKDOWN_E_SUCCESS || IsWifiConnect)
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
                throw new iPhoneException("Lockdown Encountered an Error {0}", lockdownReturnCode);
            }
        }

        public bool SaveScreenshot(string filename)
        {
            bool ret = false;
            if (!QueryDeveloperDisk())
                return false;
            IntPtr currDevice = this.iPhoneHandle;
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lde = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            IntPtr scrClient;
            IntPtr scrSvc;
            lde = Lockdown.lockdownd_start_service(lockdownClient, "com.apple.mobile.screenshotr", out scrSvc);
            if (lde == Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
            {
                bool didwrite = false;
                Screenshot.screenshotr_error_t sce = Screenshot.screenshotr_client_start_service(currDevice, out scrClient, "MK.MobileDevice");
                IntPtr imgData;
                ulong imgSize;
                sce = Screenshot.screenshotr_take_screenshot(scrClient, out imgData, out imgSize);
                if (sce == Screenshot.screenshotr_error_t.SCREENSHOTR_E_SUCCESS)
                {
                    System.IO.FileStream file = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                    int written;
                    didwrite = LibiMobileDevice.WriteFile(file.Handle, imgData, (int)imgSize, out written, IntPtr.Zero);
                    file.Close();
                }
                Screenshot.screenshotr_client_free(scrClient);
                ret = didwrite;
            }
            Lockdown.FreeClient(lockdownClient);
            Lockdown.FreeService(ldService);
            return ret;
        }

        public bool PingDevice()
        {
            bool ret = false;
            IntPtr currDevice = this.iPhoneHandle;
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lde = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            if (lde != Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
            {
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                return ret;
            }
            IntPtr hbClient;
            IntPtr hbSvc;
            Heartbeat.heartbeat_error_t hbe = Heartbeat.heartbeat_client_start_service(currDevice, out hbClient, out hbSvc);
            if (hbe != Heartbeat.heartbeat_error_t.HEARTBEAT_E_SUCCESS)
            {
                Heartbeat.heartbeat_client_free(hbClient);
                return ret;
            }
            IntPtr msg;
            IntPtr resultPlist;
            hbe = Heartbeat.heartbeat_receive_with_timeout(hbClient, out resultPlist, 60);
            if (hbe != Heartbeat.heartbeat_error_t.HEARTBEAT_E_SUCCESS)
            {
                Heartbeat.heartbeat_client_free(hbClient);
                return ret;
            }
            IntPtr reply;
            reply = LibPlist.plist_new_dict();
            LibPlist.plist_dict_insert_item(reply, "Command", LibPlist.plist_new_string("Polo"));
            hbe = Heartbeat.heartbeat_send(hbClient, reply);
            if (hbe != Heartbeat.heartbeat_error_t.HEARTBEAT_E_SUCCESS)
            {
                Heartbeat.heartbeat_client_free(hbClient);
                return ret;
            }
            Heartbeat.heartbeat_client_free(hbClient);
            ret = true;
            Lockdown.FreeClient(lockdownClient);
            Lockdown.FreeService(ldService);
            return ret;
        }

        public XDocument GetIconLayout()
        {
            XDocument ret = null;
            IntPtr currDevice = this.iPhoneHandle;
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lde = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            if (lde != Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
            {
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                return ret;
            }
            IntPtr sbClient;
            IntPtr sbSrv;
            Springboard.sbservices_error_t sbe = Springboard.sbservices_client_start_service(currDevice, out sbClient, out sbSrv);
            if (sbe != Springboard.sbservices_error_t.SBSERVICES_E_SUCCESS)
            {
                Springboard.sbservices_client_free(sbClient);
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                return ret;
            }
            IntPtr layoutPlist;
            sbe = Springboard.sbservices_get_icon_state(sbClient, out layoutPlist, "2");
            if (sbe != Springboard.sbservices_error_t.SBSERVICES_E_SUCCESS)
            {
                Springboard.sbservices_client_free(sbClient);
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                return ret;
            }
            XDocument layout = LibiMobileDevice.PlistToXml(layoutPlist);
            ret = layout;
            Springboard.sbservices_client_free(sbClient);
            Lockdown.FreeClient(lockdownClient);
            Lockdown.FreeService(ldService);
            return ret;
        }

        public bool SetIconLayout(IntPtr layoutPlist)
        {
            bool ret = false;
            IntPtr currDevice = this.iPhoneHandle;
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lde = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            if (lde != Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
            {
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                return ret;
            }
            IntPtr sbClient;
            IntPtr sbSrv;
            Springboard.sbservices_error_t sbe = Springboard.sbservices_client_start_service(currDevice, out sbClient, out sbSrv);
            if (sbe != Springboard.sbservices_error_t.SBSERVICES_E_SUCCESS)
            {
                Springboard.sbservices_client_free(sbClient);
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                return ret;
            }
            sbe = Springboard.sbservices_set_icon_state(sbClient, layoutPlist);
            if (sbe != Springboard.sbservices_error_t.SBSERVICES_E_SUCCESS)
            {
                Springboard.sbservices_client_free(sbClient);
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                return ret;
            }
            Springboard.sbservices_client_free(sbClient);
            Lockdown.FreeClient(lockdownClient);
            Lockdown.FreeService(ldService);
            return ret;
        }

        public bool QueryDeveloperDisk()
        {
            bool ret = false;
            this.developer = false;
            IntPtr currDevice = this.iPhoneHandle;
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            if (!(lockdownReturnCode == Lockdown.LockdownError.LOCKDOWN_E_SUCCESS))
                return false;
            IntPtr ddservice;
            IntPtr ddclient;
            Lockdown.LockdownError lde = Lockdown.lockdownd_start_service(lockdownClient, "com.apple.mobile.mobile_image_mounter", out ddservice);
            if (!(lde == Lockdown.LockdownError.LOCKDOWN_E_SUCCESS))
                return false;
            ImageMounter.mobile_image_mounter_error_t dde = ImageMounter.mobile_image_mounter_new(currDevice, ddservice, out ddclient);
            if (!(dde == ImageMounter.mobile_image_mounter_error_t.MOBILE_IMAGE_MOUNTER_E_SUCCESS))
                return false;
            string imageType = "Developer";
            IntPtr resultPlist;
            dde = ImageMounter.mobile_image_mounter_lookup_image(ddclient, imageType, out resultPlist);
            if (!(dde == ImageMounter.mobile_image_mounter_error_t.MOBILE_IMAGE_MOUNTER_E_SUCCESS))
                return false;
            ImageMounter.mobile_image_mounter_free(ddclient);
            Lockdown.FreeClient(lockdownClient);
            Lockdown.FreeService(ldService);
            XDocument xd = LibiMobileDevice.PlistToXml(resultPlist);
            PList pl = new PList(xd, true);
            this.developer = ret;
            ret = pl.ContainsKey("ImagePresent") && pl["ImagePresent"] == true;
            return ret;
        }

        public bool MountDeveloperDisk(string DeveloperImagePath)
        {
            string mnt = "mnt.exe";
            bool allFilesPresent = (System.IO.File.Exists(DeveloperImagePath) && System.IO.File.Exists(DeveloperImagePath + ".signature"));
            if (!allFilesPresent)
                return false;
            System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("MobileDevice.mntddi.exe");
            System.IO.FileStream fs = new System.IO.FileStream(mnt, System.IO.FileMode.Create);
            s.CopyTo(fs);
            fs.Close();
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = mnt;
            p.StartInfo.Arguments = DeveloperImagePath;
            p.Start();
            p.WaitForExit();
            System.IO.File.Delete(mnt);
            return QueryDeveloperDisk();
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
            if (lockdownReturnCode == Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
            {
                IntPtr plistString = LibiMobileDevice.plist_new_string(value);
                Lockdown.LockdownError lderror = Lockdown.lockdownd_set_value(lockdownClient, domain, key, plistString);
                if (lderror == Lockdown.LockdownError.LOCKDOWN_E_SUCCESS)
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
        public dynamic RequestProperty<T>(string domain, string key)
        {
            //PList pl = new PList(this.RequestProperties(domain), true);
            //return Convert.ToString(pl[key]);
            iDevice id = Devices[0];
            IntPtr currDevice;
            string currUdid = id.Udid;
            LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            if (lockdownReturnCode == Lockdown.LockdownError.LOCKDOWN_E_SUCCESS || IsWifiConnect)
            {
                IntPtr resultPlist;
                Lockdown.LockdownError lderror = Lockdown.lockdownd_get_value(lockdownClient, domain, key, out resultPlist);
                XDocument xd = LibiMobileDevice.PlistToXml(resultPlist);
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                LibiMobileDevice.idevice_free(currDevice);
                XElement[] elems = xd.Root.Elements().ToArray();
                string sVal = elems[0].Name.LocalName;
                var typeConv = TypeDescriptor.GetConverter(typeof(T));
                dynamic eVal = (T)typeConv.ConvertFromInvariantString(sVal);
                return eVal;
            }
            else
            {
                Lockdown.FreeClient(lockdownClient);
                Lockdown.FreeService(ldService);
                LibiMobileDevice.idevice_free(currDevice);
                throw new iPhoneException("Lockdown Encountered an Error {0}", lockdownReturnCode);
            }
        }

        public bool ConnectViaHouseArrest(string appId)
        {
            bool ret;
            IntPtr currDevice = this.iPhoneHandle;
            IntPtr ldService;
            IntPtr lockdownClient;
            Lockdown.LockdownError lockdownReturnCode = Lockdown.Start(currDevice, out lockdownClient, out ldService);
            IntPtr hHA;
            IntPtr hSvc;
            HouseArrest.house_arrest_error_t hea = HouseArrest.house_arrest_client_start_service(currDevice, out hHA, out hSvc);
            hea = HouseArrest.house_arrest_send_command(hHA, "VendDocuments", appId);
            IntPtr result;
            hea = HouseArrest.house_arrest_get_result(hHA, out result);
            XDocument har = LibiMobileDevice.PlistToXml(result);
            PList pl = new PList(har, true);
            if (pl.ContainsKey("Error"))
            {
                ret = false;
            }
            else
            {
                IntPtr HA_afc;
                HouseArrest.afc_error_t afce = HouseArrest.afc_client_new_from_house_arrest_client(hHA, out HA_afc);
                ret = afce == HouseArrest.afc_error_t.AFC_E_SUCCESS;
                if (ret)
                    this.hAFC = HA_afc;
            }
            //Don't free the HA client because doing so makes the new AFC client fail.
            //HouseArrest.house_arrest_client_free(hHA);
            Lockdown.FreeService(ldService);
            Lockdown.FreeClient(lockdownClient);
            return ret;
        }

        public void EnableWifiConnection()
        {
            this.SetProperty("com.apple.mobile.wireless_lockdown", "EnableWifiConnections", "true");
        }
        public void DisableWifiConnection()
        {
            this.SetProperty("com.apple.mobile.wireless_lockdown", "EnableWifiConnections", "false");
        }

        public void ReconnectAFC()
        {
            if (this.hAFC != this.hAFC_original)
                AFC.afc_client_free(this.hAFC);
            this.hAFC = this.hAFC_original;
        }

        public unsafe bool CreateDirectory(string path)
        {
            string str = this.FullPath(this.current_directory, path);
            return AFC.afc_make_directory(this.hAFC, str) == AFC.AFCError.AFC_E_SUCCESS;
            //throw new NotImplementedException();
            /*
            if (MobileDevice.AFCDirectoryCreate(this.hAFC, str) != 0)
            {
                return false;
            }
            return true;
            */
        }

        public bool CopyFileToDevice(string computerFile, string deviceFile)
        {
            return AFC.copyToDevice(this.hAFC, computerFile, deviceFile) == AFC.AFCError.AFC_E_SUCCESS;
        }

        public bool CopyFileFromDevice(string computerFile, string deviceFile)
        {
            iDevice id = Devices[0];
            IntPtr currDevice;
            string currUdid = id.Udid;
            LibiMobileDevice.iDeviceError returnCode = LibiMobileDevice.NewDevice(out currDevice, currUdid);
            IntPtr ldService;
            IntPtr lockdownClient;
            IntPtr afcC;
            var afce = AFC.afc_client_start_service(currDevice, out afcC, "MK-iMD");
            return AFC.copyToDisk(afcC, deviceFile, computerFile) == AFC.AFCError.AFC_E_SUCCESS;
        }

        public void DeleteDirectory(string path, bool recursive)
        {
            string str = this.FullPath(this.current_directory, path);
            if (!recursive)
            {
                AFC.afc_remove_path(this.hAFC, str);
            }
            else
            {
                AFC.afc_remove_path_and_contents(this.hAFC, str);
                /*
                if (this.IsDirectory(str))
                {
                    this.InternalDeleteDirectory(path);
                }
                */
            }
        }

        public unsafe void DeleteFile(string path)
        {
            string str = this.FullPath(this.current_directory, path);
            if (this.FileExists(str))
            {
                //MobileDevice.AFCRemovePath(this.hAFC, str);
                AFC.afc_remove_path(this.hAFC, str);
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
            /*
            IntPtr voidPtr;
            this.dnc = new DeviceNotificationCallback(this.NotifyCallback);
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
            */
            IntPtr udata;

            IMDDeviceSubscribeCallbackDelegate = new LibiMobileDevice.LibIMDDeviceNotificationCallback(_NotifyCallback);
            LibiMobileDevice.iDeviceError ide = LibiMobileDevice.idevice_event_subscribe(IMDDeviceSubscribeCallbackDelegate, out udata);
            if (this.lastError == "No accessible Devices found. Enter the passcode and try again.")
                throw new iPhoneException("No accessible Devices found. Enter the passcode and try again.");
            //this.dnc = cbf;
            this.current_directory = "/";

        }

        public unsafe void EnterDFU()
        {
            throw new NotImplementedException();
            //MobileDevice.AMRestorePerformDFURestore(this.iPhoneHandle);
        }



        public unsafe bool FileExists(string path)
        {

            IntPtr dict = IntPtr.Zero;
            //int num = MobileDevice.AFCFileInfoOpen(this.hAFC, path, ref dict);
            int num = (int)AFC.afc_get_file_info(this.hAFC, path, out dict);
            if (num == 0)
            {
                AFC.afc_dictionary_free(dict);
            }
            return (num == 0);
        }

        public ulong FileSize(string path)
        {
            throw new NotImplementedException();
            /*
            ulong num;
            bool flag;
            this.GetFileInfo(path, out num, out flag);
            return num;
            */
        }

        public unsafe void FixRecovery()
        {
            throw new NotImplementedException();
            //MobileDevice.AMRecoveryModeDeviceSetAutoBoot(this.iPhoneHandle);
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
            //throw new NotImplementedException();
            Dictionary<string, string> st_ifmtD = this.GetFileInfo(path);
            if (st_ifmtD.ContainsKey("st_ifmt"))
            {
                string st_ifmt = st_ifmtD["st_ifmt"];
                return st_ifmt;
            }
            else
            {
                return "";
            }
        }

        public string GetCurrentDirectory()
        {
            return this.current_directory;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public unsafe string[] GetAFCInfo()
        {
            if (!this.IsConnected)
            {
                throw new Exception("Not connected to phone");
            }
            List<string> list = new List<string>();
            try
            {
                IntPtr InfoPtr;
                AFC.AFCError afce = AFC.afc_get_device_info(this.hAFC, out InfoPtr);
                if (afce == AFC.AFCError.AFC_E_SUCCESS)
                {
                    list = LibiMobileDevice.PtrToStringList(InfoPtr, 2);
                    AFC.afc_dictionary_free(InfoPtr);
                }
            }
            catch (AccessViolationException ae)
            {

            }
            return (string[])list.ToArray();
        }
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public unsafe Dictionary<string, string> GetFileInfo(string path)
        {
            path = FullPath(this.current_directory, path);
            if (!this.IsConnected)
            {
                throw new Exception("Not connected to phone");
            }

            List<string> infolist = new List<string>();
            try
            {
                IntPtr InfoPtr;
                AFC.AFCError afce = AFC.afc_get_file_info(this.hAFC, path, out InfoPtr);
                if (afce == AFC.AFCError.AFC_E_SUCCESS)
                {
                    infolist = LibiMobileDevice.PtrToStringList(InfoPtr, 2);
                    AFC.afc_dictionary_free(InfoPtr);
                }
                else
                {
                    return new Dictionary<string, string>();
                }
            }
            catch (AccessViolationException ae)
            {

            }
            Dictionary<string, string> infoDict = new Dictionary<string, string>();
            if (infolist.Count % 2 != 0)
            { return infoDict; }
            for (int i = 0; i <= infolist.Count - 2; i += 2)
            {
                infoDict.Add(infolist[i], infolist[i + 1]);
            }
            string str;
            Dictionary<string, string> fileInfo = infoDict;
            ulong size = fileInfo.ContainsKey("st_size") ? ulong.Parse(fileInfo["st_size"]) : ((ulong)0L);
            bool flag = false;
            bool directory = false;
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
                /*
                IntPtr dir = IntPtr.Zero;
                if (directory = MobileDevice.AFCDirectoryOpen(this.hAFC, Encoding.UTF8.GetBytes(path), ref dir) == 0)
                {
                    MobileDevice.AFCDirectoryClose(this.hAFC, dir);
                }
                */
            }
            return fileInfo;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public unsafe string[] GetContents(string path)
        {
            if (!this.IsConnected)
            {
                throw new Exception("Not connected to phone");
            }
            List<string> list = new List<string>();
            try
            {
                IntPtr dirInfoPtr;
                AFC.AFCError afce = AFC.afc_read_directory(this.hAFC, path, out dirInfoPtr);
                if (afce == AFC.AFCError.AFC_E_SUCCESS)
                {
                    list = LibiMobileDevice.PtrToStringList(dirInfoPtr, 2);
                    AFC.afc_dictionary_free(dirInfoPtr);
                }
            }
            catch (AccessViolationException ae)
            {

            }
            return (string[])list.ToArray();
        }
        public unsafe string[] GetDirectories(string path)
        {
            string tCurDir = FullPath(this.current_directory, path);
            //throw new NotImplementedException("Implementation not finished.");
            List<string> dirlist = new List<string>();
            List<string> alllist = GetContents(path).ToList<string>();
            foreach (string wdir in alllist)
            {
                string dir = wdir;
                dir = FullPath(tCurDir, dir);
                bool dirEx = IsDirectory(dir);
                if (dirEx)
                    dirlist.Add(wdir);
            }
            return dirlist.ToArray();
        }

        public string GetDirectoryRoot(string path)
        {
            return "/";
        }
        public unsafe string[] GetFiles(string path)
        {
            //throw new NotImplementedException();

            if (!this.connected)
            {
                throw new Exception("Not connected to phone");
            }
            string tCurDir = FullPath(this.current_directory, path);
            //throw new NotImplementedException("Implementation not finished.");
            List<string> fillist = new List<string>();
            List<string> alllist = GetContents(path).ToList<string>();
            foreach (string wdir in alllist)
            {
                string dir = wdir;
                dir = FullPath(tCurDir, dir);
                bool dirEx = IsFile(dir);
                if (dirEx)
                    fillist.Add(wdir);
            }
            return fillist.ToArray();
        }

        private void InternalDeleteDirectory(string path)
        {
            throw new NotImplementedException();
            /*
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
            */
        }

        public bool IsDirectory(string path)
        {
            return (this.Get_st_ifmt(path) == "S_IFDIR");
        }

        public bool IsFile(string path)
        {
            return (this.Get_st_ifmt(path) == "S_IFREG");
        }

        public bool IsLink(string path)
        {
            return (this.Get_st_ifmt(path) == "S_IFLNK");
        }

        internal void _NotifyCallback(ref LibiMobileDevice.idevice_event_t callback)
        {
            this.attachedToHost = true;
            if (callback.connection_type.ToString() == "1")
            {
                updateDeviceList();
                iDevice id;
                int ndev = Devices.Count;
                this.OnHostAtached(new USBMultiplexArgs(callback, ndev == 0));
                if (ndev == 0)
                {
                    this.lastError = "No accessible Devices found. Enter the passcode and try again.";
                }
                else
                {
                    id = Devices[0];

                    this.iPhoneHandle = id.Handle;
                    //string udid = callback.UniqueDeviceID.ToString();
                    if (this.ConnectToPhone())
                    {
                        this.OnConnect(new ConnectEventArgs(callback));
                    }
                    else
                    {
                        this.lastError = "Device connected but locked; enter passcode and retry.";
                    }
                }
            }
            if (callback.connection_type.ToString() == "2")
            {
                this.connected = false;
                this.attachedToHost = false;
                this.OnHostDisconnected(new USBMultiplexArgs(callback, true));
                this.OnDisconnect(new ConnectEventArgs(callback));
            }
        }

        protected void OnHostAtached(USBMultiplexArgs args)
        {

            DeviceAttachedHandler connect = this.HostAttached;
            if (connect != null)
            {
                connect(this, args);
            }

        }

        protected void OnHostDisconnected(USBMultiplexArgs args)
        {

            DeviceAttachedHandler connect = this.HostDisconnected;
            if (connect != null)
            {
                connect(this, args);
            }

        }

        protected void OnConnect(ConnectEventArgs args)
        {

            ConnectEventHandler connect = this.Connect;
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

        protected void OnDisconnect(ConnectEventArgs args)
        {
            ConnectEventHandler disconnect = this.Disconnect;
            if (disconnect != null)
            {
                disconnect(this, args);
            }
        }

        protected void OnRecoveryModeEnter(DeviceNotificationEventArgs args)
        {
            EventHandler recoveryModeEnter = this.RecoveryModeEnter;
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


        public unsafe void RebootRecovery()
        {
            throw new NotImplementedException();
            //MobileDevice.AMRecoveryModeDeviceReboot(this.iPhoneHandle);
        }

        public unsafe void RebootRestore()
        {
            throw new NotImplementedException();
            //MobileDevice.AMRestorePerformDFURestore(this.iPhoneHandle);            
        }

        public unsafe void ReConnect()
        {
            AFC.afc_client_free(this.hAFC);
            LibiMobileDevice.idevice_free(this.iPhoneHandle);
            this.ConnectToPhone();
        }

        private void RecoveryConnectCallback(ref AMRecoveryDevice callback)
        {
            this.OnRecoveryModeEnter(new DeviceNotificationEventArgs(callback));
        }

        private void RecoveryDisconnectCallback(ref AMRecoveryDevice callback)
        {
            this.OnRecoveryModeLeave(new DeviceNotificationEventArgs(callback));
        }

        public unsafe bool Rename(string sourceName, string destName)
        {
            return AFC.afc_rename_path(this.hAFC, sourceName, destName) == AFC.AFCError.AFC_E_SUCCESS;
            //throw new NotImplementedException();
            //return (MobileDevice.AFCRenamePath(this.hAFC, this.FullPath(this.current_directory, sourceName), this.FullPath(this.current_directory, destName)) == 0);
        }

        public unsafe void sendCommandToDevice(string Command)
        {
            throw new NotImplementedException();
            //MobileDevice.sendCommandToDevice(this.iPhoneHandle, MobileDevice.__CFStringMakeConstantString(MobileDevice.StringToCFString(Command)), 0);
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
                return RequestProperty("ActivationState");
            }
        }

        public unsafe IntPtr AFCHandle
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

        public unsafe IntPtr Device
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
                //return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "BasebandBootloaderVersion");
                return RequestProperty("BasebandBootloaderVersion");
            }
        }

        public unsafe string DeviceBasebandVersion
        {
            get
            {
                //return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "BasebandVersion");
                return RequestProperty("BasebandVersion");
            }
        }

        public unsafe string DeviceBuildVersion
        {
            get
            {
                return RequestProperty("BuildVersion");
            }
        }

        public unsafe string DeviceFirmwareVersion
        {
            get
            {
                return RequestProperty("FirmwareVersion");
            }
        }

        public unsafe string DeviceId
        {
            get
            {
                return RequestProperty("UniqueDeviceID");
            }
        }

        public unsafe string DeviceIntegratedCircuitCardIdentity
        {
            get
            {
                return RequestProperty("IntegratedCircuitCardIdentity");
            }
        }

        public unsafe string DeviceiTunesHasConnected
        {
            get
            {
                return RequestProperty("iTunesHasConnected");
            }
        }

        public unsafe string DeviceModelNumber
        {
            get
            {
                return RequestProperty("ModelNumber");
            }
        }

        public unsafe IntPtr _DeviceHandle
        {
            get
            {
                return (IntPtr)iPhoneHandle;
            }
        }

        public string DeviceName
        {
            get
            {
                return this.RequestProperty("DeviceName");
            }
            set
            {
                this.SetProperty(null, "DeviceName", value);
            }
        }


        public unsafe string DevicePhoneNumber
        {
            get
            {
                return this.RequestProperty("PhoneNumber");
            }
        }

        public unsafe string DeviceProductType
        {
            get
            {
                //return MobileDevice.AMDeviceCopyValue(this.iPhoneHandle, 0, "ProductType");
                return this.RequestProperty("ProductType");
            }
        }

        public unsafe string DeviceSerial
        {
            get
            {
                return RequestProperty("SerialNumber");
            }
        }

        public unsafe string DeviceSIMStatus
        {
            get
            {
                return RequestProperty("SIMStatus");
            }
        }

        public unsafe string DeviceType
        {
            get
            {
                return RequestProperty("DeviceClass");
            }
        }

        public unsafe string DeviceVersion
        {
            get
            {
                return RequestProperty("ProductVersion");
            }
        }

        public unsafe string DeviceWiFiAddress
        {
            get
            {
                return RequestProperty("WiFiAddress");
            }
        }

        public unsafe string IInternationalMobileSubscriberIdentity
        {
            get
            {
                return RequestProperty("InternationalMobileSubscriberIdentity");
            }
        }

        public unsafe string InternationalMobileEquipmentIdentity
        {
            get
            {
                return RequestProperty("InternationalMobileEquipmentIdentity");
            }
        }

        bool getIsWifiConnect()
        {
            int validate = ValidatePairing();
            int pair = Pair();
            if ((pair == -256 && (validate == 0 || validate == -8)) || validate == -8)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsWifiConnect
        {
            get
            {
                return getIsWifiConnect();
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.connected;
            }
        }

        public bool FindMyiPhoneEnabled
        {
            get
            {
                return RequestProperty<bool>("com.apple.fmip", "IsAssociated");
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
                return this.FileExists("/Applications/Cydia.app/");
            }
        }

        public unsafe string ProductVersion
        {
            get
            {
                return RequestProperty("ProductVersion");
            }
        }

        public unsafe string UniqueChipID
        {
            get
            {
                return RequestProperty("UniqueChipID");
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
        public static byte[] ReadBytesOfStream(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}

