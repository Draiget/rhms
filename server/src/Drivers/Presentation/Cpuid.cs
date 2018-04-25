using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace server.Drivers.Presentation
{
    public static class Cpuid
    {
        public const uint MaxCores = 32;

        /// <summary>
        /// Returns base CPUID information.
        /// CPUID index constant. 
        /// </summary>
        public const uint Cpuid0 = 0;

        /// <summary>
        /// Returns extended CPUID information.
        /// CPUID index constant.
        /// </summary>
        public const uint CpuidExt = 0x80000000;
        
        /// <summary>
        /// Present all installed CPUs threads by CPU index
        /// </summary>
        private static Dictionary<uint, CpuidProcessorInfo> _cpusMap;

        /// <summary>
        /// Getting shared dictionary with all processing threads information
        /// </summary>
        /// <returns>Dicrionary of processor thread information where key is a processor index (from 0)</returns>
        public static Dictionary<uint, CpuidProcessorInfo> Get() {
            return _cpusMap ?? (_cpusMap = GetInstalledProcessorsInfos());
        }

        private static Dictionary<uint, CpuidProcessorInfo> GetInstalledProcessorsInfos() {
            var cpusMap = new Dictionary<uint, CpuidProcessorInfo>();
            var threads = new List<CpuidThreadInfo>();

            var cpus = 0u;

            for (uint i = 0; i <= (MaxCores * 2) * 2; i++) {
                if (!GetProcessorThreadInfo(i, out var threadInfo)) {
                    break;
                }

                cpus = Math.Max(threadInfo.ProcessorId, cpus);
                threads.Add(threadInfo);
            }

            for (uint cpuIndex = 0; cpuIndex < cpus + 1; cpuIndex++) {
                cpusMap.Add(cpuIndex, new CpuidProcessorInfo(
                    threads.Where(x => x.ProcessorId == cpuIndex).ToList()));
            }

            return cpusMap;
        }

        /// <summary>
        /// Getting single processor thread information
        /// </summary>
        /// <param name="threadIndex">Summary processor thread index (of all threads, not of specific processor)</param>
        /// <param name="info">Output processor thread CPUID info</param>
        /// <returns>If information is retrieved</returns>
        public static bool GetProcessorThreadInfo(uint threadIndex, out CpuidThreadInfo info) {
            try {
                info = new CpuidThreadInfo(threadIndex);
                return true;
            } catch {
                info = null;
                return false;
            }
        }

        public static string RemoveSpechialBrandSymbolsFromString(StringBuilder b) {
            b.Replace('\0', ' ');
            b.Replace("(R)", " ");
            b.Replace("(TM)", " ");
            b.Replace("(tm)", "");
            b.Replace("CPU", "");
            b.Replace("Quad-Core Processor", "");
            b.Replace("Six-Core Processor", "");
            b.Replace("Eight-Core Processor", "");
            var str = b.ToString();
            while (str.Contains("  ")) {
                str = str.Replace("  ", " ");
            }

            if (str.Contains("@")) {
                str = str.Remove(str.LastIndexOf('@'));
            }

            return str.Trim();
        }

        internal static bool ReadTxOut(uint index, uint subLeaf, ref CpuidRegisters registers, ulong threadAffinityMask){
            return Kernel.Cpuid.ReadTx(
                index, 
                subLeaf,
                ref registers.Eax,
                ref registers.Ebx,
                ref registers.Ecx,
                ref registers.Edx,
                (UIntPtr)threadAffinityMask);
        }

        public static CpuVendor VendorFromCpuidString(CpuidRegisters registers) {
            switch (registers.GetVendorString()) {
                case "GenuineIntel":
                    return CpuVendor.Intel;
                case "AuthenticAMD":
                    return CpuVendor.Amd;
                default:
                    return CpuVendor.Unknown;
            }
        }
    }
}
