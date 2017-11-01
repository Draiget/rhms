using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.Kernel
{
    /// <summary>
    /// CPUID functions
    /// <see cref="https://ru.wikipedia.org/wiki/CPUID"/>
    /// </summary>
    public static class Cpuid
    {
        /// <summary>
        /// Read PMC data shared function
        /// </summary>
        /// <param name="index">CPUID index</param>
        /// <param name="subLeafValue">ECX subleaf value (Bh leaf)</param>
        /// <param name="eax">EAX register</param>
        /// <param name="ebx">EBX register</param>
        /// <param name="ecx">ECX register</param>
        /// <param name="edx">EDX register</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_Cpuid", CallingConvention = CallingConvention.Winapi)]
        internal static extern bool Read(uint index, uint subLeafValue, ref uint eax, ref uint ebx, ref uint ecx, ref uint edx);

        /// <summary>
        /// Read PMC data function (thread context)
        /// </summary>
        /// <param name="index">CPUID index</param>
        /// <param name="subLeafValue">ECX subleaf value (Bh leaf)</param>
        /// <param name="eax">EAX register</param>
        /// <param name="ebx">EBX register</param>
        /// <param name="ecx">ECX register</param>
        /// <param name="edx">EDX register</param>
        /// <param name="threadAffinityMask">Thread affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_CpuidTx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadTx(uint index, uint subLeafValue, ref uint eax, ref uint ebx, ref uint ecx, ref uint edx, UIntPtr threadAffinityMask);

        /// <summary>
        /// Read PMC data function (process context)
        /// </summary>
        /// <param name="index">CPUID index</param>
        /// <param name="subLeafValue">ECX subleaf value (Bh leaf)</param>
        /// <param name="eax">EAX register</param>
        /// <param name="ebx">EBX register</param>
        /// <param name="ecx">ECX register</param>
        /// <param name="edx">EDX register</param>
        /// <param name="processAffinityMask">Process affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_CpuidPx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadPx(uint index, uint subLeafValue, ref uint eax, ref uint ebx, ref uint ecx, ref uint edx, UIntPtr processAffinityMask);
    }
}
