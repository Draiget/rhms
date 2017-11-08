using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.NT
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SSystemProcessorPerformanceInformation
    {
        public long IdleTime;
        public long KernelTime;
        public long UserTime;
        public long Reserved0;
        public long Reserved1;
        public ulong Reserved2;
    }
}
