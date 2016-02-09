using System;
using System.IO;
using System.Text;


namespace MK.MobileDevice.XEDevice
{
	// Token: 0x0200000B RID: 11
	public unsafe class XEFileInfo : Stream
	{
		// Token: 0x06000074 RID: 116 RVA: 0x0000209B File Offset: 0x0000029B
		private XEFileInfo(iPhoneXE phone, long handle, OpenMode mode)
		{
			this.phone = phone;
			this.mode = mode;
			this.handle = handle;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000054C0 File Offset: 0x000036C0
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.handle != 0L)
			{
				MobileDeviceBase.AFCFileRefClose(this.phone.AFCHandle, this.handle);
				this.handle = 0L;
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000020CF File Offset: 0x000002CF
		public override void Flush()
		{
			MobileDeviceBase.AFCFlushData(this.phone.AFCHandle, this.handle);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000056A8 File Offset: 0x000038A8
		public static XEFileInfo Open(iPhoneXE phone, string path, FileAccess openmode)
		{
			OpenMode openMode;
			long num2;
			lock (phone)
			{
				openMode = OpenMode.None;
				switch (openmode)
				{
				case FileAccess.Read:
					openMode = OpenMode.Read;
					break;
				case FileAccess.Write:
					openMode = OpenMode.Write;
					break;
				case FileAccess.ReadWrite:
					throw new NotImplementedException("Read+Write not (yet) implemented");
				}
				int num = MobileDeviceBase.AFCFileRefOpen(phone.AFCHandle, Encoding.UTF8.GetBytes(path), (ulong)((long)openMode), out num2);
				if (num != 0 && num != 0)
				{
					LogViewer.LogEvent(0, "**ERROR:  " + path);
					throw new IOException("AFCFileRefOpen failed with error " + num.ToString());
				}
			}
			return new XEFileInfo(phone, num2, openMode);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000576C File Offset: 0x0000396C
		public static XEFileInfo OpenRead(iPhoneXE phone, string path)
		{
			return XEFileInfo.Open(phone, path, FileAccess.Read);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00005784 File Offset: 0x00003984
		public static XEFileInfo OpenWrite(iPhoneXE phone, string path)
		{
			return XEFileInfo.Open(phone, path, FileAccess.Write);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005514 File Offset: 0x00003714
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!this.CanRead)
			{
				throw new NotImplementedException("Stream open for writing only");
			}
			int result;
			lock (this.phone)
			{
				if (offset != 0)
				{
					MobileDeviceBase.AFCFileRefSeek(this.phone.AFCHandle, this.handle, (long)offset, 0L);
				}
				uint num = (uint)count;
				if (MobileDeviceBase.AFCFileRefRead(this.phone.AFCHandle, this.handle, buffer, ref num) != 0)
				{
					throw new IOException("AFCFileRefRead error..");
				}
				result = (int)num;
			}
			return result;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005644 File Offset: 0x00003844
		public override long Seek(long offset, SeekOrigin origin)
		{
			lock (this.phone)
			{
				MobileDeviceBase.AFCFileRefSeek(this.phone.AFCHandle, this.handle, (long)((ulong)((uint)offset)), 0L);
			}
			return offset;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00005498 File Offset: 0x00003698
		public override void SetLength(long value)
		{
			MobileDeviceBase.AFCFileRefSetFileSize(this.phone.AFCHandle, this.handle, (uint)value);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000055BC File Offset: 0x000037BC
		public override void Write(byte[] buffer, int offset, int count)
		{
			lock (this.phone)
			{
				if (!this.CanWrite)
				{
					throw new NotImplementedException("Stream open for reading only");
				}
				byte[] array;
				if (offset == 0)
				{
					array = buffer;
				}
				else
				{
					array = new byte[count];
					Buffer.BlockCopy(buffer, offset, array, 0, count);
				}
				MobileDeviceBase.AFCFileRefWrite(this.phone.AFCHandle, this.handle, array, (uint)count);
			}
		}

		// Token: 0x1700001A RID: 26
		public override bool CanRead
		{
			// Token: 0x06000075 RID: 117 RVA: 0x000053D8 File Offset: 0x000035D8
			get
			{
				return this.mode == OpenMode.Read || this.mode == OpenMode.ReadSafe || this.mode == OpenMode.ReadWrite;
			}
		}

		// Token: 0x1700001B RID: 27
		public override bool CanSeek
		{
			// Token: 0x06000076 RID: 118 RVA: 0x00005404 File Offset: 0x00003604
			get
			{
				return false;
			}
		}

		// Token: 0x1700001C RID: 28
		public override bool CanTimeout
		{
			// Token: 0x06000077 RID: 119 RVA: 0x00005414 File Offset: 0x00003614
			get
			{
				return true;
			}
		}

		// Token: 0x1700001D RID: 29
		public override bool CanWrite
		{
			// Token: 0x06000078 RID: 120 RVA: 0x00005424 File Offset: 0x00003624
			get
			{
				return this.mode == OpenMode.Write;
			}
		}

		// Token: 0x1700001E RID: 30
		public override long Length
		{
			// Token: 0x06000079 RID: 121 RVA: 0x000020B8 File Offset: 0x000002B8
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		// Token: 0x1700001F RID: 31
		public override long Position
		{
			// Token: 0x0600007A RID: 122 RVA: 0x0000543C File Offset: 0x0000363C
			get
			{
				long result;
				lock (this.phone)
				{
					uint num = 0u;
					MobileDeviceBase.AFCFileRefTell(this.phone.AFCHandle, this.handle, ref num);
					result = (long)((ulong)num);
				}
				return result;
			}
			// Token: 0x0600007B RID: 123 RVA: 0x000020C4 File Offset: 0x000002C4
			set
			{
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		// Token: 0x04000044 RID: 68
		private long handle;

		// Token: 0x04000043 RID: 67
		private OpenMode mode;

		// Token: 0x04000045 RID: 69
		private iPhoneXE phone;
	}
}
