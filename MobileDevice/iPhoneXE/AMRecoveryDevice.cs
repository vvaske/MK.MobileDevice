using System;
using System.Runtime.InteropServices;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x02000019 RID: 25
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AMRecoveryDevice
	{
		// Token: 0x04000077 RID: 119
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] unknown0;

		// Token: 0x04000078 RID: 120
		public DeviceRestoreNotificationCallback callback;

		// Token: 0x04000079 RID: 121
		public IntPtr user_info;

		// Token: 0x0400007A RID: 122
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
		public byte[] unknown1;

		// Token: 0x0400007B RID: 123
		public uint readwrite_pipe;

		// Token: 0x0400007C RID: 124
		public byte read_pipe;

		// Token: 0x0400007D RID: 125
		public byte write_ctrl_pipe;

		// Token: 0x0400007E RID: 126
		public byte read_unknown_pipe;

		// Token: 0x0400007F RID: 127
		public byte write_file_pipe;

		// Token: 0x04000080 RID: 128
		public byte write_input_pipe;
	}
}
