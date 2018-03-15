using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Drivers.Presentation;

namespace server.Hardware.CPU
{
    public abstract class GenericCpu : IHardware
    {
        private readonly Cpuid _cpuid;

        public CpuVendor Vendor => _cpuid.Vendor;

        protected GenericCpu(Cpuid cpuInfo){
            _cpuid = cpuInfo;
        }

        public abstract HardwareIdentifer Identify();

        public ISensor[] GetSensors() {
            throw new NotImplementedException();
        }

        public void TickUpdate() {
            throw new NotImplementedException();
        }
    }
}
