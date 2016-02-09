using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MK.MobileDevice.XEDevice.Properties;

namespace MK.MobileDevice.XEDevice
{
    public class iTunesNotInstalledException : Exception
    {

    }
    // Token: 0x0200000F RID: 15
    public class MobileDeviceBase
	{
		// Token: 0x0600009D RID: 157 RVA: 0x00005F94 File Offset: 0x00004194
		static MobileDeviceBase()
		{
			Console.Out.WriteLine("static MobileDevice()");
			string text = Environment.GetEnvironmentVariable("Path");
			string text2 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + "\\Apple\\Mobile Device Support\\bin";
			string text3 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + "\\Apple\\Mobile Device Support";
			string text4 = "C:\\Program Files\\Common Files\\Apple\\Mobile Device Support\\bin";
			string text5 = "C:\\Program Files\\Common Files\\Apple\\Mobile Device Support";
			string text6 = "C:\\Program Files (x86)\\Common Files\\Apple\\Mobile Device Support\\bin";
			string text7 = "C:\\Program Files (x86)\\Common Files\\Apple\\Mobile Device Support";
			string directoryName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			string[] array = new string[]
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
			for (int i = 0; i < array2.Length; i++)
			{
				string text8 = array2[i];
				Console.Out.WriteLine("DEBUG: looking in {0}", Path.Combine(text8, "iTunesMobileDevice.dll"));
				if (File.Exists(Path.Combine(text8, "iTunesMobileDevice.dll")))
				{
					flag = true;
					text = text + ";" + text8;
					IL_10D:
					if (!flag)
					{
                        /*
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
                        */
                        throw new iTunesNotInstalledException();
					}
					if (Settings.Default.iTunesMobileDeviceDllPath.Length > 0)
					{
						bool flag2 = true;
						array2 = array;
						for (i = 0; i < array2.Length; i++)
						{
							text8 = array2[i];
							if (text8 == Settings.Default.iTunesMobileDeviceDllPath)
							{
								flag2 = false;
							}
						}
						if (flag2)
						{
							text = text + ";" + Settings.Default.iTunesMobileDeviceDllPath;
						}
					}
					string text10 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + "\\Apple\\Apple Application Support";
					if (!File.Exists(text10 + "\\CoreFoundation.dll"))
					{
						text10 = "C:\\Program Files\\Apple\\Apple Application Support";
					}
					text = text + ";" + text10;
					Environment.SetEnvironmentVariable("Path", text);
					Console.Out.WriteLine("DEBUG Path = {0}", text);
					return;
				}
			}
            //goto IL_10D;
            return;
		}

		// Token: 0x060000BC RID: 188
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCConnectionClose(void* conn);

		// Token: 0x060000BB RID: 187
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCConnectionInvalidate(void* conn);

		// Token: 0x060000BA RID: 186
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCConnectionIsValid(void* conn);

		// Token: 0x060000B9 RID: 185
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCConnectionOpen(void* handle, uint io_timeout, ref void* conn);

		// Token: 0x060000B1 RID: 177
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCDeviceInfoOpen(void* conn, ref void* dict);

		// Token: 0x060000B6 RID: 182
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCDirectoryClose(void* conn, void* dir);

		// Token: 0x060000CC RID: 204
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCDirectoryCreate(void* conn, byte[] path);

		// Token: 0x060000B2 RID: 178
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCDirectoryOpen(void* conn, byte[] path, ref void* dir);

		// Token: 0x060000B4 RID: 180
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCDirectoryRead(void* conn, void* dir, ref void* dirent);

		// Token: 0x060000BF RID: 191
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileInfoOpen(void* conn, byte[] path, ref void* dict);

		// Token: 0x060000C5 RID: 197
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefClose(void* conn, long handle);

		// Token: 0x060000C4 RID: 196
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefOpen(void* conn, byte[] path, ulong mode, out long handle);

		// Token: 0x060000C6 RID: 198
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefRead(void* conn, long handle, byte[] buffer, ref uint len);

		// Token: 0x060000C9 RID: 201
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefSeek(void* conn, long handle, long pos, long origin);

		// Token: 0x060000CB RID: 203
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefSetFileSize(void* conn, long handle, uint size);

		// Token: 0x060000CA RID: 202
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefTell(void* conn, long handle, ref uint position);

		// Token: 0x060000C7 RID: 199
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFileRefWrite(void* conn, long handle, byte[] buffer, uint len);

		// Token: 0x060000C8 RID: 200
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCFlushData(void* conn, long handle);

		// Token: 0x060000C1 RID: 193
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCKeyValueClose(void* dict);

		// Token: 0x060000C0 RID: 192
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCKeyValueRead(void* dict, out void* key, out void* val);

		// Token: 0x060000C2 RID: 194
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCRemovePath(void* conn, byte[] path);

		// Token: 0x060000C3 RID: 195
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AFCRenamePath(void* conn, byte[] old_path, byte[] new_path);

		// Token: 0x060000A9 RID: 169
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceConnect(void* device);

