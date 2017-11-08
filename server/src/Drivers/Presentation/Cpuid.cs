using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using server.Drivers.NT;
using Logger = server.Utils.Logging.Logger;

namespace server.Drivers.Presentation
{
    public class Cpuid
    {
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

        public CpuVendor Vendor {
            get;
            private set;
        }

        public uint LogicalCores {
            get;
            private set;
        }

        public string BrandName {
            get;
            private set;
        }

        public string Name {
            get;
            private set;
        }

        public bool IsSupportMsr {
            get;
            private set;
        }

        public bool IsSupportTsc {
            get;
            private set;
        }

        public bool IsSupportInvariantTsc {
            get;
            private set;
        }

        public static Cpuid CollectAll(){
            var cpuInfo = new Cpuid();

            for (uint i = 0; i <= 128; i++) {
                if (FillCpuidFromThread(ref cpuInfo, i)) {
                    cpuInfo.LogicalCores++;
                } else {
                    break;
                }
            }

            return cpuInfo;
        }

        public static Cpuid CollectFromThread(uint threadId){
            var cpuInfo = new Cpuid();

            FillCpuidFromThread(ref cpuInfo, threadId, true);

            return cpuInfo;
        }

        private static bool FillCpuidFromThread(ref Cpuid info, uint threadId, bool reportErrors = false) {
            var affinityMask = 1UL << (int)threadId;
            uint maxCpuidExtLen;

            var registers = new CpuidRegisters();
            if (!ReadTxOut(Cpuid0, 0, ref registers, affinityMask)) {
                if (reportErrors) {
                    Logger.Error($"Unnable to read CPUID from thread {threadId}, out of range?");
                }

                return false;
            }

            if (registers.Eax <= Cpuid0) {
                if (reportErrors) {
                    Logger.Warn($"Empty CPUID information in thread {threadId}");
                }

                return false;
            }

            var maxCpuidLen = registers.Eax;
            info.Vendor = VendorFromCpuidString(registers);

            registers.Clear();
            if (!ReadTxOut(CpuidExt, 0, ref registers, affinityMask)) {
                if (reportErrors) {
                    Logger.Error($"Unnable to read extended CPUID from thread {threadId}, not supported?");
                }

                return false;
            }

            if (registers.Eax > CpuidExt) {
                maxCpuidExtLen = registers.Eax - CpuidExt;
            } else {
                if (reportErrors) {
                    Logger.Warn($"Empty extended CPUID information in thread {threadId}");
                }

                return false;
            }

            maxCpuidLen = Math.Min(maxCpuidLen, 1024);
            maxCpuidExtLen = Math.Min(maxCpuidExtLen, 1024);

            // Read short core thread info
            var dataRegisters = new CpuidRegisters[maxCpuidLen + 1];
            for (uint i = 0; i < maxCpuidLen + 1; i++) {
                dataRegisters[i] = new CpuidRegisters();
                ReadTxOut(Cpuid0 + i, 0, ref dataRegisters[i], affinityMask);
            }

            info.IsSupportMsr = (dataRegisters[0].Edx & 0x20) != 0;
            info.IsSupportTsc = (dataRegisters[0].Edx & 0x10) != 0;

            // Read full core thread info
            var extDataRegisters = new CpuidRegisters[maxCpuidExtLen + 1];
            for (uint i = 0; i < maxCpuidExtLen + 1; i++) {
                extDataRegisters[i] = new CpuidRegisters();
                ReadTxOut(CpuidExt + i, 0, ref extDataRegisters[i], affinityMask);
            }

            info.IsSupportInvariantTsc = (extDataRegisters[0].Edx & 0x100) != 0;

            var coreNameBuilder = new StringBuilder();
            for (uint i = 2; i <= 4; i++) {
                ReadTxOut(CpuidExt + i, 0, ref registers, affinityMask);
                registers.GetVendorExtendedString(ref coreNameBuilder);
            }

            info.BrandName = coreNameBuilder.ToString().Trim().TrimEnd(' ');
            info.Name = RemoveSpechialBrandSymbolsFromString(coreNameBuilder);
            return true;
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

        private long[] _totalTimes;
        private long[] _idleTimes;

        public float GetCpuLoad(){
            long[] newIdleTimes;
            long[] newTotalTimes;

            var total = 0f;
            if (!GetCpuLoadTime(out newIdleTimes, out newTotalTimes)) {
                return -1f;
            }

            if (newIdleTimes == null || newTotalTimes == null) {
                return -1f;
            }

            if (_idleTimes == null) {
                _totalTimes = newTotalTimes;
                _idleTimes = newIdleTimes;
                return GetCpuLoad();
            }

            for (uint core = 0; core < LogicalCores; core++) {
                total += (float)(newIdleTimes[core] - _idleTimes[core]) / (newTotalTimes[core] - _totalTimes[core]);
            }

            if (LogicalCores > 0) {
                total = 1.0f - total / LogicalCores;
                total = total < 0 ? 0 : total;
            } else {
                total = 0;
            }

            _idleTimes = newIdleTimes;
            _totalTimes = newIdleTimes;
            return total * 100;
        }

        private float GetCoreLoadPercent(uint coreId){
            if (coreId > LogicalCores) {
                return -1f;
            }

            long[] newIdleTimes;
            long[] newTotalTimes;

            if (!GetCpuLoadTime(out newIdleTimes, out newTotalTimes)) {
                return -1f;
            }

            if (newIdleTimes == null || newTotalTimes == null) {
                return -1f;
            }

            var totalLoad = (float)newIdleTimes[coreId] / newTotalTimes[coreId];
            if (totalLoad > 0) {
                totalLoad = 1.0f - totalLoad / LogicalCores;
                totalLoad = totalLoad < 0 ? 0 : totalLoad;
            } else {
                totalLoad = 0;
            }

            return totalLoad;
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

        internal static CpuVendor VendorFromCpuidString(CpuidRegisters registers) {
            switch (registers.GetVendorString()) {
                case "GenuineIntel":
                    return CpuVendor.Intel;
                case "AuthenticAMD":
                    return CpuVendor.Amd;
                default:
                    return CpuVendor.Unknown;
            }
        }

        private static bool GetCpuLoadTime(out long[] idle, out long[] total) {
            var informations = new SSystemProcessorPerformanceInformation[64];
            var size = Marshal.SizeOf(typeof(SSystemProcessorPerformanceInformation));

            idle = null;
            total = null;

            if (NativeNt.NtQuerySystemInformation(
                SystemInformationClass.SystemProcessorPerformanceInformation, 
                informations,
                informations.Length * size, 
                out var returnLength) != 0) 
            {
                return false;
            }

            idle = new long[(int)returnLength / size];
            total = new long[(int)returnLength / size];

            for (var i = 0; i < idle.Length; i++) {
                idle[i] = informations[i].IdleTime;
                total[i] = informations[i].KernelTime + informations[i].UserTime;
            }

            return true;
        }

        public override string ToString(){
            return $"CPUID [Vendor={Vendor}, LogicalCores={LogicalCores}, BrandName='{BrandName}', Name='{Name}']";
        }
    }
}
