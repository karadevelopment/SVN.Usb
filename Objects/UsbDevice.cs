using SVN.Usb.Structures;

namespace SVN.Usb.Objects
{
    internal class UsbDevice : UsbBase
    {
        public string DriveName { get; set; }
        public int PortNumber { get; set; }
        public string DriverKey { get; set; } = string.Empty;
        public string HubDevicePath { get; set; } = string.Empty;
        public string InstanceID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Product { get; set; } = "Unknown USB Device";
        public string SerialNumber { get; set; } = string.Empty;
        public UsbDeviceDescriptor Descriptor { get; set; }

        public UsbDevice(UsbBase usbBase) : base(usbBase)
        {
        }
    }
}