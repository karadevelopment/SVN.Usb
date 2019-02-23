using SVN.Usb.Constants;
using System.Runtime.InteropServices;

namespace SVN.Usb.Structures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct UsbHcdDriverkeyName
    {
        public int ActualLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UsbConstants.BUFFER_SIZE)]
        public string DriverKeyName;
    }
}