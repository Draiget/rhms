using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.Kernel
{
    /// <summary>
    /// PMC functions (Performance Monitor Count Registers)
    /// <see cref="http://wiki.dreamrunner.org/public_html/Embedded-System/Cortex-A8/PerformanceMonitorControlRegister.html"/>
    /// </summary>
    public static class Pmc
    {
        /// <summary>
        /// Read PMC data shared function
        /// </summary>
        /// <param name="index">IO buffer index</param>
        /// <param name="eax">EAX out register (bit 0-31)</param>
        /// <param name="edx">EDX out register (bit 32-63)</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadPmc", CallingConvention = CallingConvention.Winapi)]
        internal static extern bool Read(uint index, ref uint eax, ref uint edx);

        /// <summary>
        /// Read PMC data function (thread context)
        /// </summary>
        /// <param name="index">IO buffer index</param>
        /// <param name="eax">EAX out register (bit 0-31)</param>
        /// <param name="edx">EDX out register (bit 32-63)</param>
        /// <param name="threadAffinityMask">Thread affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadPmcTx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadTx(uint index, ref uint eax, ref uint edx, UIntPtr threadAffinityMask);

        /// <summary>
        /// Read PMC data function (process context)
        /// </summary>
        /// <param name="index">IO buffer index</param>
        /// <param name="eax">EAX out register (bit 0-31)</param>
        /// <param name="edx">EDX out register (bit 32-63)</param>
        /// <param name="processAffinityMask">Process affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadPmcTx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadPx(uint index, ref uint eax, ref uint edx, UIntPtr processAffinityMask);
    }
}
