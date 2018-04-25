using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using server.Utils;
using server.Utils.Natives;

namespace server.Drivers.Presentation
{
    public class CpuidThreadInfo
    {
        private readonly CpuidRegisters[] _dataRegisters;
        private readonly CpuidRegisters[] _extDataRegisters;

        public CpuVendor Vendor {
            get;
            private set;
        }

        public uint ThreadId {
            get;
            private set;
        }

        public uint ThreadIndex {
            get;
            private set;
        }

        public uint CoreId {
            get;
            private set;
        }

        public uint ProcessorId {
            get;
            private set;
        }

        public uint Family {
            get;
            private set;
        }

        public uint Model {
            get;
            private set;
        }

        public uint Stepping {
            get;
            private set;
        }

        public uint ApicId {
            get;
            private set;
        }

        public string Name {
            get;
            private set;
        }

        public string BrandName {
            get;
            private set;
        }

        private readonly uint _threadMaskWith;
        private readonly uint _coreMaskWith;

        public CpuidThreadInfo(uint threadIndex) {
            ThreadIndex = threadIndex;

            var affinityMask = 1UL << (int)ThreadIndex;
            uint maxCpuidExtLen;

            var registers = new CpuidRegisters();
            if (!Cpuid.ReadTxOut(Cpuid.Cpuid0, 0, ref registers, affinityMask)) {
                throw new Exception($"Unable to get CPUID0 registers from thread {ThreadIndex}");
            }

            if (registers.Eax <= Cpuid.Cpuid0) {
                throw new Exception($"CPUID0_Eax: {registers.Eax} <= {Cpuid.Cpuid0}");
            }

            Vendor = Cpuid.VendorFromCpuidString(registers);
            var maxCpuidLen = registers.Eax;

            registers.Clear();
            if (!Cpuid.ReadTxOut(Cpuid.CpuidExt, 0, ref registers, affinityMask)) {
                throw new Exception("ReadTxOut_CpuidExt: fail");
            }

            if (registers.Eax > Cpuid.CpuidExt) {
                maxCpuidExtLen = registers.Eax - Cpuid.CpuidExt;
            } else {
                throw new Exception($"CPUIDExt_Eax: {registers.Eax} <= {Cpuid.CpuidExt}");
            }

            maxCpuidLen = Math.Min(maxCpuidLen, 1024);
            maxCpuidExtLen = Math.Min(maxCpuidExtLen, 1024);

            // Read short core thread info
            var dataRegisters = new CpuidRegisters[maxCpuidLen + 1];
            for (uint i = 0; i < maxCpuidLen + 1; i++) {
                dataRegisters[i] = new CpuidRegisters();
                Cpuid.ReadTxOut(Cpuid.Cpuid0 + i, 0, ref dataRegisters[i], affinityMask);
            }
            
            // Read full core thread info
            var extDataRegisters = new CpuidRegisters[maxCpuidExtLen + 1];
            for (uint i = 0; i < maxCpuidExtLen + 1; i++) {
                extDataRegisters[i] = new CpuidRegisters();
                Cpuid.ReadTxOut(Cpuid.CpuidExt + i, 0, ref extDataRegisters[i], affinityMask);
            }

            var coreNameBuilder = new StringBuilder();
            for (uint i = 2; i <= 4; i++) {
                Cpuid.ReadTxOut(Cpuid.CpuidExt + i, 0, ref registers, affinityMask);
                registers.GetVendorExtendedString(ref coreNameBuilder);
            }

            BrandName = coreNameBuilder.ToString().Trim().TrimEnd(' ').Replace("\0", string.Empty);
            Name = Cpuid.RemoveSpechialBrandSymbolsFromString(coreNameBuilder);

            Family = ((dataRegisters[1].Eax & 0x0FF00000) >> 20) + ((dataRegisters[1].Eax & 0x0F00) >> 8);
            Model = ((dataRegisters[1].Eax & 0x0F0000) >> 12) + ((dataRegisters[1].Eax & 0xF0) >> 4);
            Stepping = dataRegisters[1].Eax & 0x0F;
            ApicId = (dataRegisters[1].Ebx >> 24) & 0xFF;

            if (Vendor == CpuVendor.Intel) {
                var maxCoreAndThreadIdPerPackage = (dataRegisters[1].Ebx >> 16) & 0xFF;
                uint maxCoreIdPerPackage;
                if (maxCpuidLen >= 4) {
                    maxCoreIdPerPackage = ((dataRegisters[4].Eax >> 26) & 0x3F) + 1;
                } else {
                    maxCoreIdPerPackage = 1;
                }

                _threadMaskWith = MathUtils.NextLog2(maxCoreAndThreadIdPerPackage / maxCoreIdPerPackage);
                _coreMaskWith = MathUtils.NextLog2(maxCoreIdPerPackage);
            } else if (Vendor == CpuVendor.Amd) {
                uint corePerPackage;
                if (maxCpuidExtLen >= 8) {
                    corePerPackage = (extDataRegisters[8].Ecx & 0xFF) + 1;
                } else {
                    corePerPackage = 1;
                }

                _threadMaskWith = 0;
                _coreMaskWith = MathUtils.NextLog2(corePerPackage);
            } else {
                _threadMaskWith = 0;
                _coreMaskWith = 0;
            }

            ProcessorId = ApicId >> (int)(_coreMaskWith + _threadMaskWith);
            CoreId = (ApicId >> (int)_threadMaskWith) - (ProcessorId << (int)_coreMaskWith);
            ThreadId = ApicId - (ProcessorId << (int)(_coreMaskWith + _threadMaskWith)) - (CoreId << (int)_threadMaskWith);
        }

        public override string ToString() {
            return $"ProcessorThreadCPUID [ProcessorId={ProcessorId}, CoreId={CoreId}, ThreadId={ThreadId}, Vendor={Vendor}, Name='{Name}']";
        }
    }
}
