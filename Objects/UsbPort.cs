using SVN.Usb.Constants;
using SVN.Usb.Enums;
using SVN.Usb.Extern;
using SVN.Usb.Structures;
using System;
using System.Runtime.InteropServices;

namespace SVN.Usb.Objects
{
    internal class UsbPort : UsbBase
    {
        public int PortNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public string HubDevicePath { get; set; } = string.Empty;
        public string Speed { get; set; } = string.Empty;
        public bool IsHub { get; set; }
        public bool IsDeviceConnected { get; set; }
        public UsbDeviceDescriptor DeviceDescriptor { get; set; }

        public UsbPort(UsbBase usbBase) : base(usbBase)
        {
        }

        protected override string Identification
        {
            get => this.PortNumber.ToString();
        }

        public UsbHub GetHub(string driveName)
        {
            if (!this.IsHub)
            {
                return null;
            }

            IntPtr h, h2;
            var hub = new UsbHub(this)
            {
                HubIsRootHub = false,
                HubDeviceDesc = "External Hub",
            };

            h = Kernel32.CreateFile(this.HubDevicePath, UsbConstants.GENERIC_WRITE, UsbConstants.FILE_SHARE_WRITE, IntPtr.Zero, UsbConstants.OPEN_EXISTING, default(int), IntPtr.Zero);
            if (h.ToInt32() != UsbConstants.INVALID_HANDLE_VALUE)
            {
                var nodeName = new UsbNodeConnectionName
                {
                    ConnectionIndex = this.PortNumber,
                };
                var nBytes = Marshal.SizeOf(nodeName);
                var ptrNodeName = Marshal.AllocHGlobal(nBytes);
                Marshal.StructureToPtr(nodeName, ptrNodeName, true);

                if (Kernel32.DeviceIoControl(h, UsbConstants.IOCTL_USB_GET_NODE_CONNECTION_NAME, ptrNodeName, nBytes, ptrNodeName, nBytes, out int nBytesReturned, IntPtr.Zero))
                {
                    nodeName = (UsbNodeConnectionName)Marshal.PtrToStructure(ptrNodeName, typeof(UsbNodeConnectionName));
                    hub.HubDevicePath = $@"\\.\{nodeName.NodeName}";
                }

                h2 = Kernel32.CreateFile(hub.HubDevicePath, UsbConstants.GENERIC_WRITE, UsbConstants.FILE_SHARE_WRITE, IntPtr.Zero, UsbConstants.OPEN_EXISTING, default(int), IntPtr.Zero);
                if (h2.ToInt32() != UsbConstants.INVALID_HANDLE_VALUE)
                {
                    var nodeInfo = new UsbNodeInformation
                    {
                        NodeType = (int)UsbHubNode.UsbHub,
                    };
                    nBytes = Marshal.SizeOf(nodeInfo);
                    var ptrNodeInfo = Marshal.AllocHGlobal(nBytes);
                    Marshal.StructureToPtr(nodeInfo, ptrNodeInfo, true);

                    if (Kernel32.DeviceIoControl(h2, UsbConstants.IOCTL_USB_GET_NODE_INFORMATION, ptrNodeInfo, nBytes, ptrNodeInfo, nBytes, out nBytesReturned, IntPtr.Zero))
                    {
                        nodeInfo = (UsbNodeInformation)Marshal.PtrToStructure(ptrNodeInfo, typeof(UsbNodeInformation));
                        hub.HubIsBusPowered = Convert.ToBoolean(nodeInfo.HubInformation.HubIsBusPowered);
                        hub.HubPortCount = nodeInfo.HubInformation.HubDescriptor.bNumberOfPorts;
                    }
                    Marshal.FreeHGlobal(ptrNodeInfo);
                    Kernel32.CloseHandle(h2);
                }

                var device = this.GetDevice(driveName);
                hub.HubInstanceID = device.InstanceID;
                hub.HubManufacturer = device.Manufacturer;
                hub.HubProduct = device.Product;
                hub.HubSerialNumber = device.SerialNumber;
                hub.HubDriverKey = device.DriverKey;

                Marshal.FreeHGlobal(ptrNodeName);
                Kernel32.CloseHandle(h);
            }

            return hub;
        }

