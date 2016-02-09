using System;
using System.Collections.Generic;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x0200000D RID: 13
	public class XEPathInfo
	{
		// Token: 0x06000085 RID: 133 RVA: 0x0000579C File Offset: 0x0000399C
		public XEPathInfo(iPhoneXE device, string fullPath, string service, bool isAppRoot = false)
		{
			this.Device = device;
			this._fullPath = fullPath;
			this.Service = service;
			bool flag = !(service == "com.apple.afc") && !(service == "com.apple.afc2");
			if ((fullPath == "/" && flag) || isAppRoot)
			{
				this._isDirEnum = EnumFolderOrFile.Folder;
				this._propertiesDict = new Dictionary<string, string>();
				this._propertiesDict.Add("st_blocks", "0");
				this._propertiesDict.Add("st_ifmt", "S_IFDIR");
			}
			else
			{
				this._propertiesDict = device.GetFileInfo(this._fullPath, service);
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005B04 File Offset: 0x00003D04
		public List<XEPathInfo> children(bool refresh)
		{
			if (this.IsDirectory || refresh)
			{
				this._children = new List<XEPathInfo>();
				string[] contents = this.Device.GetContents(this._fullPath, this.Service, true);
				string[] array = contents;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					if (text != "." && text != "..")
					{
						XEPathInfo item = new XEPathInfo(this.Device, XEPathInfo.UnixPathCombine(this._fullPath, text), this.Service, false);
						this._children.Add(item);
					}
				}
			}
			return this._children;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005C20 File Offset: 0x00003E20
		public bool Equals(XEPathInfo otherInfo)
		{
			return this.Device == otherInfo.Device && !(this.Service != otherInfo.Service) && !(this._fullPath != otherInfo._fullPath) && !(this.DateModified != otherInfo.DateModified);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00005914 File Offset: 0x00003B14
		public static string UnixGetPathExtension(string path)
		{
			string result;
			if (path == null)
			{
				result = null;
			}
			else
			{
				int num = path.Length - 1;
				while (num >= 0 && path[num] != '.')
				{
					num--;
				}
				if (num < 0)
				{
					result = "";
				}
				else
				{
					num++;
					if (num >= path.Length)
					{
						result = "";
					}
					else
					{
						result = "." + path.Substring(num);
					}
				}
			}
			return result;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005994 File Offset: 0x00003B94
		public static string UnixGetPathWithoutExtension(string path)
		{
			int num = path.Length - 1;
			while (num >= 0 && path[num] != '.')
			{
				num--;
			}
			string result;
			if (num <= 0)
			{
				result = path;
			}
			else
			{
				result = path.Substring(0, num);
			}
			return result;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005ABC File Offset: 0x00003CBC
		public static string UnixIncrementPathString(string originalPath, int incrementNumber)
		{
			string text;
			string text2;
			string text3;
			XEPathInfo.UnixSplitFileNameAndExtension(originalPath, out text, out text2, out text3);
			return string.Format("{0}/{1} ({2}){3}", new object[]
			{
				text,
				text2,
				incrementNumber,
				text3
			});
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00005890 File Offset: 0x00003A90
		public static string UnixPathCombine(string str1, string str2)
		{
			string result;
			if (str1.Length == 0)
			{
				result = str2;
			}
			else if (str2.Length == 0)
			{
				result = "";
			}
			else
			{
				if (str1[str1.Length - 1] == '/')
				{
					str1 = str1.Substring(0, str1.Length - 1);
				}
				if (str2[0] == '/')
				{
					str2 = str2.Substring(1);
				}
				result = str1 + "/" + str2;
			}
			return result;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000059E0 File Offset: 0x00003BE0
		public static void UnixSplitFileNameAndExtension(string srcPath, out string pathBeforeFile, out string fileName, out string extension)
		{
			int num = srcPath.Length - 1;
			while (num >= 0 && srcPath[num] != '.')
			{
				num--;
			}
			if (num < 0)
			{
				num = srcPath.Length;
			}
			int num2 = num - 1;
			while (num2 >= 0 && srcPath[num2] != '/')
			{
				num2--;
			}
			if (num < srcPath.Length)
			{
				extension = srcPath.Substring(num);
			}
			else
			{
				extension = "";
			}
			if (num2 < num - 1)
			{
				fileName = srcPath.Substring(num2 + 1, num - num2 - 1);
			}
			else
			{
				fileName = "";
			}
			if (num2 == 0)
			{
				pathBeforeFile = "/";
			}
			else if (num2 < 0)
			{
				pathBeforeFile = "";
			}
			else
			{
				pathBeforeFile = srcPath.Substring(0, num2);
			}
		}

		// Token: 0x17000021 RID: 33
		public List<XEPathInfo> Children
		{
			// Token: 0x0600008D RID: 141 RVA: 0x00005BBC File Offset: 0x00003DBC
			get
			{
				return this.children(false);
			}
		}

		// Token: 0x17000026 RID: 38
		public virtual DateTime? DateModified
		{
			// Token: 0x06000093 RID: 147 RVA: 0x00005DE0 File Offset: 0x00003FE0
			get
			{
				DateTime? result = null;
				if (this._propertiesDict.ContainsKey("st_mtime"))
				{
					long num = long.Parse(this._propertiesDict["st_mtime"]);
					num /= 1000000000L;
					DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					result = new DateTime?(dateTime.AddSeconds((double)num).ToLocalTime());
				}
				return result;
			}
		}

		// Token: 0x17000020 RID: 32
		public bool Exists
		{
			// Token: 0x06000086 RID: 134 RVA: 0x00005868 File Offset: 0x00003A68
			get
			{
				return this._propertiesDict != null && this._propertiesDict.Count > 0;
			}
		}

		// Token: 0x17000027 RID: 39
		public virtual ulong FileLength
		{
			// Token: 0x06000094 RID: 148 RVA: 0x00005E64 File Offset: 0x00004064
			get
			{
				ulong result = 0uL;
				if (this._propertiesDict.ContainsKey("st_size"))
				{
					result = ulong.Parse(this._propertiesDict["st_size"]);
				}
				return result;
			}
		}

		// Token: 0x17000022 RID: 34
		public string Filename
		{
			// Token: 0x0600008E RID: 142 RVA: 0x00005BD4 File Offset: 0x00003DD4
			get
			{
				string result;
				if (this._fullPath == "/")
				{
					result = "/";
				}
				else
				{
					string[] array = this._fullPath.Split(new char[]
					{
						'/'
					});
					result = array[array.Length - 1];
				}
				return result;
			}
		}

		// Token: 0x17000023 RID: 35
		public string FullPath
		{
			// Token: 0x06000090 RID: 144 RVA: 0x00005CC4 File Offset: 0x00003EC4
			get
			{
				return this._fullPath;
			}
		}

		// Token: 0x17000024 RID: 36
		public bool IsDirectory
		{
			// Token: 0x06000091 RID: 145 RVA: 0x00005CDC File Offset: 0x00003EDC
			get
			{
				if (this._isDirEnum == EnumFolderOrFile.NotLoaded)
				{
					if (this._propertiesDict.ContainsKey("st_ifmt"))
					{
						string a = this._propertiesDict["st_ifmt"];
						if (a == "S_IFDIR")
						{
							this._isDirEnum = EnumFolderOrFile.Folder;
						}
						if (a == "S_IFLNK")
						{
							this._isDirEnum = EnumFolderOrFile.Alias;
						}
						if (a == "S_IFREG")
						{
							this._isDirEnum = EnumFolderOrFile.File;
						}
					}
					else
					{
						this._isDirEnum = EnumFolderOrFile.Other;
					}
				}
				return this._isDirEnum == EnumFolderOrFile.Folder || this._isDirEnum == EnumFolderOrFile.Alias;
			}
		}

		// Token: 0x17000025 RID: 37
		public virtual string Type
		{
			// Token: 0x06000092 RID: 146 RVA: 0x00005D80 File Offset: 0x00003F80
			get
			{
				string result = "";
				switch (this._isDirEnum)
				{
				case EnumFolderOrFile.NotLoaded:
					result = "Not Loaded";
					break;
				case EnumFolderOrFile.Folder:
					result = "Folder";
					break;
				case EnumFolderOrFile.File:
					result = "File";
					break;
				case EnumFolderOrFile.Alias:
					result = "Alias";
					break;
				case EnumFolderOrFile.Other:
					result = "Other";
					break;
				}
				return result;
			}
		}

		// Token: 0x0400004C RID: 76
		public readonly iPhoneXE Device;

		// Token: 0x0400004E RID: 78
		public readonly string Service;

		// Token: 0x0400004F RID: 79
		private List<XEPathInfo> _children = null;

		// Token: 0x0400004D RID: 77
		private string _fullPath;

		// Token: 0x04000051 RID: 81
		private EnumFolderOrFile _isDirEnum = EnumFolderOrFile.NotLoaded;

		// Token: 0x04000050 RID: 80
		private Dictionary<string, string> _propertiesDict = null;
	}
}
