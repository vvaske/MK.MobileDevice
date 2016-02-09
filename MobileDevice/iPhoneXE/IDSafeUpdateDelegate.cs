using System;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x0200001D RID: 29
	public interface IDSafeUpdateDelegate
	{
		// Token: 0x060000E5 RID: 229
		void HandleTransferUpdate(bool isFromDevToComp, string devID, string compPath, string devPath, string devService, double fraction);
	}
}
