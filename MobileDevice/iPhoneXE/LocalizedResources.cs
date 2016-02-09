using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x0200000E RID: 14
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), DebuggerNonUserCode, CompilerGenerated]
	public class LocalizedResources
	{
		// Token: 0x17000029 RID: 41
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static CultureInfo Culture
		{
			// Token: 0x06000097 RID: 151 RVA: 0x00005EF0 File Offset: 0x000040F0
			get
			{
				return LocalizedResources.resourceCulture;
			}
			// Token: 0x06000098 RID: 152 RVA: 0x000020E8 File Offset: 0x000002E8
			set
			{
				LocalizedResources.resourceCulture = value;
			}
		}

		// Token: 0x17000028 RID: 40
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ResourceManager ResourceManager
		{
			// Token: 0x06000096 RID: 150 RVA: 0x00005EAC File Offset: 0x000040AC
			get
			{
				if (object.ReferenceEquals(LocalizedResources.resourceMan, null))
				{
					ResourceManager resourceManager = new ResourceManager("MK.MobileDevice.XEDevice.LocalizedResources", typeof(LocalizedResources).Assembly);
					LocalizedResources.resourceMan = resourceManager;
				}
				return LocalizedResources.resourceMan;
			}
		}

		// Token: 0x1700002A RID: 42
		public static string txtItunesMobileDeviceDLLNotFound
		{
			// Token: 0x06000099 RID: 153 RVA: 0x00005F04 File Offset: 0x00004104
			get
			{
				return LocalizedResources.ResourceManager.GetString("txtItunesMobileDeviceDLLNotFound", LocalizedResources.resourceCulture);
			}
		}

		// Token: 0x1700002B RID: 43
		public static string txtUnableToFindiTunesMobileDeviceDll
		{
			// Token: 0x0600009A RID: 154 RVA: 0x00005F28 File Offset: 0x00004128
			get
			{
				return LocalizedResources.ResourceManager.GetString("txtUnableToFindiTunesMobileDeviceDll", LocalizedResources.resourceCulture);
			}
		}

		// Token: 0x1700002C RID: 44
		public static string txtUnableToStartIPhoneListner
		{
			// Token: 0x0600009B RID: 155 RVA: 0x00005F4C File Offset: 0x0000414C
			get
			{
				return LocalizedResources.ResourceManager.GetString("txtUnableToStartIPhoneListner", LocalizedResources.resourceCulture);
			}
		}

		// Token: 0x1700002D RID: 45
		public static string txtWhereIsITunesMobileDeviceDLL
		{
			// Token: 0x0600009C RID: 156 RVA: 0x00005F70 File Offset: 0x00004170
			get
			{
				return LocalizedResources.ResourceManager.GetString("txtWhereIsITunesMobileDeviceDLL", LocalizedResources.resourceCulture);
			}
		}

		// Token: 0x04000053 RID: 83
		private static CultureInfo resourceCulture;

		// Token: 0x04000052 RID: 82
		private static ResourceManager resourceMan;
	}
}
