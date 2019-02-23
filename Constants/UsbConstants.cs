namespace SVN.Usb.Constants
{
    internal static class UsbConstants
    {
        public const int GENERIC_WRITE = 0x40000000;
        public const int FILE_SHARE_READ = 0x1;
        public const int FILE_SHARE_WRITE = 0x2;
        public const int OPEN_EXISTING = 0x3;
        public const int INVALID_HANDLE_VALUE = -1;

        public const int IOCTL_GET_HCD_DRIVERKEY_NAME = 0x220424;
        public const int IOCTL_USB_GET_ROOT_HUB_NAME = 0x220408;
        public const int IOCTL_USB_GET_NODE_INFORMATION = 0x220408;
        public const int IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX = 0x220448;
        public const int IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION = 0x220410;
        public const int IOCTL_USB_GET_NODE_CONNECTION_NAME = 0x220414;
        public const int IOCTL_USB_GET_NODE_CONNECTION_DRIVERKEY_NAME = 0x220420;

        public const int USB_DEVICE_DESCRIPTOR_TYPE = 0x1;
        public const int USB_STRING_DESCRIPTOR_TYPE = 0x3;

        public const int BUFFER_SIZE = 2048;
        public const int MAXIMUM_USB_STRING_LENGTH = 255;

        public const string GUID_DEVINTERFACE_HUBCONTROLLER = "3abf6f2d-71c4-462a-8a92-1e6861e6af27";
        public const string REGSTR_KEY_USB = "USB";
        public const int DIGCF_PRESENT = 0x2;
        public const int DIGCF_ALLCLASSES = 0x4;
        public const int DIGCF_DEVICEINTERFACE = 0x10;
        public const int SPDRP_DRIVER = 0x9;
        public const int SPDRP_DEVICEDESC = 0x0;
        public const int REG_SZ = 1;

        public const int IOCTL_STORAGE_GET_DEVICE_NUMBER = 0x2D1080;
        public const string GUID_DEVINTERFACE_DISK = "53f56307-b6bf-11d0-94f2-00a0c91efb8b";
    }
}