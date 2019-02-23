using SVN.Usb.Constants;
using SVN.Usb.Extern;
using SVN.Usb.Objects;
using SVN.Usb.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SVN.Usb
{
    internal static class UsbLogic
    {
        public static string GetDescriptionByKeyName(string driverKeyName)
        {
            var result = string.Empty;

            var h = SetupApi.SetupDiGetClassDevs(default(int), UsbConstants.REGSTR_KEY_USB, IntPtr.Zero, UsbConstants.DIGCF_PRESENT | UsbConstants.DIGCF_ALLCLASSES);
            if (h.ToInt32() != UsbConstants.INVALID_HANDLE_VALUE)
            {
                var ptrBuf = Marshal.AllocHGlobal(UsbConstants.BUFFER_SIZE);
                var keyName = string.Empty;

                var i = default(int);
                var success = default(bool);
                do
                {
                    var da = new SpDevinfoData();
                    da.cbSize = Marshal.SizeOf(da);

                    success = SetupApi.SetupDiEnumDeviceInfo(h, i, ref da);
                    if (success)
                    {
                        var regType = UsbConstants.REG_SZ;
                        var requiredSize = default(int);
                        keyName = string.Empty;

                        if (SetupApi.SetupDiGetDeviceRegistryProperty(h, ref da, UsbConstants.SPDRP_DRIVER, ref regType, ptrBuf, UsbConstants.BUFFER_SIZE, ref requiredSize))
                        {
                            keyName = Marshal.PtrToStringAuto(ptrBuf);
                        }

                        if (keyName == driverKeyName)
                        {
                            if (SetupApi.SetupDiGetDeviceRegistryProperty(h, ref da, UsbConstants.SPDRP_DEVICEDESC, ref regType, ptrBuf, UsbConstants.BUFFER_SIZE, ref requiredSize))
                            {
                                result = Marshal.PtrToStringAuto(ptrBuf);
                            }
                            break;
                        }
                    }
                    i++;
                } while (success);

                Marshal.FreeHGlobal(ptrBuf);
                SetupApi.SetupDiDestroyDeviceInfoList(h);
            }

            return result;
        }

        public static string GetInstanceIDByKeyName(string driverKeyName)
        {
            var result = string.Empty;

            var h = SetupApi.SetupDiGetClassDevs(default(int), UsbConstants.REGSTR_KEY_USB, IntPtr.Zero, UsbConstants.DIGCF_PRESENT | UsbConstants.DIGCF_ALLCLASSES);
            if (h.ToInt32() != UsbConstants.INVALID_HANDLE_VALUE)
            {
                var ptrBuf = Marshal.AllocHGlobal(UsbConstants.BUFFER_SIZE);
                var keyName = string.Empty;

                var i = default(int);
                var success = default(bool);
                do
                {
                    var da = new SpDevinfoData();
                    da.cbSize = Marshal.SizeOf(da);

                    success = SetupApi.SetupDiEnumDeviceInfo(h, i, ref da);
                    if (success)
                    {
                        var regType = UsbConstants.REG_SZ;
                        var requiredSize = default(int);

                        keyName = string.Empty;
                        if (SetupApi.SetupDiGetDeviceRegistryProperty(h, ref da, UsbConstants.SPDRP_DRIVER, ref regType, ptrBuf, UsbConstants.BUFFER_SIZE, ref requiredSize))
                        {
                            keyName = Marshal.PtrToStringAuto(ptrBuf);
                        }

                        if (keyName == driverKeyName)
                        {
                            var nBytes = UsbConstants.BUFFER_SIZE;
                            var sb = new StringBuilder(nBytes);
                            SetupApi.SetupDiGetDeviceInstanceId(h, ref da, sb, nBytes, out requiredSize);
                            result = sb.ToString();
                            break;
                        }
                    }
                    i++;
                } while (success);

                Marshal.FreeHGlobal(ptrBuf);
                SetupApi.SetupDiDestroyDeviceInfoList(h);
            }

            return result;
        }

        private static void SearchHubInstanceID(UsbHub hub, ref UsbDevice foundDevice, string instanceID, string driveName)
        {
            foreach (var port in hub.GetPorts())
            {
                if (port.IsHub)
                {
                    UsbLogic.SearchHubInstanceID(port.GetHub(driveName), ref foundDevice, instanceID, driveName);
                }
                else
                {
                    if (port.IsDeviceConnected)
                    {
                        var device = port.GetDevice(driveName);

                        if (device.InstanceID == instanceID)
                        {
                            foundDevice = device;
                            break;
                        }
                    }
                }
            }
        }

        private static int GetDeviceNumber(string devicePath)
        {
            var result = -1;

            var h = Kernel32.CreateFile(devicePath.TrimEnd('\\'), default(int), default(int), IntPtr.Zero, UsbConstants.OPEN_EXISTING, default(int), IntPtr.Zero);
            if (h.ToInt32() != UsbConstants.INVALID_HANDLE_VALUE)
            {
                var sdn = new StorageDeviceNumber();
                var nBytes = Marshal.SizeOf(sdn);
                var ptrSdn = Marshal.AllocHGlobal(nBytes);

                if (Kernel32.DeviceIoControl(h, UsbConstants.IOCTL_STORAGE_GET_DEVICE_NUMBER, IntPtr.Zero, default(int), ptrSdn, nBytes, out int requiredSize, IntPtr.Zero))
                {
                    sdn = (StorageDeviceNumber)Marshal.PtrToStructure(ptrSdn, typeof(StorageDeviceNumber));
                    result = (sdn.DeviceType << 8) + sdn.DeviceNumber;
                }
                Marshal.FreeHGlobal(ptrSdn);
                Kernel32.CloseHandle(h);
            }

            return result;
        }

        private static UsbDevice FindDeviceByInstanceID(string instanceID, string driveName)
        {
            UsbDevice foundDevice = null;

            foreach (var controller in UsbHost.GetControllers())
            {
                UsbLogic.SearchHubInstanceID(controller.GetRootHub(), ref foundDevice, instanceID, driveName);

                if (foundDevice != null)
                {
                    break;
                }
            }

            return foundDevice;
        }

        public static UsbDevice FindDriveLetter(string driveName)
        {
            UsbDevice foundDevice = null;
            var instanceID = string.Empty;

            var devNum = UsbLogic.GetDeviceNumber($@"\\.\{driveName.TrimEnd('\\')}");

            if (devNum < default(int))
            {
                return foundDevice;
            }

            var diskGUID = new Guid(UsbConstants.GUID_DEVINTERFACE_DISK);

            var h = SetupApi.SetupDiGetClassDevs(ref diskGUID, default(int), IntPtr.Zero, UsbConstants.DIGCF_PRESENT | UsbConstants.DIGCF_DEVICEINTERFACE);
            if (h.ToInt32() != UsbConstants.INVALID_HANDLE_VALUE)
            {
                var i = default(int);
                var success = true;
                do
                {
                    var dia = new SpDeviceInterfaceData();
                    dia.cbSize = Marshal.SizeOf(dia);

                    success = SetupApi.SetupDiEnumDeviceInterfaces(h, IntPtr.Zero, ref diskGUID, i, ref dia);
                    if (success)
                    {
                        var da = new SpDevinfoData();
                        da.cbSize = Marshal.SizeOf(da);

                        var didd = new SpDeviceInterfaceDetailData
                        {
                            cbSize = 4 + Marshal.SystemDefaultCharSize,
                        };

                        var nRequiredSize = default(int);
                        int nBytes = UsbConstants.BUFFER_SIZE;
                        if (SetupApi.SetupDiGetDeviceInterfaceDetail(h, ref dia, ref didd, nBytes, ref nRequiredSize, ref da))
                        {
                            if (UsbLogic.GetDeviceNumber(didd.DevicePath) == devNum)
                            {
                                SetupApi.CM_Get_Parent(out IntPtr ptrPrevious, da.DevInst, default(int));

                                var ptrInstanceBuf = Marshal.AllocHGlobal(nBytes);
                                SetupApi.CM_Get_Device_ID(ptrPrevious, ptrInstanceBuf, nBytes, default(int));
                                instanceID = Marshal.PtrToStringAuto(ptrInstanceBuf);

                                Marshal.FreeHGlobal(ptrInstanceBuf);
                                break;
                            }
                        }
                    }
                    i++;
                } while (success);
                SetupApi.SetupDiDestroyDeviceInfoList(h);
            }

            if (instanceID.StartsWith("USB\\"))
            {
                foundDevice = UsbLogic.FindDeviceByInstanceID(instanceID, driveName);
            }

            return foundDevice;
        }

        private static void SearchHubDriverKeyName(UsbHub hub, ref UsbDevice foundDevice, string driverKeyName, string driveName)
        {
            foreach (var port in hub.GetPorts())
            {
                if (port.IsHub)
                {
                    UsbLogic.SearchHubDriverKeyName(port.GetHub(driveName), ref foundDevice, driverKeyName, driveName);
                }
                else
                {
                    if (port.IsDeviceConnected)
                    {
                        var device = port.GetDevice(driveName);

                        if (device.DriverKey == driverKeyName)
                        {
                            foundDevice = device;
                            break;
                        }
                    }
                }
            }
        }

        public static UsbDevice FindDeviceByDriverKeyName(string driverKeyName, string driveName)
        {
            UsbDevice foundDevice = null;

            foreach (var controller in UsbHost.GetControllers())
            {
                UsbLogic.SearchHubDriverKeyName(controller.GetRootHub(), ref foundDevice, driverKeyName, driveName);

                if (foundDevice != null)
                {
                    break;
                }
            }

            return foundDevice;
        }

        private static IEnumerable<UsbDevice> ListHub(UsbHub hub, string driveName)
        {
            foreach (var port in hub.GetPorts())
            {
                if (port.IsHub)
                {
                    foreach (var device in UsbLogic.ListHub(port.GetHub(driveName), driveName))
                    {
                        yield return device;
                    }
                }
                else
                {
                    if (port.IsDeviceConnected)
                    {
                        yield return port.GetDevice(driveName);
                    }
                }
            }
        }

        public static IEnumerable<UsbDevice> GetConnectedDevices(string driveName)
        {
            foreach (var device in UsbHost.GetControllers().Select(x => x.GetRootHub()).SelectMany(x => UsbLogic.ListHub(x, driveName)))
            {
                yield return device;
            }
        }
    }
}