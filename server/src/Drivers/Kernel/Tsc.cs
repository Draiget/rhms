using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.Kernel
{
    /// <summary>
    /// TSC (Time Stamp Counter) registers
    /// <see cref="https://ru.wikipedia.org/wiki/Rdtsc"/>
    /// </summary>
    public static class Tsc
    {
        /// <summary>
        /// Read TSC data shared
        /// </summary>
        /// <param name="eax">EAX register (bit 0-31)</param>
        /// <param name="edx">EDX register (bit 32-63)</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadTsc", CallingConvention = CallingConvention.Winapi)]
        internal static extern bool Read(ref uint eax, ref uint edx);

        /// <summary>
        /// Read TSC data (thread context)
        /// </summary>
        /// <param name="eax">EAX register (bit 0-31)</param>
        /// <param name="edx">EDX register (bit 32-63)</param>
        /// <param name="threadAffinityMask">Thread affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadTscTx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadTx(ref uint eax, ref uint edx, UIntPtr threadAffinityMask);

        /// <summary>
        /// Read TSC data (process context)
        /// </summary>
        /// <param name="eax">EAX register (bit 0-31)</param>
        /// <param name="edx">EDX register (bit 32-63)</param>
        /// <param name="processAffinityMask">Process affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadTscPx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadPx(ref uint eax, ref uint edx, UIntPtr processAffinityMask);
    }
}
