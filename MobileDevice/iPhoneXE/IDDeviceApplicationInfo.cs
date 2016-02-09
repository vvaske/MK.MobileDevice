using System;
using System.Collections.Generic;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x02000007 RID: 7
	public class XEDeviceApplicationInfo
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00002058 File Offset: 0x00000258
		public XEDeviceApplicationInfo(string bundleID, Dictionary<string, object> infoDict)
		{
			this._bundleInfoDict = infoDict;
			this._bundleID = bundleID;
		}

		// Token: 0x17000007 RID: 7
		public string AppFolder
		{
			// Token: 0x06000023 RID: 35 RVA: 0x00002850 File Offset: 0x00000A50
			get
			{
				string result;
				if (this._bundleInfoDict.ContainsKey("Path"))
				{
					result = (string)this._bundleInfoDict["Path"];
				}
				else
				{
					result = "";
				}
				return result;
			}
		}

		// Token: 0x17000002 RID: 2
		public string ApplicationType
		{
			// Token: 0x0600001E RID: 30 RVA: 0x00002720 File Offset: 0x00000920
			get
			{
				string result;
				if (this._bundleInfoDict.ContainsKey("ApplicationType"))
				{
					result = (string)this._bundleInfoDict["ApplicationType"];
				}
				else
				{
					result = "";
				}
				return result;
			}
		}

		// Token: 0x17000001 RID: 1
		public string BundleID
		{
			// Token: 0x0600001D RID: 29 RVA: 0x00002708 File Offset: 0x00000908
			get
			{
				return this._bundleID;
			}
		}

		// Token: 0x17000004 RID: 4
		public string BundleName
		{
			// Token: 0x06000020 RID: 32 RVA: 0x00002784 File Offset: 0x00000984
			get
			{
				string result;
				if (this._bundleInfoDict.ContainsKey("CFBundleName"))
				{
					result = (string)this._bundleInfoDict["CFBundleName"];
				}
				else
				{
					result = "";
				}
				return result;
			}
		}

		// Token: 0x17000005 RID: 5
		public string DisplayName
		{
			// Token: 0x06000021 RID: 33 RVA: 0x000027C8 File Offset: 0x000009C8
			get
			{
				string result;
				if (this._bundleInfoDict.ContainsKey("CFBundleDisplayName"))
				{
					result = (string)this._bundleInfoDict["CFBundleDisplayName"];
				}
				else
				{
					result = "";
				}
				return result;
			}
		}

		// Token: 0x1700000A RID: 10
		private Dictionary<string, object> Entitlements
		{
			// Token: 0x06000026 RID: 38 RVA: 0x0000291C File Offset: 0x00000B1C
			get
			{
				if (this._entitlements == null && this._bundleInfoDict.ContainsKey("Entitlements"))
				{
					this._entitlements = (Dictionary<string, object>)this._bundleInfoDict["Entitlements"];
				}
				return this._entitlements;
			}
		}

		// Token: 0x17000008 RID: 8
		public bool FileSharingEnabled
		{
			// Token: 0x06000024 RID: 36 RVA: 0x00002894 File Offset: 0x00000A94
			get
			{
				return this._bundleInfoDict.ContainsKey("UIFileSharingEnabled") && (bool)this._bundleInfoDict["UIFileSharingEnabled"];
			}
		}

		// Token: 0x1700000B RID: 11
		public string ICloudDocumentIDs
		{
			// Token: 0x06000027 RID: 39 RVA: 0x0000296C File Offset: 0x00000B6C
			get
			{
				string result;
				if (this.Entitlements != null)
				{
					result = (string)this.Entitlements["com.apple.developer.ubiquity-container-identifiers"];
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x1700000C RID: 12
		public string ICloudKVID
		{
			// Token: 0x06000028 RID: 40 RVA: 0x000029A0 File Offset: 0x00000BA0
			get
			{
				string result;
				if (this.Entitlements != null)
				{
					result = (string)this.Entitlements["com.apple.developer.ubiquity-kvstore-identifier"];
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		// Token: 0x17000006 RID: 6
		public string IconPath
		{
			// Token: 0x06000022 RID: 34 RVA: 0x0000280C File Offset: 0x00000A0C
			get
			{
				string result;
				if (this._bundleInfoDict.ContainsKey("CFBundleIconFile"))
				{
					result = (string)this._bundleInfoDict["CFBundleIconFile"];
				}
				else
				{
					result = "";
				}
				return result;
			}
		}

		// Token: 0x17000003 RID: 3
		public bool IsAppTypeUser
		{
			// Token: 0x0600001F RID: 31 RVA: 0x00002764 File Offset: 0x00000964
			get
			{
				return this.ApplicationType == "User";
			}
		}

		// Token: 0x17000009 RID: 9
		public bool IsFromAppStore
		{
			// Token: 0x06000025 RID: 37 RVA: 0x000028D4 File Offset: 0x00000AD4
			get
			{
				return this._bundleInfoDict.ContainsKey("SignerIdentity") && (string)this._bundleInfoDict["SignerIdentity"] == "Apple iPhone OS Application Signing";
			}
		}

		// Token: 0x0400001C RID: 28
		private string _bundleID;

		// Token: 0x0400001B RID: 27
		private Dictionary<string, object> _bundleInfoDict = null;

		// Token: 0x0400001D RID: 29
		private Dictionary<string, object> _entitlements = null;
	}
}
