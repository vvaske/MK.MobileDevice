using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;


namespace MK.MobileDevice.XEDevice
{
	// Token: 0x02000027 RID: 39
	public class UTF8Marshaler : ICustomMarshaler
	{
		// Token: 0x06000149 RID: 329 RVA: 0x00002221 File Offset: 0x00000421
		public void CleanUpManagedData(object ManagedObj)
		{
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00008AD4 File Offset: 0x00006CD4
		public void CleanUpNativeData(IntPtr pNativeData)
		{
			if (this.allocated.Contains(pNativeData))
			{
				Marshal.FreeHGlobal(pNativeData);
				this.allocated.Remove(pNativeData);
			}
			else
			{
				LogViewer.LogEvent(1, "WARNING: Trying to free an unallocated pointer!");
				LogViewer.LogEvent(1, "         This is most likely a bug in mono");
			}
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00008AC0 File Offset: 0x00006CC0
		public static ICustomMarshaler GetInstance(string cookie)
		{
			return UTF8Marshaler.marshaler;
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00008B28 File Offset: 0x00006D28
		public int GetNativeDataSize()
		{
			return -1;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00008B38 File Offset: 0x00006D38
		public IntPtr MarshalManagedToNative(object ManagedObj)
		{
			IntPtr result;
			if (ManagedObj == null)
			{
				result = IntPtr.Zero;
			}
			else
			{
				if (ManagedObj.GetType() != typeof(string))
				{
					throw new ArgumentException("ManagedObj", "Can only marshal type of System.string");
				}
				byte[] bytes = Encoding.UTF8.GetBytes((string)ManagedObj);
				int cb = Marshal.SizeOf(typeof(byte)) * (bytes.Length + 1);
				IntPtr intPtr = Marshal.AllocHGlobal(cb);
				this.allocated.Add(intPtr, null);
				Marshal.Copy(bytes, 0, intPtr, bytes.Length);
				Marshal.WriteByte(intPtr, bytes.Length, 0);
				result = intPtr;
			}
			return result;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00008BDC File Offset: 0x00006DDC
		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			object result;
			if (pNativeData == IntPtr.Zero)
			{
				result = null;
			}
			else
			{
				int num = 0;
				while (Marshal.ReadByte(pNativeData, num) > 0)
				{
					num++;
				}
				byte[] array = new byte[num];
				Marshal.Copy(pNativeData, array, 0, num);
				result = Encoding.UTF8.GetString(array);
			}
			return result;
		}

		// Token: 0x040000C8 RID: 200
		private Hashtable allocated = new Hashtable();

		// Token: 0x040000C7 RID: 199
		private static UTF8Marshaler marshaler = new UTF8Marshaler();
	}
}
