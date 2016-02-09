using System;
using System.Runtime.InteropServices;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x0200001A RID: 26
	// Token: 0x060000DB RID: 219
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void DeviceNotificationCallback(ref AMDeviceNotificationCallbackInfo callback_info);
}
