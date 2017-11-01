using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
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

        public CpuidCoreInfo[] CoresInfo {
            get;
            private set;
        }

        public string BrandName {
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
            uint maxCpuidLen = 0;
            uint maxCpuidExtLen = 0;

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

            maxCpuidLen = registers.Eax;
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
                ReadTxOut(CpuidExt + i, 0, ref dataRegisters[i], affinityMask);
            }

            // Read full core thread info
            var extDataRegisters = new CpuidRegisters[maxCpuidExtLen + 1];
            for (uint i = 0; i < maxCpuidExtLen + 1; i++) {
                extDataRegisters[i] = new CpuidRegisters();
                ReadTxOut(CpuidExt + i, 0, ref extDataRegisters[i], affinityMask);
            }

            var coreNameBuilder = new StringBuilder();
            for (uint i = 2; i <= 4; i++) {
                ReadTxOut(CpuidExt + i, 0, ref registers, affinityMask);
                registers.GetVendorExtendedString(ref coreNameBuilder);
            }

            coreNameBuilder.Replace('\0', ' ');

            info.BrandName = coreNameBuilder.ToString().Trim();
            return true;
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

        public override string ToString(){
            return $"CPUID [Vendor={Vendor}, LogicalCores={LogicalCores}, BrandName={BrandName}]";
        }
    }
}
