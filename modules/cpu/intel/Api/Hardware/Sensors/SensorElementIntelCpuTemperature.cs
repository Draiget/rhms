using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SensorElementIntelCpuTemperature : SensorBaseIntelCpuMulti
    {
        private readonly ISensorElement[] _sensors;
        private readonly SensorElementIntelCpuPackageTemp _cpuPackageTemp;
        private readonly SensorElementIntelCpuThreadTemp[] _cpuThreadsTemp;

        public SensorElementIntelCpuTemperature(IntelCpu cpu)
            : base(cpu) 
        {
            var tempSensors = new List<ISensorElement>();
            var cpuidInfo = Cpu.GetCpuidProcessorInfo();
            var core0Info = cpuidInfo[0, 0];
            if (core0Info.DataRegisters.Length > 0 && (core0Info.DataRegisters[6].Eax & 1) != 0 && Cpu.Microarchitecture != MicroArchitecture.Unknown) {
                _cpuThreadsTemp = new SensorElementIntelCpuThreadTemp[Cpu.ThreadsCount];
                foreach (var core in cpuidInfo.Cores) {
                    foreach (var thread in core.Value) {
                        var threadSensor = new SensorElementIntelCpuThreadTemp(core.Key, thread.ThreadIndex);
                        _cpuThreadsTemp[thread.ThreadIndex] = threadSensor;
                        tempSensors.Add(threadSensor);
                    }
                }
            }

            if (core0Info.DataRegisters.Length > 0 && (core0Info.DataRegisters[6].Eax & 0x40) != 0 && Cpu.Microarchitecture != MicroArchitecture.Unknown) {
                _cpuPackageTemp = new SensorElementIntelCpuPackageTemp();
                tempSensors.Add(_cpuPackageTemp);
            }

            _sensors = tempSensors.ToArray();
            IsSensorActive = _sensors.Length > 0;
        }

        public override SensorType GetSensorType() {
            return SensorType.Temperature;
        }

        public override string GetDisplayName() {
            return "CPU Temperatures";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }

        public override void TickSpecificLoad(CpuidProcessorInfo info) {
            foreach (var coreTempSensor in _cpuThreadsTemp) {
                coreTempSensor.Update(Cpu);
            }

            _cpuPackageTemp.Update(Cpu);
        }

        public override ISensorElement[] GetElements() {
            return _sensors;
        }
    }

    internal class SensorElementIntelCpuPackageTemp : SensorElementBaseIntelCpu<IntelCpu>
    {
        private const uint Ia32PackageThermStatus = 0x1B1;

        public override void Update(IntelCpu cpu) {
            uint eax = 0, edx = 0;
            if (Msr.ReadTx(Ia32PackageThermStatus, ref eax, ref edx, (UIntPtr)(1UL << 0)) && (eax & 0x80000000) != 0) {
                // From tjMax from at bits 22 -> 16
                float deltaT = ((eax & 0x007F0000) >> 16);
                var tjMax = cpu.GetTjMaxOfThreadAt(0);
                const float tSlope = 1;
                Value = tjMax - tSlope * deltaT;
                SetActive(true);
            } else {
                Value = -1;
                SetActive(false);
            }
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "°C";
        }

        public override string GetSystemTag() {
            return "temp_package";
        }
    }

    internal class SensorElementIntelCpuThreadTemp : SensorElementBaseIntelCpu<IntelCpu>
    {
        private const uint Ia32ThermStatusMsr = 0x019C;
        private readonly uint _coreIndex;
        private readonly uint _threadIndex;
        private readonly UIntPtr _threadMask;

        public SensorElementIntelCpuThreadTemp(uint coreIndex, uint threadIndex) {
            _coreIndex = coreIndex;
            _threadIndex = threadIndex;
            _threadMask = (UIntPtr)(1UL << (int)threadIndex);
        }

        public override void Update(IntelCpu cpu) {
            uint eax = 0, edx = 0;
            if (Msr.ReadTx(Ia32ThermStatusMsr, ref eax, ref edx, _threadMask) && (eax & 0x80000000) != 0) {
                // From tjMax from at bits 22 -> 16
                float deltaT = (eax & 0x007F0000) >> 16;
                var tjMax = cpu.GetTjMaxOfThreadAt(_threadIndex);
                const float tSlope = 1;
                Value = tjMax - tSlope * deltaT;
                SetActive(true);
            } else {
                Value = -1;
                SetActive(false);
            }

            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "°C";
        }

        public override string GetSystemTag() {
            return $"temp_thread_{_threadIndex}";
        }
    }
}