		// Token: 0x060000BD RID: 189 RVA: 0x00006240 File Offset: 0x00004440
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

		// Token: 0x060000BE RID: 190
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "AMDeviceCopyValue")]
		public unsafe static extern MobileDeviceBase.__CFString* AMDeviceCopyValue_1(void* device, uint unknown, void* cfstring);

		// Token: 0x060000AA RID: 170
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceDisconnect(void* device);

		// Token: 0x060000AF RID: 175
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceGetConnectionID(void* device);

		// Token: 0x060000AB RID: 171
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceIsPaired(void* device);

		// Token: 0x060000D0 RID: 208
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceLookupApplications(void* device, void* options, ref MobileDeviceBase.__CFDictionary* appBundles);

		// Token: 0x060000A8 RID: 168
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceNotificationSubscribe(DeviceNotificationCallback callback, uint unused1, uint unused2, uint unused3, out void* am_device_notification_ptr);

		// Token: 0x060000CF RID: 207
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDevicePair(void* device);

		// Token: 0x060000CD RID: 205
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceStartHouseArrestService(void* device, void* service_name, void* unknown, ref void* handle, int what);

		// Token: 0x060000B8 RID: 184
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceStartService(void* device, void* service_name, ref void* handle, void* unknown);

		// Token: 0x060000AD RID: 173
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceStartSession(void* device);

		// Token: 0x060000AE RID: 174
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceStopSession(void* device);

		// Token: 0x060000CE RID: 206
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceUnpair(void* device);

		// Token: 0x060000AC RID: 172
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMDeviceValidatePairing(void* device);

		// Token: 0x060000B0 RID: 176
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int AMRestoreModeDeviceCreate(uint unknown0, int connection_id, uint unknown1);

		// Token: 0x060000B7 RID: 183
		[DllImport("iTunesMobileDevice.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int AMRestoreRegisterForDeviceNotifications(DeviceRestoreNotificationCallback dfu_connect, DeviceRestoreNotificationCallback recovery_connect, DeviceRestoreNotificationCallback dfu_disconnect, DeviceRestoreNotificationCallback recovery_disconnect, uint unknown0, void* user_info);

		// Token: 0x060000A1 RID: 161
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* CFArrayGetValueAtIndex(void* CFArray, int indexNumber);

		// Token: 0x060000A5 RID: 165
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern bool CFDictionaryContainsKey(void* thisDict, void* thisCFString);

		// Token: 0x060000A6 RID: 166
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* CFDictionaryCreate(void* allocator, void*[] keys, void*[] values, long count, void* keyCallBacks, void* valueCallbacks);

		// Token: 0x0600009F RID: 159
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern uint CFDictionaryGetCount(MobileDeviceBase.__CFDictionary* CFDictionary);

		// Token: 0x060000A0 RID: 160
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int CFDictionaryGetKeysAndValues(MobileDeviceBase.__CFDictionary* CFDictionary, ref MobileDeviceBase.__CFArray* CFArrayKeys, ref MobileDeviceBase.__CFArray* CFArrayValues);

		// Token: 0x060000A7 RID: 167
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void CFRelease(void* cfObj);

		// Token: 0x060000A4 RID: 164
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* CFShow(void* thisObj);

		// Token: 0x060000A3 RID: 163
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern byte CFStringGetCString(MobileDeviceBase.__CFString* thisString, void* value, int length, uint format);

		// Token: 0x060000A2 RID: 162
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern int CFStringGetLength(MobileDeviceBase.__CFString* thisString);

		// Token: 0x060000D3 RID: 211 RVA: 0x00006368 File Offset: 0x00004568
		public static string CFStringToString(byte[] value)
		{
			return Encoding.ASCII.GetString(value, 9, (int)value[9]);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00006204 File Offset: 0x00004404
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

		// Token: 0x060000B3 RID: 179 RVA: 0x000061B8 File Offset: 0x000043B8
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

		// Token: 0x060000D2 RID: 210 RVA: 0x00006318 File Offset: 0x00004518
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

		// Token: 0x060000D1 RID: 209 RVA: 0x000062E4 File Offset: 0x000044E4
		public static byte[] StringToCString(string value)
		{
			byte[] array = new byte[value.Length + 1];
			Encoding.ASCII.GetBytes(value, 0, value.Length, array, 0);
			return array;
		}

		// Token: 0x0600009E RID: 158
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* __CFStringMakeConstantString(byte[] s);

		// Token: 0x04000054 RID: 84
		private const string DLLPath = "iTunesMobileDevice.dll";

		// Token: 0x04000055 RID: 85
		private const string DLLPathCF = "CoreFoundation.dll";

		// Token: 0x02000011 RID: 17
		public struct __CFArray
		{
		}

		// Token: 0x02000010 RID: 16
		public struct __CFDictionary
		{
		}

		// Token: 0x02000012 RID: 18
		public struct __CFString
		{
		}
	}
}
