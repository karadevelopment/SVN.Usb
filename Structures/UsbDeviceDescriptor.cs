using System.Runtime.InteropServices;

namespace SVN.Usb.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct UsbDeviceDescriptor
    {
        public byte bLength;
        public byte bDescriptorType;
        public short bcdUSB;
        public byte bDeviceClass;
        public byte bDeviceSubClass;
        public byte bDeviceProtocol;
        public byte bMaxPacketSize0;
        public short idVendor;
        public short idProduct;
        public short bcdDevice;
        public byte iManufacturer;
        public byte iProduct;
        public byte iSerialNumber;
        public byte bNumConfigurations;
    }
}