using System.Runtime.InteropServices;

namespace SVN.Usb.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct UsbNodeConnectionInformationEx
    {
        public int ConnectionIndex;
        public UsbDeviceDescriptor DeviceDescriptor;
        public byte CurrentConfigurationValue;
        public byte Speed;
        public byte DeviceIsHub;
        public short DeviceAddress;
        public int NumberOfOpenPipes;
        public int ConnectionStatus;
    }
}