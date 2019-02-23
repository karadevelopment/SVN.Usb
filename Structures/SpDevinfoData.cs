using System;
using System.Runtime.InteropServices;

namespace SVN.Usb.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SpDevinfoData
    {
        public int cbSize;
        public Guid ClassGuid;
        public IntPtr DevInst;
        public IntPtr Reserved;
    }
}