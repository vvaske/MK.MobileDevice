
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using Reiboot;

namespace rebootpwn
{
    public class rebootpwn
    {
        private recoverydll.DelegateMode_callback_t mode_callback_t;
        public string m_deviceName;
        public string m_deviceType;
        public string m_deviceVersion;
        public int m_deviceID;
        public int m_iphoneConnectState;
        private DispatcherTimer m_timer;
        private Dictionary<string, string> m_iphoneType;
        
        private bool _contentLoaded;

        public int connectionStatus
        {
            get
            {
                return m_iphoneConnectState;
            }
        }

        public int initRbthck()
        {
            //this.InitializeComponent();
            //this.InitButton();
            this.ShowDownloadDllDialog();
            this.InitiPhoneVersion();
            this.InitIOSConnect();
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string path = folderPath + "\\Tenorshare\\Reiboot\\ShowUpgradeDlg";
            if (!Directory.Exists(path))
            {
                //this.ShowUpgradeDialog();
            }
            this.m_timer = new DispatcherTimer();
            this.m_timer.Tick += new EventHandler(SetButtonAndLabelByiphoneConnectState);
            this.m_timer.Interval = TimeSpan.FromMilliseconds(50.0);
            this.m_timer.Start();
            DiagnoseStatus();
            int iphcs = this.m_iphoneConnectState;
            return this.m_iphoneConnectState;
        }

        private void DiagnoseStatus()
        {
            switch (this.m_iphoneConnectState)
            {
                //current status
                case 0:
                case 3:
                case 4:
                    break;
                case 1:
                    //successful connect
                    return;
                case 2:
                    //Not connecting
                    return;
                case 5:
                    //In Recovery
                    return;
                case 6:
                    //In normal boot
                    break;
                default:
                    return;
            }
        }
        private void SetButtonAndLabelByiphoneConnectState(object sender, EventArgs e)
        {
            
            switch (this.m_iphoneConnectState)
            {
                //current status
                case 0:
                case 3:
                case 4:
                    break;
                case 1:
                    //successful connect
                    return;
                case 2:
                    //Not connecting
                    return;
                case 5:
                    //In Recovery
                    return;
                case 6:
                    //In normal boot
                    break;
                default:
                    return;
            }
        }
        
        private void InitiPhoneVersion()
        {
            this.m_iphoneType = new Dictionary<string, string>();
            this.m_iphoneType.Add("iPhone8,2", "iPhone 6S Plus");
            this.m_iphoneType.Add("iPhone8,1", "iPhone 6S");
            this.m_iphoneType.Add("iPhone7,2", "iPhone 6");
            this.m_iphoneType.Add("iPhone7,1", "iPhone 6 Plus");
            this.m_iphoneType.Add("iPhone6,2", "iPhone 5S");
            this.m_iphoneType.Add("iPhone6,1", "iPhone 5S");
            this.m_iphoneType.Add("iPhone5,4", "iPhone 5C");
            this.m_iphoneType.Add("iPhone5,3", "iPhone 5C");
            this.m_iphoneType.Add("iPhone5,2", "iPhone 5");
            this.m_iphoneType.Add("iPhone5,1", "iPhone 5");
            this.m_iphoneType.Add("iPhone4,1", "iPhone 4S");
            this.m_iphoneType.Add("iPhone3,3", "iPhone 4");
            this.m_iphoneType.Add("iPhone3,2", "iPhone 4");
            this.m_iphoneType.Add("iPhone3,1", "iPhone 4");
            this.m_iphoneType.Add("iPhone2,1", "iPhone 3GS");
            this.m_iphoneType.Add("iPhone1,2", "iPhone 3G");
            this.m_iphoneType.Add("iPad4,6", "iPad");
            this.m_iphoneType.Add("iPad4,5", "iPad Mini 2 LTE");
            this.m_iphoneType.Add("iPad4,4", "iPad Mini 2 WIFI");
            this.m_iphoneType.Add("iPad4,3", "iPad");
            this.m_iphoneType.Add("iPad4,2", "iPad Air LTE");
            this.m_iphoneType.Add("iPad4,1", "iPad Air WIFI");
            this.m_iphoneType.Add("iPad3,4", "iPad 4");
            this.m_iphoneType.Add("iPad3,6", "iPad 4");
            this.m_iphoneType.Add("iPad3,3", "iPad 3");
            this.m_iphoneType.Add("iPad3,2", "iPad 3");
            this.m_iphoneType.Add("iPad3,1", "iPad 3");
            this.m_iphoneType.Add("iPad2,7", "iPad Mini");
            this.m_iphoneType.Add("iPad2,6", "iPad Mini");
            this.m_iphoneType.Add("iPad2,5", "iPad Mini");
            this.m_iphoneType.Add("iPad2,4", "iPad 2");
            this.m_iphoneType.Add("iPad2,3", "iPad 2");
            this.m_iphoneType.Add("iPad2,2", "iPad 2");
            this.m_iphoneType.Add("iPad2,1", "iPad 2");
            this.m_iphoneType.Add("iPad1,1", "iPad");
            this.m_iphoneType.Add("iPod5,1", "iPod");
            this.m_iphoneType.Add("iPod4,1", "iPod");
            this.m_iphoneType.Add("iPod3,1", "iPod");
            this.m_iphoneType.Add("iPod2,1", "iPod");
            this.m_iphoneType.Add("iPod1,1", "iPod");
        }

