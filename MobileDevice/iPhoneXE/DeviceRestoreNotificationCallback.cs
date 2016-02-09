using System;
using System.Runtime.InteropServices;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x0200001B RID: 27
	// Token: 0x060000DF RID: 223
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void DeviceRestoreNotificationCallback(ref AMRecoveryDevice callback_info);
}
