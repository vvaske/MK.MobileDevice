using System;
using System.Runtime.InteropServices;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x02000018 RID: 24
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AMDeviceNotificationCallbackInfo
	{
		// Token: 0x1700002E RID: 46
		public unsafe void* dev
		{
			// Token: 0x060000D9 RID: 217 RVA: 0x00006388 File Offset: 0x00004588
			get
			{
				return this.dev_ptr;
			}
		}

		// Token: 0x04000075 RID: 117
		public unsafe void* dev_ptr;

		// Token: 0x04000076 RID: 118
		public NotificationMessage msg;
	}
}
