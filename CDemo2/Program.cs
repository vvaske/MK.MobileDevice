/*

 */
using System;
using MK.MobileDevice;
using MK.Plist;
using System.Collections.Generic;

namespace CDemo2
{
	class Program
	{
		static iPhone iph;
		static void ccallback(object sender, EventArgs args)
		{
			Console.WriteLine("Connected.");
			bool available = iph.IsConnected;
			Console.WriteLine(available);
			string dn = iph.RequestProperty<string>(null, "DeviceName");
			Console.WriteLine("Device: "+dn);
			Console.Write("Enter new name: ");
			string nn = Console.ReadLine();
			if (nn != "")
			{
				bool s = iph.SetProperty(null,"DeviceName",nn);
				Console.WriteLine(s);
			}
			PList pl = new PList(iph.RequestProperties(null), false);
			foreach (KeyValuePair<string, dynamic> kvp in pl)
			{
				Console.WriteLine("{0} : {1}", kvp.Key, kvp.Value);
			}
			string[] dirlist = iph.GetContents("/");
			Console.WriteLine("\n\nDirectory Listing:");
			foreach (string path in dirlist)
			{
				Console.WriteLine(path);
			}
			string[] afcinfo = iph.GetAFCInfo();
			Console.WriteLine("\n\nAFC Info:");
			foreach (string key in afcinfo)
			{
				Console.WriteLine(key);
			}
            Dictionary<string, string> info;
			info = iph.GetFileInfo("/DCIM");
			Console.WriteLine("\n\nDCIM Info:");
			foreach (KeyValuePair<string, string> kvp in info)
			{
                Console.WriteLine("{0} : {1}", kvp.Key, kvp.Value);
            }
            info = iph.GetFileInfo("test");
            Console.WriteLine("\n\ntest file Info:");
            foreach (KeyValuePair<string, string> kvp in info)
            {
                Console.WriteLine("{0} : {1}", kvp.Key, kvp.Value);
            }

            Console.WriteLine("Writing File to Device...");
			Console.WriteLine(iph.CopyFileToDevice("MK.MobileDevice.dll","test"));
			Console.WriteLine("Fetching File from Device...");
			Console.WriteLine(iph.CopyFileFromDevice("test.dll","test"));
            Console.WriteLine(iph.IsLink("/"));
            
            //System.Threading.Thread.Sleep(10);
		}
		static void cdallback(object sender, EventArgs args)
		{
            
			Console.WriteLine("Disconnected.");
			//Console.ReadKey();
            //Environment.Exit(0);
		}
        static void PowerMode()
        {
            iPhone iph = new iPhone();
            while (true)
            {
                Console.WriteLine("Power Mode:");
                Console.WriteLine("1. Enter Recovery");
                Console.WriteLine("2. Exit Recovery");
                Console.WriteLine("3. Recovery Cycle [Refresh Firmware]");
                Console.WriteLine("X. Exit");
                string c = Console.ReadLine();
                if (c == "1")
                {
                    //iph.EnterRecovery();
                }
                else if (c == "2")
                {
                    //iph.ExitRecovery();
                }
                else if (c == "3")
                {
                    //iph.EnterRecovery();
                    //iph.ExitRecovery();
                }
                else
                {
                    Console.WriteLine("Exiting Advanced Mode.");
                    break;
                }
            }
        }
        static void runLauncher()
        {
            Console.WriteLine("Press SPACE to enter Advanced Mode, or press any other key to start normally.");
            ConsoleKeyInfo cki = Console.ReadKey();
            if (cki.Key == ConsoleKey.Spacebar)
            {
                PowerMode();
            }
            normalMode();
        }
        static void normalMode()
        {
            Console.WriteLine("Waiting for device...");
            iph = new iPhone();
            //GC.KeepAlive(iph);
            iph.Connect += (sender, args) => ccallback(sender, args);
            iph.Disconnect += (sender, args) => cdallback(sender, args);
            while (true)
            { }
        }
		public static void Main(string[] cmdline)
		{
            TestITMD.itmdTest();
            //runLauncher();
		}
		
	}
}