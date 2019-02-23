using SVN.Usb.TransferObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SVN.Usb
{
    public static class Usb
    {
        public static IEnumerable<UsbDevice> GetDrives()
        {
            foreach (var drive in DriveInfo.GetDrives().Select(x => UsbLogic.FindDriveLetter(x.Name)).Where(x => x != null))
            {
                yield return new UsbDevice
                {
                    Address = drive.Address,
                    DriveName = drive.DriveName,
                    PortNumber = drive.PortNumber,
                    DriverKey = drive.DriverKey,
                    HubDevicePath = drive.HubDevicePath,
                    InstanceID = drive.InstanceID,
                    Name = drive.Name,
                    Manufacturer = drive.Manufacturer,
                    Product = drive.Product,
                    SerialNumber = drive.SerialNumber,
                };
            }
        }

        public static IEnumerable<UsbTreeItem> GetTreeView()
        {
            foreach (var item in UsbTreeView.TextHost())
            {
                yield return item;
            }
        }
    }
}