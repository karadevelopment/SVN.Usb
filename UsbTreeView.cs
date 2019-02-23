using SVN.Usb.Objects;
using SVN.Usb.TransferObjects;
using System.Collections.Generic;

namespace SVN.Usb
{
    internal static class UsbTreeView
    {
        public static IEnumerable<UsbTreeItem> TextHost()
        {
            var depth = 0;

            foreach (var controller in UsbHost.GetControllers())
            {
                yield return new UsbTreeItem { Depth = depth, Value = $"C-{controller?.Name} ({controller?.Address})" };

                foreach (var item in UsbTreeView.TextController(depth + 1, controller))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<UsbTreeItem> TextController(int depth, UsbController controller)
        {
            var hub = controller.GetRootHub();
            yield return new UsbTreeItem { Depth = depth, Value = $"H-{hub?.Name} ({hub?.Address})" };

            foreach (var item in UsbTreeView.TextHub(depth + 1, hub))
            {
                yield return item;
            }
        }

        public static IEnumerable<UsbTreeItem> TextHub(int depth, UsbHub hub)
        {
            foreach (var port in hub.GetPorts())
            {
                yield return new UsbTreeItem { Depth = depth, Value = $"P-{port?.PortNumber} ({port?.Address})" };

                if (port.IsHub)
                {
                    foreach (var item in UsbTreeView.TextHub(depth + 1, port.GetHub(null)))
                    {
                        yield return item;
                    }
                }
                else
                {
                    foreach (var item in UsbTreeView.TextPort(depth + 1, port))
                    {
                        yield return item;
                    }
                }
            }
        }

        public static IEnumerable<UsbTreeItem> TextPort(int depth, UsbPort port)
        {
            var device = port.GetDevice(null);
            yield return new UsbTreeItem { Depth = depth, Value = $"D-{device?.Manufacturer} ({device?.Address})" };
        }
    }
}