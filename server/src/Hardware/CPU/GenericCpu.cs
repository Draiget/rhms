using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Drivers.Presentation;

namespace server.Hardware.CPU
{
    public class GenericCpu
    {
        private readonly Cpuid _cpuid;

        public CpuVendor Vendor => _cpuid.Vendor;

        public GenericCpu(Cpuid cpuInfo){
            _cpuid = cpuInfo;
        }
    }
}
