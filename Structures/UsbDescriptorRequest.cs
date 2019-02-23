using System.Runtime.InteropServices;

namespace SVN.Usb.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct UsbDescriptorRequest
    {
        public int ConnectionIndex;
        public UsbSetupPacket SetupPacket;
    }
}