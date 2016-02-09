namespace MK.MobileDevice
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void DeviceRestoreNotificationCallback(ref AMRecoveryDevice callback_info);
    internal delegate void DeviceRestoreNotificationCallbackBKP(ref AMRecoveryDevice callback_info);
}

