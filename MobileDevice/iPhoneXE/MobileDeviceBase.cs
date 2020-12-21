using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using MK.MobileDevice.XEDevice.Properties;

namespace MK.MobileDevice.XEDevice
{
    public class iTunesNotInstalledException : Exception
    {

    }

    public class MobileDeviceBase
	{
		private const string DLLPath = "MobileDevice.dll";

		[DllImport("msvcr71.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* malloc(uint size);

		static MobileDeviceBase()
		{
			Console.Out.WriteLine("static MobileDevice()");
			string text = Environment.GetEnvironmentVariable("Path");
			string text2 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + "\\Apple\\Mobile Device Support\\bin";
			string text3 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + "\\Apple\\Mobile Device Support";
			string text4 = "C:\\Program Files (x86)\\Common Files\\Apple\\Mobile Device Support\\bin";
			string text5 = "C:\\Program Files (x86)\\Common Files\\Apple\\Mobile Device Support";
			string text6 = "C:\\Program Files\\Common Files\\Apple\\Mobile Device Support\\bin";
			string text7 = "C:\\Program Files\\Common Files\\Apple\\Mobile Device Support";
			string directoryName = Path.GetDirectoryName(Application.ExecutablePath);
			string[] array = new string[8]
			{
				directoryName,
				text2,
				text3,
				text4,
				text5,
				text6,
				text7,
				Settings.Default.iTunesMobileDeviceDllPath
			};
			bool flag = false;
			string[] array2 = array;
			foreach (string text8 in array2)
			{
				Console.Out.WriteLine("DEBUG: looking in {0}", Path.Combine(text8, "MobileDevice.dll"));
				if (File.Exists(Path.Combine(text8, "MobileDevice.dll")))
				{
					flag = true;
					text = text + ";" + text8;
					break;
				}
			}
			if (!flag)
			{
				string caption = "Install iTunes?";
				string text9 = "iTunes must be installed in order to use iExplorer.\nClick YES to download iTunes now.";
				DialogResult dialogResult = MessageBox.Show(text9, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
				if (dialogResult == DialogResult.Yes)
				{
					Process.Start("http://www.itunes.com/");
				}
				else
				{
					Application.Exit();
				}
			}
			if (Settings.Default.iTunesMobileDeviceDllPath.Length > 0)
			{
				bool flag2 = true;
				string[] array3 = array;
				foreach (string a in array3)
				{
					if (a == Settings.Default.iTunesMobileDeviceDllPath)
					{
						flag2 = false;
					}
				}
				if (flag2)
				{
					text = text + ";" + Settings.Default.iTunesMobileDeviceDllPath;
				}
			}
			string text10 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + "\\Apple\\Mobile Device Support";
			string text11 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + "\\Apple\\Apple Application Support";
			string text12 = "C:\\Program Files (x86)\\Common Files\\Apple\\Mobile Device Support";
			string text13 = "C:\\Program Files (x86)\\Common Files\\Apple\\Apple Application Support";
			string[] array4 = new string[4]
			{
				text10,
				text11,
				text12,
				text13
			};
			string[] array5 = array4;
			foreach (string text14 in array5)
			{
				Console.Out.WriteLine("DEBUG: looking in {0}", Path.Combine(text14, "CoreFoundation.dll"));
				if (File.Exists(Path.Combine(text14, "CoreFoundation.dll")))
				{
					text = text + ";" + text14;
					break;
				}
			}
			Environment.SetEnvironmentVariable("Path", text);
		}

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCConnectionClose(void* conn);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCConnectionInvalidate(void* conn);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCConnectionIsValid(void* conn);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCConnectionOpen(void* handle, uint io_timeout, ref void* conn);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCDeviceInfoOpen(void* conn, ref void* dict);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCDirectoryClose(void* conn, void* dir);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCDirectoryCreate(void* conn, byte[] path);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCDirectoryOpen(void* conn, byte[] path, ref void* dir);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCDirectoryRead(void* conn, void* dir, ref void* dirent);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileInfoOpen(void* conn, byte[] path, ref void* dict);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefClose(void* conn, long handle);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefOpen(void* conn, byte[] path, ulong mode, out long handle);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefRead(void* conn, long handle, byte[] buffer, ref uint len);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefSeek(void* conn, long handle, long pos, long origin);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefSetFileSize(void* conn, long handle, uint size);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefTell(void* conn, long handle, ref uint position);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefWrite(void* conn, long handle, byte[] buffer, uint len);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFlushData(void* conn, long handle);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCKeyValueClose(void* dict);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCKeyValueRead(void* dict, out void* key, out void* val);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCRemovePath(void* conn, byte[] path);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCRenamePath(void* conn, byte[] old_path, byte[] new_path);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceConnect(void* device);

		public unsafe static string AMDeviceCopyValue(void* device, string name)
		{
			string result;
			try
			{
				MobileDeviceBase.__CFString* ptr = MobileDeviceBase.AMDeviceCopyValue_1(device, 0u, MobileDeviceBase.__CFStringMakeConstantString(MobileDeviceBase.StringToCString(name)));
				if (ptr != null)
				{
					uint num = (uint)MobileDeviceBase.CFStringGetLength(ptr);
					uint num2 = 4u * num + 2u;
					sbyte* value = (sbyte*)MobileDevice32.malloc(num2);
					MobileDeviceBase.CFStringGetCString(ptr, (void*)value, (int)num2, 134217984u);
					UTF8Marshaler uTF8Marshaler = new UTF8Marshaler();
					result = (string)uTF8Marshaler.MarshalNativeToManaged(new IntPtr((void*)value));
					return result;
				}
			}
			catch (Exception ex)
			{
				LogViewer.LogEvent(0, "AMDeviceCopyValue Error: " + ex.ToString());
				result = "Unknown";
				return result;
			}
			result = string.Empty;
			return result;
		}

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "AMDeviceCopyValue")]
		public unsafe static extern MobileDeviceBase.__CFString* AMDeviceCopyValue_1(void* device, uint unknown, void* cfstring);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceDisconnect(void* device);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceGetConnectionID(void* device);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceIsPaired(void* device);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceLookupApplications(void* device, void* options, ref MobileDeviceBase.__CFDictionary* appBundles);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceNotificationSubscribe(DeviceNotificationCallback callback, uint unused1, uint unused2, uint unused3, out void* am_device_notification_ptr);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDevicePair(void* device);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceStartHouseArrestService(void* device, void* service_name, void* unknown, ref void* handle, int what);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceStartService(void* device, void* service_name, ref void* handle, void* unknown);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceStartSession(void* device);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceStopSession(void* device);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceUnpair(void* device);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceValidatePairing(void* device);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int AMRestoreModeDeviceCreate(uint unknown0, int connection_id, uint unknown1);

		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMRestoreRegisterForDeviceNotifications(DeviceRestoreNotificationCallback dfu_connect, DeviceRestoreNotificationCallback recovery_connect, DeviceRestoreNotificationCallback dfu_disconnect, DeviceRestoreNotificationCallback recovery_disconnect, uint unknown0, void* user_info);

		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* CFArrayGetValueAtIndex(void* CFArray, int indexNumber);

		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern bool CFDictionaryContainsKey(void* thisDict, void* thisCFString);

		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* CFDictionaryCreate(void* allocator, void*[] keys, void*[] values, long count, void* keyCallBacks, void* valueCallbacks);

		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint CFDictionaryGetCount(MobileDeviceBase.__CFDictionary* CFDictionary);

		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int CFDictionaryGetKeysAndValues(MobileDeviceBase.__CFDictionary* CFDictionary, ref MobileDeviceBase.__CFArray* CFArrayKeys, ref MobileDeviceBase.__CFArray* CFArrayValues);

		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void CFRelease(void* cfObj);

		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* CFShow(void* thisObj);

		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern byte CFStringGetCString(MobileDeviceBase.__CFString* thisString, void* value, int length, uint format);

		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int CFStringGetLength(MobileDeviceBase.__CFString* thisString);

		public static string CFStringToString(byte[] value)
		{
			return Encoding.ASCII.GetString(value, 9, (int)value[9]);
		}

		public unsafe static int IDDirectoryRead(void* conn, void* dir, ref string itemName)
		{
			void* ptr = null;
			int result = MobileDeviceBase.AFCDirectoryRead(conn, dir, ref ptr);
			if (ptr != null)
			{
				IntPtr ptr2 = new IntPtr(ptr);
				itemName = MobileDeviceBase.PointerToString(ptr2);
			}
			else
			{
				itemName = null;
			}
			return result;
		}

		private static string PointerToString(IntPtr ptr)
		{
			int num = 0;
			while (Marshal.ReadByte(ptr, num) != 0)
			{
				num++;
				if (num >= 2048)
				{
					break;
				}
			}
			byte[] array = new byte[num];
			Marshal.Copy(ptr, array, 0, num);
			return Encoding.UTF8.GetString(array);
		}

		public static byte[] StringToCFString(string value)
		{
			byte[] array = new byte[value.Length + 10];
			array[4] = 140;
			array[5] = 7;
			array[6] = 1;
			array[8] = (byte)value.Length;
			Encoding.ASCII.GetBytes(value, 0, value.Length, array, 10);
			return array;
		}

		public static byte[] StringToCString(string value)
		{
			byte[] array = new byte[value.Length + 1];
			Encoding.ASCII.GetBytes(value, 0, value.Length, array, 0);
			return array;
		}

		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* __CFStringMakeConstantString(byte[] s);

		private const string DLLPathCF = "CoreFoundation.dll";

		public struct __CFArray
		{
		}

		public struct __CFDictionary
		{
		}

		public struct __CFString
		{
		}
	}
}
