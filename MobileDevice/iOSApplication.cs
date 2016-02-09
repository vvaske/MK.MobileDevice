/*

 */
using System;

namespace MK.MobileDevice
{
	/// <summary>
	/// iOS Application
	/// </summary>
    public class iOSApplication
    {
        public string Type;
        public string Name;
        public string Version;
        public string Identifier;
        public string ExecutableName;
        public string ApplicationIdentifier;
        public string StaticDiskUsage;
        public string DynamicDiskUsage;

        public iOSApplication(string type, string name, string version, string identifier, string executableName, string appid, string staticsize, string dynsize)
        {
            this.Type = type;
            this.Name = name;
            this.Version = version;
            this.Identifier = identifier;
            this.ExecutableName = executableName;
            this.ApplicationIdentifier = appid;
            this.StaticDiskUsage=staticsize;
            this.DynamicDiskUsage=dynsize;
        }
    }

}
