using SVN.Usb.Constants;
using SVN.Usb.Enums;
using SVN.Usb.Extern;
using SVN.Usb.Structures;
using System;
using System.Runtime.InteropServices;

namespace SVN.Usb.Objects
{
    internal class UsbController : UsbBase
    {
        public int ControllerIndex { get; set; }
        public string ControllerDriverKeyName { get; set; } = string.Empty;
        public string ControllerDevicePath { get; set; } = string.Empty;
        public string ControllerDeviceDesc { get; set; } = string.Empty;

        public UsbController(UsbBase usbBase) : base(usbBase)
        {
        }

        protected override string Identification
        {
            get => this.ControllerIndex.ToString();
        }

        public int Index
        {
            get => this.ControllerIndex;
        }

        public string DriverKeyName
        {
            get => this.ControllerDriverKeyName;
        }

        public string DevicePath
        {
            get => this.ControllerDevicePath;
        }

        public string Name
        {
            get => this.ControllerDeviceDesc;
        }

        public UsbHub GetRootHub()
        {
            IntPtr h, h2;
            var root = new UsbHub(this)
            {
                HubIsRootHub = true,
                HubDeviceDesc = "Root Hub",
            };

            h = Kernel32.CreateFile(this.ControllerDevicePath, UsbConstants.GENERIC_WRITE, UsbConstants.FILE_SHARE_WRITE, IntPtr.Zero, UsbConstants.OPEN_EXISTING, default(int), IntPtr.Zero);
            if (h.ToInt32() != UsbConstants.INVALID_HANDLE_VALUE)
            {
                var hubName = new UsbRootHubName();
                var nBytes = Marshal.SizeOf(hubName);
                var ptrHubName = Marshal.AllocHGlobal(nBytes);

                if (Kernel32.DeviceIoControl(h, UsbConstants.IOCTL_USB_GET_ROOT_HUB_NAME, ptrHubName, nBytes, ptrHubName, nBytes, out int nBytesReturned, IntPtr.Zero))
                {
                    hubName = (UsbRootHubName)Marshal.PtrToStructure(ptrHubName, typeof(UsbRootHubName));
                    root.HubDevicePath = $@"\\.\{hubName.RootHubName}";
                }

                h2 = Kernel32.CreateFile(root.HubDevicePath, UsbConstants.GENERIC_WRITE, UsbConstants.FILE_SHARE_WRITE, IntPtr.Zero, UsbConstants.OPEN_EXISTING, default(int), IntPtr.Zero);
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
                        root.HubIsBusPowered = Convert.ToBoolean(nodeInfo.HubInformation.HubIsBusPowered);
                        root.HubPortCount = nodeInfo.HubInformation.HubDescriptor.bNumberOfPorts;
                    }
                    Marshal.FreeHGlobal(ptrNodeInfo);
                    Kernel32.CloseHandle(h2);
                }

                Marshal.FreeHGlobal(ptrHubName);
                Kernel32.CloseHandle(h);
            }

            return root;
        }
    }
}