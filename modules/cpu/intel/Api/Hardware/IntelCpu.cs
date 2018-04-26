using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Drivers.Kernel;
using server.Drivers.Presentation;
using server.Hardware;
using server.Hardware.CPU;
using server.Hardware.GPU;
using server.Utils;

namespace cpu_intel.Api.Hardware
{
    public class IntelCpu : GenericCpu
    {
        public const uint Ia32TemperatureTarget = 0x01A2;
        public const uint Ia32PerfStatus = 0x0198;
        public const uint MsrPlatformInfo = 0xCE;

        public string ModelName;
        public string FullName;

        private float[] _tjMax;

        public double TimeStampCounterMultiper {
            get;
            private set;
        }

        public MicroArchitecture Microarchitecture {
            get;
            private set;
        }

        public IntelCpu(CpuidProcessorInfo cpuInfo)
            : base(cpuInfo) {
            ModelName = cpuInfo[0, 0].Name;
            FullName = cpuInfo[0, 0].BrandName;

            DetectMicroarchitecture();
            DetectTimeStampCounterMultiper();
        }

        private void DetectTimeStampCounterMultiper() {
            switch (Microarchitecture) {
                case MicroArchitecture.NetBurst:
                case MicroArchitecture.Atom:
                case MicroArchitecture.Core: {
                    uint eax = 0, edx = 0;
                    if (Msr.Read(Ia32PerfStatus, ref eax, ref edx)) {
                        TimeStampCounterMultiper = ((edx >> 8) & 0x1f) + 0.5 * ((edx >> 14) & 1);
                    }
                }
                    break;
                case MicroArchitecture.Nehalem:
                case MicroArchitecture.SandyBridge:
                case MicroArchitecture.IvyBridge:
                case MicroArchitecture.Haswell:
                case MicroArchitecture.Broadwell:
                case MicroArchitecture.Silvermont:
                case MicroArchitecture.Skylake:
                case MicroArchitecture.Airmont:
                case MicroArchitecture.KabyLake: {
                    uint eax = 0, edx = 0;
                    if (Msr.Read(MsrPlatformInfo, ref eax, ref edx)) {
                        TimeStampCounterMultiper = (eax >> 8) & 0xff;
                    }
                }
                    break;
                default:
                    TimeStampCounterMultiper = 0;
                    break;
            }
        }

        private void DetectMicroarchitecture() {
            // https://en.wikichip.org/wiki/intel/microarchitectures
            switch (Family) {
                case 0x06: {
                    switch (ModelId) {
                        // Intel Core 2 (65nm)
                        case 0x0F:
                            ApplyMicroarchitecture65Nm();
                            break;
                        // Intel Core 2 (45nm)
                        case 0x17:
                            ApplyMicroarchitecture45Nm();
                            break;
                        // Intel Atom (45nm)
                        case 0x1C:
                            ApplyMicroarchitecture45NmAtom();
                            break;
                        case 0x1A: // Intel Core i7 LGA1366 (45nm)
                        case 0x1E: // Intel Core i5, i7 LGA1156 (45nm)
                        case 0x1F: // Intel Core i5, i7 
                        case 0x25: // Intel Core i3, i5, i7 LGA1156 (32nm)
                        case 0x2C: // Intel Core i7 LGA1366 (32nm) 6 Core
                        case 0x2E: // Intel Xeon Processor 7500 series (45nm)
                        case 0x2F: // Intel Xeon Processor (32nm)
                            Microarchitecture = MicroArchitecture.Nehalem;
                            _tjMax = GetTjMaxFromMsr();
                            break;
                        case 0x2A: // Intel Core i5, i7 2xxx LGA1155 (32nm)
                        case 0x2D: // Next Generation Intel Xeon, i7 3xxx LGA2011 (32nm)
                            Microarchitecture = MicroArchitecture.SandyBridge;
                            _tjMax = GetTjMaxFromMsr();
                            break;
                        case 0x3A: // Intel Core i5, i7 3xxx LGA1155 (22nm)
                        case 0x3E: // Intel Core i7 4xxx LGA2011 (22nm)
                            Microarchitecture = MicroArchitecture.IvyBridge;
                            _tjMax = GetTjMaxFromMsr();
                            break;
                        case 0x3C: // Intel Core i5, i7 4xxx LGA1150 (22nm)              
                        case 0x3F: // Intel Xeon E5-2600/1600 v3, Core i7-59xx
                        // LGA2011-v3, Haswell-E (22nm)
                        case 0x45: // Intel Core i5, i7 4xxxU (22nm)
                        case 0x46:
                            Microarchitecture = MicroArchitecture.Haswell;
                            _tjMax = GetTjMaxFromMsr();
                            break;
                        case 0x3D: // Intel Core M-5xxx (14nm)
                        case 0x47: // Intel i5, i7 5xxx, Xeon E3-1200 v4 (14nm)
                        case 0x4F: // Intel Xeon E5-26xx v4
                        case 0x56: // Intel Xeon D-15xx
                            Microarchitecture = MicroArchitecture.Broadwell;
                            _tjMax = GetTjMaxFromMsr();
                            break;
                        case 0x36: // Intel Atom S1xxx, D2xxx, N2xxx (32nm)
                            Microarchitecture = MicroArchitecture.Atom;
                            _tjMax = GetTjMaxFromMsr();
                            break;
                        case 0x37: // Intel Atom E3xxx, Z3xxx (22nm)
                        case 0x4A:
                        case 0x4D: // Intel Atom C2xxx (22nm)
                        case 0x5A:
                        case 0x5D:
                            Microarchitecture = MicroArchitecture.Silvermont;
                            _tjMax = GetTjMaxFromMsr();
                            break;
                        case 0x4E:
                        case 0x5E: // Intel Core i5, i7 6xxxx LGA1151 (14nm)
                            Microarchitecture = MicroArchitecture.Skylake;
                            _tjMax = GetTjMaxFromMsr();
                            break;
                        case 0x4C:
                            Microarchitecture = MicroArchitecture.Airmont;
                            _tjMax = GetTjMaxFromMsr();
                            break;
                        case 0x8E: // Intel Core i5, i7 7xxxx (14nm)
                            Microarchitecture = MicroArchitecture.KabyLake;
                            _tjMax = GetTjMaxFromMsr();
                            break;
                        case 0x9E: // Intel Core i3, i5, i7, i9 (14nm)
                            Microarchitecture = MicroArchitecture.CoffeeLake;
                            _tjMax = GetTjMaxFromMsr();
                            break;
                        default:
                            Microarchitecture = MicroArchitecture.Unknown;
                            _tjMax = FillCoresFloats(100);
                            break;
                    }

                    break;
                }
                case 0x0F: {
                    switch (ModelId) {
                        case 0x00: // Pentium 4 (180nm)
                        case 0x01: // Pentium 4 (130nm)
                        case 0x02: // Pentium 4 (130nm)
                        case 0x03: // Pentium 4, Celeron D (90nm)
                        case 0x04: // Pentium 4, Pentium D, Celeron D (90nm)
                        case 0x06: // Pentium 4, Pentium D, Celeron D (65nm)
                            Microarchitecture = MicroArchitecture.NetBurst;
                            _tjMax = FillCoresFloats(100);
                            break;
                        default:
                            Microarchitecture = MicroArchitecture.Unknown;
                            _tjMax = FillCoresFloats(100);
                            break;
                    }

                    break;
                }
                default:
                    Microarchitecture = MicroArchitecture.Unknown;
                    _tjMax = FillCoresFloats(100);
                    break;
            }
        }

