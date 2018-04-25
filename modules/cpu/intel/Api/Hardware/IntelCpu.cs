using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Drivers.Presentation;
using server.Hardware;
using server.Hardware.CPU;
using server.Hardware.GPU;
using server.Utils;

namespace cpu_intel.Api.Hardware
{
    public class IntelCpu : GenericCpu
    {
        public string Model;
        public string FullName;

        public IntelCpu(CpuidProcessorInfo cpuInfo)
            : base(cpuInfo) {
            Model = cpuInfo[0, 0].Name;
            FullName = cpuInfo[0, 0].BrandName;
        }

        public int CoreCount => ProcessorCpuid.GetCoresCount();

        public int ThreadsCount => ProcessorCpuid.GetThreadsCount();

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

        public override string ToString() {
            return $"IntelCpu [Cores/Threads='{CoreCount}/{ThreadsCount}', Model='{Model}', BrandName={FullName}]";
        }
    }
}
