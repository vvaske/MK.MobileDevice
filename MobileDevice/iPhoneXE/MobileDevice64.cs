using System;
using System.Runtime.InteropServices;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x02000014 RID: 20
	internal class MobileDevice64 : MobileDeviceBase
	{
		// Token: 0x060000D7 RID: 215
		[DllImport("msvcr100.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* malloc(ulong size);
	}
}
