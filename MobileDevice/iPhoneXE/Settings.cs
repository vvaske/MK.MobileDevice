using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MK.MobileDevice.XEDevice.Properties
{
	// Token: 0x02000026 RID: 38
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0"), CompilerGenerated]
	public sealed class Settings : ApplicationSettingsBase
	{
		// Token: 0x17000034 RID: 52
		public static Settings Default
		{
			// Token: 0x06000141 RID: 321 RVA: 0x00008A6C File Offset: 0x00006C6C
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x17000036 RID: 54
		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string hasShownSuckyDLLErrorForVersion
		{
			// Token: 0x06000144 RID: 324 RVA: 0x00008AA0 File Offset: 0x00006CA0
			get
			{
				return (string)this["hasShownSuckyDLLErrorForVersion"];
			}
			// Token: 0x06000145 RID: 325 RVA: 0x000021F5 File Offset: 0x000003F5
			set
			{
				this["hasShownSuckyDLLErrorForVersion"] = value;
			}
		}

		// Token: 0x17000035 RID: 53
		[DefaultSettingValue(""), UserScopedSetting, DebuggerNonUserCode]
		public string iTunesMobileDeviceDllPath
		{
			// Token: 0x06000142 RID: 322 RVA: 0x00008A80 File Offset: 0x00006C80
			get
			{
				return (string)this["iTunesMobileDeviceDllPath"];
			}
			// Token: 0x06000143 RID: 323 RVA: 0x000021E7 File Offset: 0x000003E7
			set
			{
				this["iTunesMobileDeviceDllPath"] = value;
			}
		}

		// Token: 0x040000C6 RID: 198
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
