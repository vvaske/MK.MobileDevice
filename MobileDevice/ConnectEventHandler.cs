namespace MK.MobileDevice
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void ConnectEventHandler(object sender, ConnectEventArgs args);
    public delegate void DeviceAttachedHandler(object sender, USBMultiplexArgs args);
    public delegate void ITMDConnectEventHandler(object sender, ITMDConnectEventArgs args);
}

