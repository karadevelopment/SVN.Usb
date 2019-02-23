using System.Runtime.InteropServices;

namespace SVN.Usb.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct UsbHubInformation
    {
        public UsbHubDescriptor HubDescriptor;
        public byte HubIsBusPowered;
    }
}