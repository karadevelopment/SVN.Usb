using SVN.Usb.Constants;
using SVN.Usb.Extern;
using SVN.Usb.Structures;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SVN.Usb.Objects
{
    internal static class UsbHost
    {
        public static IEnumerable<UsbController> GetControllers()
        {
            var hostGUID = new Guid(UsbConstants.GUID_DEVINTERFACE_HUBCONTROLLER);

            var h = SetupApi.SetupDiGetClassDevs(ref hostGUID, default(int), IntPtr.Zero, UsbConstants.DIGCF_PRESENT | UsbConstants.DIGCF_DEVICEINTERFACE);
            if (h.ToInt32() != UsbConstants.INVALID_HANDLE_VALUE)
            {
                var ptrBuf = Marshal.AllocHGlobal(UsbConstants.BUFFER_SIZE);

                var i = default(int);
                var success = default(bool);
                do
                {
                    var host = new UsbController(null)
                    {
                        ControllerIndex = i,
                    };

                    var dia = new SpDeviceInterfaceData();
                    dia.cbSize = Marshal.SizeOf(dia);

                    success = SetupApi.SetupDiEnumDeviceInterfaces(h, IntPtr.Zero, ref hostGUID, i, ref dia);
                    if (success)
                    {
                        var da = new SpDevinfoData();
                        da.cbSize = Marshal.SizeOf(da);

                        var didd = new SpDeviceInterfaceDetailData
                        {
                            cbSize = 4 + Marshal.SystemDefaultCharSize,
                        };

                        var nRequiredSize = default(int);
                        var nBytes = UsbConstants.BUFFER_SIZE;

                        if (SetupApi.SetupDiGetDeviceInterfaceDetail(h, ref dia, ref didd, nBytes, ref nRequiredSize, ref da))
                        {
                            host.ControllerDevicePath = didd.DevicePath;

                            var requiredSize = default(int);
                            var regType = UsbConstants.REG_SZ;

                            if (SetupApi.SetupDiGetDeviceRegistryProperty(h, ref da, UsbConstants.SPDRP_DEVICEDESC, ref regType, ptrBuf, UsbConstants.BUFFER_SIZE, ref requiredSize))
                            {
                                host.ControllerDeviceDesc = Marshal.PtrToStringAuto(ptrBuf);
                            }
                            if (SetupApi.SetupDiGetDeviceRegistryProperty(h, ref da, UsbConstants.SPDRP_DRIVER, ref regType, ptrBuf, UsbConstants.BUFFER_SIZE, ref requiredSize))
                            {
                                host.ControllerDriverKeyName = Marshal.PtrToStringAuto(ptrBuf);
                            }
                        }

                        yield return host;
                    }
                    i++;
                }
                while (success);

                Marshal.FreeHGlobal(ptrBuf);
                SetupApi.SetupDiDestroyDeviceInfoList(h);
            }
        }
    }
}