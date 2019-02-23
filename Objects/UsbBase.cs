using System.Collections.Generic;

namespace SVN.Usb.Objects
{
    internal class UsbBase
    {
        protected UsbBase Parent { get; }

        protected UsbBase(UsbBase parent)
        {
            this.Parent = parent;
        }

        protected virtual string Identification
        {
            get
            {
                return default(string);
            }
        }

        public string Address
        {
            get
            {
                var result = new List<string>();

                if (this.Parent is null)
                {
                    result.Add(default(int).ToString());
                }
                else
                {
                    result.Add(this.Parent.Address);
                }
                if (this.Identification != null)
                {
                    result.Add(this.Identification);
                }

                return string.Join(".", result);
            }
        }
    }
}