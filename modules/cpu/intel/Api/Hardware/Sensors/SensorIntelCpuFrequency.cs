using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using cpu_intel.Api.Hardware.Sensors.Base;
using server.Drivers.Kernel;
using server.Drivers.Presentation;
using server.Hardware;
using server.Utils;

namespace cpu_intel.Api.Hardware.Sensors
{
    [SensorRegister]
    public class SensorIntelCpuFrequency : SensorBaseIntelCpuMulti
    {
        private readonly ISensorElement[] _sensors;
        private readonly SensorElementIntelCpuCoreFrequency[] _coreFrequenciesSensors;
        private readonly SensorElementIntelCpuBaseClock _baseClock;

        public SensorIntelCpuFrequency(IntelCpu cpu)
            : base(cpu) {
            _coreFrequenciesSensors = new SensorElementIntelCpuCoreFrequency[Cpu.CoreCount];
            for (ushort i = 0; i < _coreFrequenciesSensors.Length; i++) {
                _coreFrequenciesSensors[i] = new SensorElementIntelCpuCoreFrequency(i);
            }

            _baseClock = new SensorElementIntelCpuBaseClock();

            _sensors = new ISensorElement[_coreFrequenciesSensors.Length + 1];
            for (var i = 0; i < _coreFrequenciesSensors.Length; i++) {
                _sensors[i] = _coreFrequenciesSensors[i];
            }

            _sensors[_coreFrequenciesSensors.Length] = _baseClock;
        }
        
        public override SensorType GetSensorType() {
            return SensorType.Clocks;
        }

        public override string GetDisplayName() {
            return "Cores Frequency";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }

        public override void TickSpecificLoad(CpuidProcessorInfo info) {
            var tscSupport = info.IsTimeStampCounterSupported && info.TimeStampCounterFrequency > 0;

            double newBusClock = 0;
            foreach (var coreFrequencySensor in _coreFrequenciesSensors) {
                coreFrequencySensor.SetActive(tscSupport);
                if (!tscSupport) {
                    continue;
                }

                uint eax = 0, edx = 0;
                if (Msr.ReadTx(IntelCpu.Ia32PerfStatus, ref eax, ref edx, 
                    (UIntPtr)(1UL << (int)Cpu.GetCpuidProcessorInfo()[0, 0].ThreadIndex))) 
                {
                    newBusClock = Cpu.GetCpuidProcessorInfo().TimeStampCounterFrequency / Cpu.TimeStampCounterMultiper;
                    switch (Cpu.Microarchitecture) {
                        case MicroArchitecture.Nehalem: {
                            var multiplier = eax & 0xff;
                            coreFrequencySensor.Update((float)(multiplier * newBusClock));
                        } break;
                        case MicroArchitecture.SandyBridge:
                        case MicroArchitecture.IvyBridge:
                        case MicroArchitecture.Haswell:
                        case MicroArchitecture.Broadwell:
                        case MicroArchitecture.Silvermont:
                        case MicroArchitecture.Skylake: {
                            var multiplier = (eax >> 8) & 0xff;
                            coreFrequencySensor.Update((float)(multiplier * newBusClock));
                        } break;
                        default: {
                            var multiplier = ((eax >> 8) & 0x1f) + 0.5 * ((eax >> 14) & 1);
                            coreFrequencySensor.Update((float)(multiplier * newBusClock));
                        } break;
                    } 
                } else {
                    // If IA32_PERF_STATUS is not available, assume TSC frequency
                    coreFrequencySensor.Update((float)Cpu.GetCpuidProcessorInfo().TimeStampCounterFrequency);
                }
            }

            if (newBusClock > 0) {
                _baseClock.Update((float)newBusClock);
            }

            _baseClock.SetActive(newBusClock > 0);
            IsSensorActive = newBusClock > 0;
        }

        public override ISensorElement[] GetElements() {
            return _sensors;
        }
    }

    internal class SensorElementIntelCpuBaseClock : SensorElementBaseIntelCpu<float>
    {
        public override void Update(float load) {
            Value = load;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "MHz";
        }

        public override string GetSystemTag() {
            return "intel_base_clock";
        }
    }

    internal class SensorElementIntelCpuCoreFrequency : SensorElementBaseIntelCpu<float>
    {
        private readonly uint _coreIndex;

        public SensorElementIntelCpuCoreFrequency(uint coreIndex) {
            _coreIndex = coreIndex;
        }

        public override void Update(float load) {
            Value = load;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "MHz";
        }

        public override string GetSystemTag() {
            return $"intel_core_{_coreIndex}_clock";
        }
    }
}
