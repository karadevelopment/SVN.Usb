using System.Runtime.InteropServices;

namespace SVN.Usb.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct UsbSetupPacket
    {
        public byte bmRequest;
        public byte bRequest;
        public short wValue;
        public short wIndex;
        public short wLength;
    }
}