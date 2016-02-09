using System;
using System.Runtime.InteropServices;

namespace Reiboot
{
	internal class recoverydll
	{
		public enum iphone_mode
		{
			NORMAL_UNKNOWN,
			NORMAL_CONNECTED,
			NORMAL_NCONNECTED,
			NORMAL_DFU,
			NORMAL_NDFU,
			NORMAL_RECOVERY,
			NORMAL_NRECOVERY
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public delegate int DelegateMode_callback_t(int dev, int id);

		[DllImport("Reiboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern int modification_init();

		[DllImport("Reiboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern int device_connect(recoverydll.DelegateMode_callback_t callback);

		[DllImport("Reiboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern bool IsWiFiConnect(int device);

		[DllImport("Reiboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern bool GetDeviceInfo(int device, string[] str_info, int num, IntPtr[] str_key);

		[DllImport("Reiboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern void FreeDeviceInfo(IntPtr[] str_key, int num);

		[DllImport("Reiboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern int intorecovery(int device);

		[DllImport("Reiboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		public static extern int outrecovery(int device);
	}
}
