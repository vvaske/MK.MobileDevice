using System;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x0200001C RID: 28
	public unsafe class ConnectEventArgs : EventArgs
	{
		// Token: 0x060000E2 RID: 226 RVA: 0x000020F8 File Offset: 0x000002F8
		public ConnectEventArgs(AMDeviceNotificationCallbackInfo cbi)
		{
			this.message = cbi.msg;
			this.device = cbi.dev;
		}

		// Token: 0x1700002F RID: 47
		public unsafe void* Device
		{
			// Token: 0x060000E3 RID: 227 RVA: 0x000063A0 File Offset: 0x000045A0
			get
			{
				return this.device;
			}
		}

		// Token: 0x17000030 RID: 48
		public NotificationMessage Message
		{
			// Token: 0x060000E4 RID: 228 RVA: 0x000063B8 File Offset: 0x000045B8
			get
			{
				return this.message;
			}
		}

		// Token: 0x04000082 RID: 130
		private unsafe void* device;

		// Token: 0x04000081 RID: 129
		private NotificationMessage message;
	}
}
