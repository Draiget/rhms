using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.NT
{
    public static class NativeNt
    {
        [DllImport("ntdll.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern int NtQuerySystemInformation(
            SystemInformationClass informationClass,
            [Out] SSystemProcessorPerformanceInformation[] informations,
            int structSize, 
            out IntPtr returnLength);
    }
}
