using System;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x02000025 RID: 37
	public class DeviceNotificationEventArgs : EventArgs
	{
		// Token: 0x0600013F RID: 319 RVA: 0x000021D8 File Offset: 0x000003D8
		public DeviceNotificationEventArgs(AMRecoveryDevice device)
		{
			this.device = device;
		}

		// Token: 0x17000033 RID: 51
		public AMRecoveryDevice Device
		{
			// Token: 0x06000140 RID: 320 RVA: 0x00008A54 File Offset: 0x00006C54
			get
			{
				return this.device;
			}
		}

		// Token: 0x040000C5 RID: 197
		private AMRecoveryDevice device;
	}
}