        private void ApplyMicroarchitecture45NmAtom() {
            Microarchitecture = MicroArchitecture.Atom;
            switch (Stepping) {
                case 0x0A: // A0, B0
                    _tjMax = FillCoresFloats(100);
                    break;
                case 0x02: // C0
                default:
                    _tjMax = FillCoresFloats(90);
                    break;
            }
        }

        private void ApplyMicroarchitecture45Nm() {
            Microarchitecture = MicroArchitecture.Core;
            _tjMax = FillCoresFloats(100);
        }

        private void ApplyMicroarchitecture65Nm() {
            Microarchitecture = MicroArchitecture.Core;
            switch (Stepping) {
                case 0x06: // B2
                    if (CoreCount == 2) {
                        _tjMax = FillCoresFloats(80 + 10);
                        break;
                    }

                    if (CoreCount == 4) {
                        _tjMax = FillCoresFloats(90 + 10);
                        break;
                    }

                    _tjMax = FillCoresFloats(85 + 10);
                    break;
                case 0x0B: // G0
                    _tjMax = FillCoresFloats(90 + 10);
                    break;
                case 0x0D: // M0
                    _tjMax = FillCoresFloats(85 + 10);
                    break;
                default:
                    _tjMax = FillCoresFloats(85 + 10);
                    break;
            }
        }

        private float[] GetTjMaxFromMsr() {
            uint eax = 0, edx = 0;
            var result = new float[CoreCount];
            for (uint i = 0; i < CoreCount; i++) {
                if (Msr.ReadTx(Ia32TemperatureTarget, ref eax, ref edx, (UIntPtr)(1UL << (int)ProcessorCpuid[i, 0].ThreadIndex))) {
                    result[i] = (eax >> 16) & 0xFF;
                } else {
                    result[i] = 100;
                }
            }
            return result;
        }

        public int CoreCount => ProcessorCpuid.GetCoresCount();

        public int ThreadsCount => ProcessorCpuid.GetThreadsCount();

        public uint Family => ProcessorCpuid[0, 0].Family;

        public uint ModelId => ProcessorCpuid[0, 0].Model;

        public uint Stepping => ProcessorCpuid[0, 0].Stepping;

        public override void InitializeSensors() {
            var sensors = AssemblyUtils.GetAttributesInModule(typeof(IntelCpu), typeof(SensorRegisterAttribute));
            foreach (var sensor in sensors) {
                AddAvaliableSensor((BaseCpuSensor)Activator.CreateInstance(sensor, this));
            }
        }

        public override HardwareIdentifer Identify() {
            return new IntelCpuIdentifier(this);
        }

        public override void TickUpdate() {
            var sensors = GetSensors();
            foreach (var sensor in sensors) {
                if (sensor.IsAvaliable()) {
                    sensor.Tick();
                }
            }
        }

        private float[] FillCoresFloats(float value) {
            var result = new float[CoreCount];
            for (var i = 0; i < result.Length; i++) {
                result[i] = value;
            }

            return result;
        }

        public override string ToString() {
            return $"IntelCpu [Cores/Threads='{CoreCount}/{ThreadsCount}', Model='{ModelName}', BrandName={FullName}]";
        }
    }
}
