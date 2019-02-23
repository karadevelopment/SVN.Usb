using System.Runtime.InteropServices;

namespace SVN.Usb.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct UsbNodeInformation
    {
        public int NodeType;
        public UsbHubInformation HubInformation;
    }
}