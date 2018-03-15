using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Hardware
{
    public abstract class HardwareIdentifer
    {
        protected IHardware HardwareRef;

        protected HardwareIdentifer(IHardware hardware){
            HardwareRef = hardware;
        }

        public abstract HardwareType GetHardwareType();

        public abstract string GetVendor();

        public abstract string GetModel();

        public abstract string GetFullSystemName();
    }
}
