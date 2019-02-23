using System.Runtime.InteropServices;

namespace SVN.Usb.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct StorageDeviceNumber
    {
        public int DeviceType;
        public int DeviceNumber;
        public int PartitionNumber;
    }
}