        private void ShowTrustComputerDialog()
        {
            //TrustComputerWidget trustComputerWidget = new TrustComputerWidget();
            //trustComputerWidget.Show();
            throw new Exception("Set the iOS device to trust this computer.");
        }

        public int InitIOSConnect()
        {
            recoverydll.modification_init();
            this.mode_callback_t = new recoverydll.DelegateMode_callback_t(this.Callback);
            int ret = recoverydll.device_connect(this.mode_callback_t);
            int cs = this.m_iphoneConnectState;
            return ret;
            
        }

        private int Callback(int mode, int devinfo)
        {
            int num = 0;
            switch (mode)
            {
                case 1:
                    if (recoverydll.IsWiFiConnect(devinfo))
                    {
                        return 0;
                    }
                    num = 1;
                    break;
                case 5:
                    num = 5;
                    break;
            }
            if (num == 1)
            {
                string[] array = new string[4];
                array[0] = "DeviceName";
                array[1] = "ProductType";
                array[2] = "ProductVersion";
                string[] str_info = array;
                string[] array2 = new string[4];
                string[] array3 = array2;
                IntPtr[] array4 = new IntPtr[4];
                for (int i = 0; i < array3.Length; i++)
                {
                    array4[i] = Marshal.StringToCoTaskMemUni(array3[i]);
                }
                GCHandle gCHandle = GCHandle.Alloc(array4, GCHandleType.Pinned);
                gCHandle.AddrOfPinnedObject();
                gCHandle.Free();
                for (int j = 0; j < array3.Length; j++)
                {
                    Marshal.FreeCoTaskMem(array4[j]);
                }
                recoverydll.GetDeviceInfo(devinfo, str_info, 3, array4);
                this.m_deviceName = Marshal.PtrToStringAnsi(array4[0]);
                string text = Marshal.PtrToStringAnsi(array4[1]);
                if (!string.IsNullOrEmpty(text))
                {
                    this.m_deviceType = this.m_iphoneType[text];
                }
                this.m_deviceVersion = Marshal.PtrToStringAnsi(array4[2]);
                recoverydll.FreeDeviceInfo(array4, 4);
            }
            this.m_deviceID = devinfo;
            this.m_iphoneConnectState = mode;
            
            return 0;
        }

        public bool EnterRecovery()
        {
            //this.m_btnEnter.IsEnabled = false;
            //this.m_btnExit.IsEnabled = true;
            //this.m_tbState.Text = base.FindResource("EnterRecovering").ToString();
            int num = recoverydll.intorecovery(this.m_deviceID);
            if (num != 0)
            {
                //this.m_btnEnter.IsEnabled = true;
                //this.m_btnExit.IsEnabled = false;
                //this.m_tbState.Text = base.FindResource("FailToEnter").ToString();
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ExitRecovery()
        {
            //this.m_btnEnter.IsEnabled = true;
            //this.m_btnExit.IsEnabled = false;
            //this.m_tbState.Text = base.FindResource("ExitRecovering").ToString();
            int num = recoverydll.outrecovery(this.m_deviceID);
            if (num != 0)
            {
                //this.m_btnEnter.IsEnabled = true;
                //this.m_btnExit.IsEnabled = false;
                //this.m_tbState.Text = base.FindResource("FailToExit").ToString();
                return false;
            }
            else
                return true;
        }


        private void ShowDownloadDllDialog()
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string path = folderPath + "\\Tenorshare\\Driver\\iTunesMobileDevice.dll";
            string path2 = folderPath + "\\Tenorshare\\Driver\\iTunes.dll";
            string path3 = folderPath + "\\Tenorshare\\Driver\\";
            if (!File.Exists(path2) || !File.Exists(path))
            {
                if (!Directory.Exists(path3))
                {
                    Directory.CreateDirectory(path3);
                }
                //throw new Exception("iTunes DLLs Missing; please download them by using the GUI version of pwnreboot");
                File.Copy("iTunesMobileDevice.dll", path);
                File.Copy("iTunes.dll", path2);
            }
        }

        

        

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
            {
                return;
            }
            this._contentLoaded = true;
            Uri resourceLocator = new Uri("/Reiboot;component/window1.xaml", UriKind.Relative);
            Application.LoadComponent(this, resourceLocator);
        }

        [DebuggerNonUserCode]
        internal Delegate _CreateDelegate(Type delegateType, string handler)
        {
            return Delegate.CreateDelegate(delegateType, this, handler);
        }

        
    }
}
