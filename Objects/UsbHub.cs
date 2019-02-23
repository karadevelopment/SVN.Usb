using SVN.Usb.Constants;
using SVN.Usb.Enums;
using SVN.Usb.Extern;
using SVN.Usb.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SVN.Usb.Objects
{
    internal class UsbHub : UsbBase
    {
        public int HubPortCount { get; set; }
        public string HubDriverKey { get; set; } = string.Empty;
        public string HubDevicePath { get; set; } = string.Empty;
        public string HubDeviceDesc { get; set; } = string.Empty;
        public string HubManufacturer { get; set; } = string.Empty;
        public string HubProduct { get; set; } = string.Empty;
        public string HubSerialNumber { get; set; } = string.Empty;
        public string HubInstanceID { get; set; } = string.Empty;
        public bool HubIsBusPowered { get; set; }
        public bool HubIsRootHub { get; set; }

        public UsbHub(UsbBase usbBase) : base(usbBase)
        {
        }

        public int PortCount
        {
            get { return HubPortCount; }
        }

        public string DevicePath
        {
            get { return HubDevicePath; }
        }

        public string DriverKey
        {
            get { return HubDriverKey; }
        }

        public string Name
        {
            get { return HubDeviceDesc; }
        }

        public string InstanceID
        {
            get { return HubInstanceID; }
        }

        public bool IsBusPowered
        {
            get { return HubIsBusPowered; }
        }

        public bool IsRootHub
        {
            get { return HubIsRootHub; }
        }

        public string Manufacturer
        {
            get { return HubManufacturer; }
        }

        public string Product
        {
            get { return HubProduct; }
        }

        public string SerialNumber
        {
            get { return HubSerialNumber; }
        }

        public IEnumerable<UsbPort> GetPorts()
        {
            var h = Kernel32.CreateFile(this.HubDevicePath, UsbConstants.GENERIC_WRITE, UsbConstants.FILE_SHARE_WRITE, IntPtr.Zero, UsbConstants.OPEN_EXISTING, default(int), IntPtr.Zero);
            if (h.ToInt32() != UsbConstants.INVALID_HANDLE_VALUE)
            {
                var nBytes = Marshal.SizeOf(typeof(UsbNodeConnectionInformationEx));
                var ptrNodeConnection = Marshal.AllocHGlobal(nBytes);

                for (var i = 1; i <= this.HubPortCount; i++)
                {
                    var nodeConnection = new UsbNodeConnectionInformationEx
                    {
                        ConnectionIndex = i,
                    };
                    Marshal.StructureToPtr(nodeConnection, ptrNodeConnection, true);

                    if (Kernel32.DeviceIoControl(h, UsbConstants.IOCTL_USB_GET_NODE_CONNECTION_INFORMATION_EX, ptrNodeConnection, nBytes, ptrNodeConnection, nBytes, out int nBytesReturned, IntPtr.Zero))
                    {
                        nodeConnection = (UsbNodeConnectionInformationEx)Marshal.PtrToStructure(ptrNodeConnection, typeof(UsbNodeConnectionInformationEx));
                        var status = (UsbConnectionStatus)nodeConnection.ConnectionStatus;
                        var speed = (UsbDeviceSpeed)nodeConnection.Speed;

                        var port = new UsbPort(this)
                        {
                            PortNumber = i,
                            Status = status.ToString(),
                            HubDevicePath = this.HubDevicePath,
                            Speed = speed.ToString(),
                            IsHub = Convert.ToBoolean(nodeConnection.DeviceIsHub),
                            IsDeviceConnected = nodeConnection.ConnectionStatus == (int)UsbConnectionStatus.DeviceConnected,
                            DeviceDescriptor = nodeConnection.DeviceDescriptor,
                        };

                        yield return port;
                    }
                }
                Marshal.FreeHGlobal(ptrNodeConnection);
                Kernel32.CloseHandle(h);
            }
        }
    }
}