        public UsbDevice GetDevice(string driveName)
        {
            if (!this.IsDeviceConnected)
            {
                return null;
            }

            var device = new UsbDevice(this)
            {
                DriveName = driveName, // TODO
                PortNumber = this.PortNumber,
                HubDevicePath = this.HubDevicePath,
                Descriptor = this.DeviceDescriptor,
            };

            var h = Kernel32.CreateFile(this.HubDevicePath, UsbConstants.GENERIC_WRITE, UsbConstants.FILE_SHARE_WRITE, IntPtr.Zero, UsbConstants.OPEN_EXISTING, default(int), IntPtr.Zero);
            if (h.ToInt32() != UsbConstants.INVALID_HANDLE_VALUE)
            {
                var nBytesReturned = default(int);
                var nBytes = UsbConstants.BUFFER_SIZE;
                var nullString = new string((char)default(int), UsbConstants.BUFFER_SIZE / Marshal.SystemDefaultCharSize);

                if (default(int) < this.DeviceDescriptor.iManufacturer)
                {
                    var request = new UsbDescriptorRequest
                    {
                        ConnectionIndex = this.PortNumber,
                    };
                    request.SetupPacket.wValue = (short)((UsbConstants.USB_STRING_DESCRIPTOR_TYPE << 8) + this.DeviceDescriptor.iManufacturer);
                    request.SetupPacket.wLength = (short)(nBytes - Marshal.SizeOf(request));
                    request.SetupPacket.wIndex = 0x409;

                    var ptrRequest = Marshal.StringToHGlobalAuto(nullString);
                    Marshal.StructureToPtr(request, ptrRequest, true);

                    if (Kernel32.DeviceIoControl(h, UsbConstants.IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION, ptrRequest, nBytes, ptrRequest, nBytes, out nBytesReturned, IntPtr.Zero))
                    {
                        var ptrStringDesc = new IntPtr(ptrRequest.ToInt32() + Marshal.SizeOf(request));
                        var stringDesc = (UsbStringDescriptor)Marshal.PtrToStructure(ptrStringDesc, typeof(UsbStringDescriptor));
                        device.Manufacturer = stringDesc.bString;
                    }
                    Marshal.FreeHGlobal(ptrRequest);
                }
                if (default(int) < this.DeviceDescriptor.iProduct)
                {
                    var request = new UsbDescriptorRequest
                    {
                        ConnectionIndex = this.PortNumber,
                    };
                    request.SetupPacket.wValue = (short)((UsbConstants.USB_STRING_DESCRIPTOR_TYPE << 8) + this.DeviceDescriptor.iProduct);
                    request.SetupPacket.wLength = (short)(nBytes - Marshal.SizeOf(request));
                    request.SetupPacket.wIndex = 0x409;

                    var ptrRequest = Marshal.StringToHGlobalAuto(nullString);
                    Marshal.StructureToPtr(request, ptrRequest, true);

                    if (Kernel32.DeviceIoControl(h, UsbConstants.IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION, ptrRequest, nBytes, ptrRequest, nBytes, out nBytesReturned, IntPtr.Zero))
                    {
                        var ptrStringDesc = new IntPtr(ptrRequest.ToInt32() + Marshal.SizeOf(request));
                        var stringDesc = (UsbStringDescriptor)Marshal.PtrToStructure(ptrStringDesc, typeof(UsbStringDescriptor));
                        device.Product = stringDesc.bString;
                    }
                    Marshal.FreeHGlobal(ptrRequest);
                }
                if (default(int) < this.DeviceDescriptor.iSerialNumber)
                {
                    var request = new UsbDescriptorRequest
                    {
                        ConnectionIndex = this.PortNumber,
                    };
                    request.SetupPacket.wValue = (short)((UsbConstants.USB_STRING_DESCRIPTOR_TYPE << 8) + this.DeviceDescriptor.iSerialNumber);
                    request.SetupPacket.wLength = (short)(nBytes - Marshal.SizeOf(request));
                    request.SetupPacket.wIndex = 0x409;

                    var ptrRequest = Marshal.StringToHGlobalAuto(nullString);
                    Marshal.StructureToPtr(request, ptrRequest, true);

                    if (Kernel32.DeviceIoControl(h, UsbConstants.IOCTL_USB_GET_DESCRIPTOR_FROM_NODE_CONNECTION, ptrRequest, nBytes, ptrRequest, nBytes, out nBytesReturned, IntPtr.Zero))
                    {
                        var ptrStringDesc = new IntPtr(ptrRequest.ToInt32() + Marshal.SizeOf(request));
                        var stringDesc = (UsbStringDescriptor)Marshal.PtrToStructure(ptrStringDesc, typeof(UsbStringDescriptor));
                        device.SerialNumber = stringDesc.bString;
                    }
                    Marshal.FreeHGlobal(ptrRequest);
                }

                var driverKey = new UsbNodeConnectionDriverkeyName
                {
                    ConnectionIndex = this.PortNumber,
                };
                nBytes = Marshal.SizeOf(driverKey);
                var ptrDriverKey = Marshal.AllocHGlobal(nBytes);
                Marshal.StructureToPtr(driverKey, ptrDriverKey, true);

                if (Kernel32.DeviceIoControl(h, UsbConstants.IOCTL_USB_GET_NODE_CONNECTION_DRIVERKEY_NAME, ptrDriverKey, nBytes, ptrDriverKey, nBytes, out nBytesReturned, IntPtr.Zero))
                {
                    driverKey = (UsbNodeConnectionDriverkeyName)Marshal.PtrToStructure(ptrDriverKey, typeof(UsbNodeConnectionDriverkeyName));
                    device.DriverKey = driverKey.DriverKeyName;

                    device.Name = UsbLogic.GetDescriptionByKeyName(device.DriverKey);
                    device.InstanceID = UsbLogic.GetInstanceIDByKeyName(device.DriverKey);
                }
                Marshal.FreeHGlobal(ptrDriverKey);
                Kernel32.CloseHandle(h);
            }

            return device;
        }
    }
}