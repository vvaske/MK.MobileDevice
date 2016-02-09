using System;
using System.Text.RegularExpressions;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x02000022 RID: 34
	public class ProductInfo
	{
		// Token: 0x06000137 RID: 311 RVA: 0x00002050 File Offset: 0x00000250
		protected ProductInfo()
		{
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00008910 File Offset: 0x00006B10
		private void DiscoverColor(string deviceColor, string enclosureColor)
		{
			if (deviceColor == "black")
			{
				this.Color = ProductInfo.ColorType.Black;
			}
			else if (deviceColor == "white")
			{
				this.Color = ProductInfo.ColorType.White;
			}
			else if (!string.IsNullOrEmpty(enclosureColor))
			{
				this.DiscoverEnclosureColor(enclosureColor);
			}
			else
			{
				this.Color = ProductInfo.ColorType.Black;
			}
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00008968 File Offset: 0x00006B68
		private void DiscoverEnclosureColor(string enclosureColor)
		{
			if (enclosureColor == "#99989b" || enclosureColor == "#b4b5b9")
			{
				this.Color = ProductInfo.ColorType.SpaceGray;
			}
			else if (enclosureColor == "#d7d9d8")
			{
				this.Color = ProductInfo.ColorType.Silver;
			}
			else if (enclosureColor == "#d4c5b3")
			{
				this.Color = ProductInfo.ColorType.Gold;
			}
			else if (enclosureColor == "#f5f4f7")
			{
				this.Color = ProductInfo.ColorType.White;
			}
			else if (enclosureColor == "#fe767a")
			{
				this.Color = ProductInfo.ColorType.Pink;
			}
			else if (enclosureColor == "#faf189")
			{
				this.Color = ProductInfo.ColorType.Yellow;
			}
			else if (enclosureColor == "#46abe0")
			{
				this.Color = ProductInfo.ColorType.Blue;
			}
			else if (enclosureColor == "#a1e877")
			{
				this.Color = ProductInfo.ColorType.Green;
			}
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00008748 File Offset: 0x00006948
		private void DiscoverIPadModel(string productName, int combinedRev)
		{
			switch (combinedRev)
			{
			case 11:
				this.Model = ProductInfo.ModelType.iPad;
				return;
			case 21:
			case 22:
			case 23:
			case 24:
				this.Model = ProductInfo.ModelType.iPad2;
				return;
			case 25:
			case 26:
			case 27:
				this.Model = ProductInfo.ModelType.iPadMini;
				return;
			case 31:
			case 32:
			case 33:
				this.Model = ProductInfo.ModelType.iPad3;
				return;
			case 34:
			case 35:
			case 36:
				this.Model = ProductInfo.ModelType.iPad4;
				return;
			case 41:
			case 42:
			case 43:
				this.Model = ProductInfo.ModelType.iPadAir;
				return;
			case 44:
			case 45:
			case 46:
				this.Model = ProductInfo.ModelType.iPadMini2;
				return;
			case 47:
			case 48:
			case 49:
				this.Model = ProductInfo.ModelType.iPadMini3;
				return;
			case 53:
			case 54:
				this.Model = ProductInfo.ModelType.iPadAir2;
				return;
			}
			this.Model = ProductInfo.ModelType.iPadUnknown;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00008644 File Offset: 0x00006844
		private void DiscoverIPhoneModel(string productName, int combinedRev)
		{
			if (combinedRev <= 33)
			{
				switch (combinedRev)
				{
				case 11:
					this.Model = ProductInfo.ModelType.iPhone;
					return;
				case 12:
					this.Model = ProductInfo.ModelType.iPhone3G;
					return;
				default:
					if (combinedRev == 21)
					{
						this.Model = ProductInfo.ModelType.iPhone3Gs;
						return;
					}
					switch (combinedRev)
					{
					case 31:
					case 32:
					case 33:
						this.Model = ProductInfo.ModelType.iPhone4;
						return;
					}
					break;
				}
			}
			else if (combinedRev <= 54)
			{
				if (combinedRev == 41)
				{
					this.Model = ProductInfo.ModelType.iPhone4s;
					return;
				}
				switch (combinedRev)
				{
				case 51:
				case 52:
					this.Model = ProductInfo.ModelType.iPhone5;
					return;
				case 53:
				case 54:
					this.Model = ProductInfo.ModelType.iPhone5c;
					return;
				}
			}
			else
			{
				switch (combinedRev)
				{
				case 61:
				case 62:
					this.Model = ProductInfo.ModelType.iPhone5s;
					return;
				default:
					switch (combinedRev)
					{
					case 71:
						this.Model = ProductInfo.ModelType.iPhone6Plus;
						return;
					case 72:
						this.Model = ProductInfo.ModelType.iPhone6;
						return;
					}
					break;
				}
			}
			this.Model = ProductInfo.ModelType.iPhoneUnknown;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00008894 File Offset: 0x00006A94
		private void DiscoverIPodTouchModel(string productName, int combinedRev)
		{
			if (combinedRev <= 21)
			{
				if (combinedRev == 11)
				{
					this.Model = ProductInfo.ModelType.iPodTouch;
					return;
				}
				if (combinedRev == 21)
				{
					this.Model = ProductInfo.ModelType.iPodTouch2;
					return;
				}
			}
			else
			{
				if (combinedRev == 31)
				{
					this.Model = ProductInfo.ModelType.iPodTouch3;
					return;
				}
				if (combinedRev == 41)
				{
					this.Model = ProductInfo.ModelType.iPodTouch4;
					return;
				}
				if (combinedRev == 51)
				{
					this.Model = ProductInfo.ModelType.iPodTouch5;
					return;
				}
			}
			this.Model = ProductInfo.ModelType.iPodTouchUnknown;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00008574 File Offset: 0x00006774
		private void DiscoverModel(string productType)
		{
			Match match = Regex.Match(productType, "(.*?)(\\d+),(\\d+)");
			if (!match.Success)
			{
				this.Model = ProductInfo.ModelType.Unknown;
			}
			else
			{
				string value = match.Groups[1].Value;
				int num = int.Parse(match.Groups[2].Value);
				int num2 = int.Parse(match.Groups[3].Value);
				int combinedRev = num * 10 + num2;
				if (value == "iPhone")
				{
					this.DiscoverIPhoneModel(value, combinedRev);
				}
				else if (value == "iPad")
				{
					this.DiscoverIPadModel(value, combinedRev);
				}
				else if (value == "iPod")
				{
					this.DiscoverIPodTouchModel(value, combinedRev);
				}
				else
				{
					this.Model = ProductInfo.ModelType.Unknown;
				}
			}
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00008518 File Offset: 0x00006718
		public static ProductInfo GetProductInfoForDevice(iPhoneXE device)
		{
			string productType = device.CopyDeviceValue("ProductType");
			string deviceColor = device.CopyDeviceValue("DeviceColor");
			string enclosureColor = device.CopyDeviceValue("DeviceEnclosureColor");
			device.CopyDeviceValue("ProductVersion");
			ProductInfo productInfo = new ProductInfo();
			productInfo.DiscoverModel(productType);
			productInfo.DiscoverColor(deviceColor, enclosureColor);
			return productInfo;
		}

		// Token: 0x17000032 RID: 50
		public ProductInfo.ColorType Color
		{
			// Token: 0x06000135 RID: 309 RVA: 0x00008500 File Offset: 0x00006700
			get;
			// Token: 0x06000136 RID: 310 RVA: 0x000021CF File Offset: 0x000003CF
			private set;
		}

		// Token: 0x17000031 RID: 49
		public ProductInfo.ModelType Model
		{
			// Token: 0x06000133 RID: 307 RVA: 0x000084E8 File Offset: 0x000066E8
			get;
			// Token: 0x06000134 RID: 308 RVA: 0x000021C6 File Offset: 0x000003C6
			private set;
		}

		// Token: 0x02000024 RID: 36
		public enum ColorType
		{
			// Token: 0x040000BB RID: 187
			Unknown,
			// Token: 0x040000BC RID: 188
			Black,
			// Token: 0x040000BD RID: 189
			White,
			// Token: 0x040000BE RID: 190
			SpaceGray,
			// Token: 0x040000BF RID: 191
			Silver,
			// Token: 0x040000C0 RID: 192
			Gold,
			// Token: 0x040000C1 RID: 193
			Pink,
			// Token: 0x040000C2 RID: 194
			Yellow,
			// Token: 0x040000C3 RID: 195
			Blue,
			// Token: 0x040000C4 RID: 196
			Green
		}

		// Token: 0x02000023 RID: 35
		public enum ModelType
		{
			// Token: 0x0400009E RID: 158
			Unknown,
			// Token: 0x0400009F RID: 159
			iPhone = 100,
			// Token: 0x040000A0 RID: 160
			iPhone3G,
			// Token: 0x040000A1 RID: 161
			iPhone3Gs,
			// Token: 0x040000A2 RID: 162
			iPhone4,
			// Token: 0x040000A3 RID: 163
			iPhone4s,
			// Token: 0x040000A4 RID: 164
			iPhone5,
			// Token: 0x040000A5 RID: 165
			iPhone5c,
			// Token: 0x040000A6 RID: 166
			iPhone5s,
			// Token: 0x040000A7 RID: 167
			iPhone6,
			// Token: 0x040000A8 RID: 168
			iPhone6Plus,
			// Token: 0x040000A9 RID: 169
			iPhoneUnknown,
			// Token: 0x040000AA RID: 170
			iPad = 200,
			// Token: 0x040000AB RID: 171
			iPad2,
			// Token: 0x040000AC RID: 172
			iPad3,
			// Token: 0x040000AD RID: 173
			iPad4,
			// Token: 0x040000AE RID: 174
			iPadAir,
			// Token: 0x040000AF RID: 175
			iPadAir2,
			// Token: 0x040000B0 RID: 176
			iPadMini,
			// Token: 0x040000B1 RID: 177
			iPadMini2,
			// Token: 0x040000B2 RID: 178
			iPadMini3,
			// Token: 0x040000B3 RID: 179
			iPadUnknown,
			// Token: 0x040000B4 RID: 180
			iPodTouch = 300,
			// Token: 0x040000B5 RID: 181
			iPodTouch2,
			// Token: 0x040000B6 RID: 182
			iPodTouch3,
			// Token: 0x040000B7 RID: 183
			iPodTouch4,
			// Token: 0x040000B8 RID: 184
			iPodTouch5,
			// Token: 0x040000B9 RID: 185
			iPodTouchUnknown
		}
	}
}
