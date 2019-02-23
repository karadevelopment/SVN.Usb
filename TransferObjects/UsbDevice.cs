namespace SVN.Usb.TransferObjects
{
    public class UsbDevice
    {
        public string Address { get; set; }
        public string DriveName { get; set; }
        public int PortNumber { get; set; }
        public string DriverKey { get; set; }
        public string HubDevicePath { get; set; }
        public string InstanceID { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Product { get; set; }
        public string SerialNumber { get; set; }
    }
}