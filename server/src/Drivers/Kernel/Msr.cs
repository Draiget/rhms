using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.Kernel
{
    /// <summary>
    /// Model-specific register functions.
    /// <see cref="http://download.intel.com/products/processor/manual/325384.pdf"/>
    /// </summary>
    internal static class Msr
    {
        /// <summary>
        /// Read MSR data shared function
        /// </summary>
        /// <param name="index">IO buffer index</param>
        /// <param name="eax">EAX out register (bit 0-31)</param>
        /// <param name="edx">EDX out register (bit 32-63)</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadMsr", CallingConvention = CallingConvention.Winapi)]
        internal static extern bool Read(uint index, ref uint eax, ref uint edx);

        /// <summary>
        /// Read MSR data (thread context)
        /// </summary>
        /// <param name="index">IO buffer index</param>
        /// <param name="eax">EAX out register (bit 0-31)</param>
        /// <param name="edx">EDX out register (bit 32-63)</param>
        /// <param name="threadAffinityMask">Thread affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadMsrTx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadTx(uint index, ref uint eax, ref uint edx, UIntPtr threadAffinityMask);

        /// <summary>
        /// Read MSR data (process context)
        /// </summary>
        /// <param name="index">IO buffer index</param>
        /// <param name="eax">EAX out register (bit 0-31)</param>
        /// <param name="edx">EDX out register (bit 32-63)</param>
        /// <param name="processAffinityMask">Process affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_ReadMsrPx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool ReadPx(uint index, ref uint eax, ref uint edx, UIntPtr processAffinityMask);

        /// <summary>
        /// Write MSR data shared function
        /// </summary>
        /// <param name="index">IO buffer index</param>
        /// <param name="eax">EAX register (bit 0-31)</param>
        /// <param name="edx">EDX register (bit 32-63)</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WriteMsr", CallingConvention = CallingConvention.Winapi)]
        public static extern bool Write(uint index, uint eax, uint edx);

        /// <summary>
        /// Write MSR data (thread context)
        /// </summary>
        /// <param name="index">IO buffer index</param>
        /// <param name="eax">EAX register (bit 0-31)</param>
        /// <param name="edx">EDX register (bit 32-63)</param>
        /// <param name="threadAffinityMask">Thread affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WriteMsrTx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool WriteTx(uint index, uint eax, uint edx, UIntPtr threadAffinityMask);

        /// <summary>
        /// Write MSR data (process context)
        /// </summary>
        /// <param name="index">IO buffer index</param>
        /// <param name="eax">EAX register (bit 0-31)</param>
        /// <param name="edx">EDX register (bit 32-63)</param>
        /// <param name="processAffinityMask">Process affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_WriteMsrPx", CallingConvention = CallingConvention.Winapi)]
        public static extern bool WritePx(uint index, uint eax, uint edx, UIntPtr processAffinityMask);
    }
}
