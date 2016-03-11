using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MK.MobileDevice;
using MK.MobileDevice.TAI;
using MK.MobileDevice.XEDevice;
using System.Text.RegularExpressions;

namespace CDemo2
{
    static class TestITMD
    {
        static iTMDiPhone iph;
        static iTMDiPhoneLL lliph;
        static iPhone mdv;
        static TAIiPhone tai;
        static iPhoneXE xe;

        public static void itmdTest()
        {
            Console.WriteLine("Starting ITMD Test...");
            //Console.ReadKey();
            iph = new iTMDiPhone();
            lliph = new iTMDiPhoneLL();
            try
            {
                mdv = new iPhone();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            tai = new TAIiPhone();

            mdv.Connect += Mdv_Connect;
            mdv.HostAttached += Mdv_HostAttached;
            iph.Connect += Iph_Connect;
            tai.Connect += Tai_Connect;

            lliph.HostAttached += Lliph_HostAttached;

            iph.DfuConnect += Iph_DfuConnect;
            while (true)
            {
                if (mdv.attachedToHost ^ mdv.IsConnected)
                {
                    //Console.Clear();
                    //Console.WriteLine("Device attached but not paired. Enter passcode on device to pair.");
                }
            }
        }

        private static void Lliph_HostAttached(object sender, ITMDConnectEventArgs args)
        {
            Console.WriteLine("iTMD.dll detected device attached to host.");
            Console.WriteLine(lliph.DeviceName);
        }

        private static void Mdv_HostAttached(object sender, MK.MobileDevice.USBMultiplexArgs args)
        {
            Console.WriteLine("libimobiledevice.dll detected device attached to host.");
            Console.WriteLine("Device Locked: {0}", args.IsLocked);
            //Console.WriteLine("Uninstalled.");
            Console.WriteLine("FMIP: {0}", mdv.FindMyiPhoneEnabled);
            Console.WriteLine("PurpleBuddy GetValue: {0}", mdv.RequestProperty<string>("com.apple.PurpleBuddy", "SetupState"));
            mdv.EnableWifiConnection();
        }

        private static void Tai_Connect(object sender, MK.MobileDevice.TAI.ConnectEventArgs args)
        {

        }

        private static void Iph_DfuConnect(object sender, EventArgs e)
        {
            Console.WriteLine("iTunesMobileDevice.dll Connected to [DFU]" + iph.DeviceName);
            Console.WriteLine("Requesting Recovery...");
            iph.EnterDFU();
            Console.ReadKey();
        }

        private static void Mdv_Connect(object sender, MK.MobileDevice.ConnectEventArgs args)
        {
            Console.WriteLine("libimobiledevice.dll Connected [MUX] to " + mdv.DeviceName);
            Console.WriteLine("IsWifiConnect: {0}", mdv.IsWifiConnect);
        }

        private static void Iph_Connect(object sender, ITMDConnectEventArgs args)
        {

            Console.WriteLine("iTunesMobileDevice.dll Connected to " + iph.DeviceName);
            string dn = iph.DeviceName;
            string pn = iph.ProductName;
            string pv = iph.ProductVersion;
            string infoS = String.Format("connected to: {0} ({1}, iOS {2})", dn, pn, pv);
            Console.WriteLine("FMIP AMDeviceCopyValue: {0}", iph.RequestProperty("com.apple.fmip", "IsAssociated"));
            /*
            Console.WriteLine(infoS);
            //com.apple.mobile.battery
            //Console.WriteLine("AMDeviceCopyValue: {0}", iph.DeviceiTunesHasConnected);
            Console.WriteLine("Enabling WiFi: {0}",iph.EnableWiFiConnection());
            //Console.WriteLine("Disabling WiFi: {0}",iph.DisableWiFiConnection());
            */
        }
    }
}
