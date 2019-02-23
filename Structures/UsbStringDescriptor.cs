using SVN.Usb.Constants;
using System.Runtime.InteropServices;

namespace SVN.Usb.Structures
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct UsbStringDescriptor
    {
        public byte bLength;
        public byte bDescriptorType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UsbConstants.MAXIMUM_USB_STRING_LENGTH)]
        public string bString;
    }
}