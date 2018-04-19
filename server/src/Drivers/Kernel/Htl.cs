using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.Kernel
{
    /// <summary>
    /// HLT (Halt)
    /// <see cref="https://en.wikipedia.org/wiki/HLT_(x86_instruction)"/>
    /// </summary>
    internal static class Htl
    {
        /// <summary>
        /// Halt to the next instruction
        /// </summary>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_Hlt", CallingConvention = CallingConvention.Winapi)]
        internal static extern bool Shared();

        /// <summary>
        /// Halt to the next instruction (thread context)
        /// </summary>
        /// <param name="threadAffinityMask">Thread affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_HltTx", CallingConvention = CallingConvention.Winapi)]
        internal static extern bool Tx(UIntPtr threadAffinityMask);

        /// <summary>
        /// Halt to the next instruction (process context)
        /// </summary>
        /// <param name="processAffinityMask">Process affinity mask</param>
        /// <returns>Success of failure</returns>
        [DllImport(KernelDriverBridge.DriverFullPath, EntryPoint = "RHMS_HltPx", CallingConvention = CallingConvention.Winapi)]
        internal static extern bool Px(UIntPtr processAffinityMask);
    }
}
