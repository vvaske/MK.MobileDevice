using System;
using System.Runtime.InteropServices;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x02000028 RID: 40
	internal class Winsock
	{
		// Token: 0x06000152 RID: 338
		[DllImport("ws2_32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int closesocket(IntPtr s);

		// Token: 0x06000151 RID: 337
		[DllImport("Ws2_32.dll")]
		public static extern int recv(IntPtr s, IntPtr buf, int len, int flags);

		// Token: 0x06000150 RID: 336
		[DllImport("Ws2_32.dll")]
		public static extern int send(IntPtr s, IntPtr buf, int len, int flags);
	}
}
