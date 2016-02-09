using imobileDeviceiDevice;

namespace MK.MobileDevice
{
    using System;

    public class ConnectEventArgs : EventArgs
    {
        private IntPtr device;
        private IntPtr message;

        internal ConnectEventArgs(LibiMobileDevice.idevice_event_t cbi)
        {
            this.message = cbi.connection_type;
            this.device = cbi.UniqueDeviceID;
        }

        public IntPtr Device
        {
            get
            {
                return this.device;
            }
        }

        public IntPtr Message
        {
            get
            {
                return this.message;
            }
        }
    }

    public class USBMultiplexArgs : EventArgs
    {
        private IntPtr device;
        private IntPtr message;
        public bool IsLocked;

        internal USBMultiplexArgs(LibiMobileDevice.idevice_event_t cbi, bool locked)
        {
            this.message = cbi.connection_type;
            this.device = cbi.UniqueDeviceID;
            IsLocked = locked;
        }

        public IntPtr Device
        {
            get
            {
                return this.device;
            }
        }

        public IntPtr Message
        {
            get
            {
                return this.message;
            }
        }
    }

    public class ITMDConnectEventArgs : EventArgs
    {
        private unsafe void* device;
        private NotificationMessage message;

        internal unsafe ITMDConnectEventArgs(ITMDAMDeviceNotificationCallbackInfo cbi)
        {
            this.message = cbi.msg;
            this.device = cbi.dev;
        }

        public unsafe void* Device
        {
            get
            {
                return this.device;
            }
        }

        public NotificationMessage Message
        {
            get
            {
                return this.message;
            }
        }
    }
}

