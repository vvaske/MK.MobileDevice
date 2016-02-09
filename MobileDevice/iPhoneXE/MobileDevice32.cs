using System;
using System.Runtime.InteropServices;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x02000013 RID: 19
	internal class MobileDevice32 : MobileDeviceBase
	{
		// Token: 0x060000D5 RID: 213
		[DllImport("msvcr71.dll", CallingConvention = CallingConvention.Cdecl)]
		public unsafe static extern void* malloc(uint size);
	}
}
