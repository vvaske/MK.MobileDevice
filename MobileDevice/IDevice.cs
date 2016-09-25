using System;
using System.Collections.Generic;

namespace imobileDeviceiDevice
{
    public class iDevice
    {
        #region Device Hardware
        public static Dictionary<string, string> DeviceHardware = new Dictionary<string, string>()
        {
        	//iPhones
            { "iPhone1,1", "iPhone 1G" },
            { "iPhone1,2", "iPhone 3G" },
            { "iPhone2,1", "iPhone 3GS" },
            { "iPhone3,1", "iPhone 4" },
            { "iPhone3,2", "iPhone 4" },
            { "iPhone3,3", "iPhone 4" },
            { "iPhone4,1", "iPhone 4S" },
            { "iPhone5,1", "iPhone 5" },
            { "iPhone5,2", "iPhone 5" },
            { "iPhone5,3", "iPhone 5C" },
            { "iPhone5,4", "iPhone 5C" },
            { "iPhone6,1", "iPhone 5S" },
            { "iPhone6,2", "iPhone 5S" },
            { "iPhone7,1", "iPhone 6 Plus" },
            { "iPhone7,2", "iPhone 6" },
            { "iPhone8,1", "iPhone 6S" },
            { "iPhone8,2", "iPhone 6S Plus" },
	    { "iPhone8,4", "iPhone SE" },
            { "iPhone9,1", "iPhone 7" },
            { "iPhone9,2", "iPhone 7 Plus" },
            { "iPhone9,3", "iPhone 7" },
            { "iPhone9,4", "iPhone 7 Plus" },
		
            //iPod Touches
            { "iPod1,1", "iPod Touch 1G" },
            { "iPod2,1", "iPod Touch 2G" },
            { "iPod3,1", "iPod Touch 3G" },
            { "iPod4,1", "iPod Touch 4G" },
            { "iPod5,1", "iPod Touch 5G" },
            { "iPod7,1", "iPod Touch 6G" },
			
            //iPads
            { "iPad1,1", "iPad 1G" },
            { "iPad2,1", "iPad 2" },
            { "iPad2,2", "iPad 2" },
            { "iPad2,3", "iPad 2" },
            { "iPad2,4", "iPad 2" },
            { "iPad3,1", "iPad 3G" },
            { "iPad3,2", "iPad 3G" },
            { "iPad3,3", "iPad 3G" },
            { "iPad3,4", "iPad 4G" },
            { "iPad3,5", "iPad 4G" },
            { "iPad3,6", "iPad 4G" },
            { "iPad4,1", "iPad Air" },
            { "iPad4,2", "iPad Air" },
            { "iPad4,3", "iPad Air" },
            { "iPad5,3", "iPad Air 2" },
            { "iPad5,4", "iPad Air 2" },
            { "iPad6,3", "iPad Pro 9.7" },
            { "iPad6,4", "iPad Pro 9.7" },
            { "iPad6,7", "iPad Pro 12.9" },
            { "iPad6,8", "iPad Pro 12.9" },

            { "iPad2,5", "iPad Mini 1G" },
            { "iPad2,6", "iPad Mini 1G" },
            { "iPad2,7", "iPad Mini 1G" },
            { "iPad4,4", "iPad Mini 2" },
            { "iPad4,5", "iPad Mini 2" },
            { "iPad4,6", "iPad Mini 2" },
            { "iPad4,7", "iPad Mini 3" },
            { "iPad4,8", "iPad Mini 3" },
            { "iPad4,9", "iPad Mini 3" },
            { "iPad5,1", "iPad Mini 4" },
            { "iPad5,2", "iPad Mini 4" },

            { "AppleTV2,1", "Apple TV 2G" },
            { "AppleTV3,1", "Apple TV 3G" },
            { "AppleTV3,2", "Apple TV 3G Rev A" }
        };
        #endregion

        public bool Connected;

        public IntPtr Handle;
        public IntPtr LockdownClient;
        public IntPtr InstallationProxyService;
        public IntPtr InstallationProxyClient;

        public string Udid;
        public string SerialNumber;
        public string Name;
        public string ProductType;
        public string ProductName;
        public string iOSVersion;
        
        public iDevice(IntPtr handle, string udid)
        {
            this.Connected = true;
            this.Handle = handle;
            this.Udid = udid;
        }

        public iDevice(IntPtr handle, string udid, string serialNumber, string name, string productType)
        {
            this.Connected = true;
            this.Handle = handle;
            this.Udid = udid;
            this.SerialNumber = serialNumber;
            this.Name = name;
            this.ProductType = productType;
            this.ProductName = DeviceHardware[productType];
        }
    }
}
