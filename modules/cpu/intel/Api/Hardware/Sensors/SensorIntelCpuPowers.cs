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
    public class SensorIntelCpuPowers : SensorBaseIntelCpuMulti
    {
        private const uint MsrRaplPowerUnit = 0x606;
        private const uint MsrPkgEneryStatus = 0x611;
        private const uint MsrDramEnergyStatus = 0x619;
        private const uint MsrPp0EneryStatus = 0x639;
        private const uint MsrPp1EneryStatus = 0x641;

        private const int MsrPowerSensorsCount = 4;
        private float _energyUnitMultiplier;

        private readonly SensorElementIntelCpuPower[] _powerSensors;
        private readonly ISensorElement[] _sensors;

        public SensorIntelCpuPowers(IntelCpu cpu)
            : base(cpu) {
            if (Cpu.Microarchitecture == MicroArchitecture.SandyBridge ||
                Cpu.Microarchitecture == MicroArchitecture.IvyBridge ||
                Cpu.Microarchitecture == MicroArchitecture.Haswell ||
                Cpu.Microarchitecture == MicroArchitecture.Broadwell ||
                Cpu.Microarchitecture == MicroArchitecture.Skylake ||
                Cpu.Microarchitecture == MicroArchitecture.Silvermont ||
                Cpu.Microarchitecture == MicroArchitecture.Airmont ||
                Cpu.Microarchitecture == MicroArchitecture.CoffeeLake ||
                Cpu.Microarchitecture == MicroArchitecture.KabyLake) 
            {
                _powerSensors = new SensorElementIntelCpuPower[MsrPowerSensorsCount];
                InitializePowerSensors();

                _sensors = new ISensorElement[_powerSensors.Length];
                for (var i = 0; i < _powerSensors.Length; i++) {
                    if (_powerSensors[i] != null) {
                        _sensors[i] = _powerSensors[i];
                    }
                }
                return;
            }

            _sensors = new ISensorElement[0];
        }

        public override SensorType GetSensorType() {
            return SensorType.Power;
        }

        public override string GetDisplayName() {
            return "CPU Powers";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }

        public override void TickSpecificLoad(CpuidProcessorInfo info) {
            foreach (var sensor in _powerSensors) {
                if (sensor != null && sensor.IsActive()) {
                    sensor.Update(_energyUnitMultiplier);
                }
            }
        }

        private void InitializePowerSensors() {
            _energyUnitMultiplier = 0f;
            uint eax = 0, edx = 0;
            if (Msr.Read(MsrRaplPowerUnit, ref eax, ref edx)) {
                switch (Cpu.Microarchitecture) {
                    case MicroArchitecture.Silvermont:
                    case MicroArchitecture.Airmont:
                        _energyUnitMultiplier = 1.0e-6f * (1 << (int) ((eax >> 8) & 0x1F));
                        break;
                    default:
                        _energyUnitMultiplier = 1.0f / (1 << (int) ((eax >> 8) & 0x1F));
                        break;
                }
            }

            if (_energyUnitMultiplier != 0) {
                _powerSensors[0] = new SensorElementIntelCpuPower("package", MsrPkgEneryStatus);
                _powerSensors[1] = new SensorElementIntelCpuPower("cores", MsrPp0EneryStatus);
                _powerSensors[2] = new SensorElementIntelCpuPower("graphics", MsrPp1EneryStatus);
                _powerSensors[3] = new SensorElementIntelCpuPower("dram", MsrDramEnergyStatus);
            }
        }

        public override ISensorElement[] GetElements() {
            return _sensors;
        }
    }

    internal class SensorElementIntelCpuPower : SensorElementBaseIntelCpu<float>
    {
        private readonly string _powerNameId;
        private readonly uint _powerTypeMsr;
        private uint _lastEnergyConsumed;
        private DateTime _lastEnergyTime;

        public SensorElementIntelCpuPower(string id, uint powerType) {
            _powerNameId = id;
            _powerTypeMsr = powerType;

            uint eax = 0, edx = 0;
            if (!Msr.Read(_powerTypeMsr, ref eax, ref edx)) {
                SetActive(false);
                return;
            }

            _lastEnergyTime = DateTime.UtcNow;
            _lastEnergyConsumed = eax;
            SetActive(true);
        }

        public override void Update(float energyUnitMultiplier) {
            uint eax = 0, edx = 0;
            if (!Msr.Read(_powerTypeMsr, ref eax, ref edx)) {
                return;
            }

            var time = DateTime.UtcNow;
            var energyConsumed = eax;
            var deltaTime = (float)(time - _lastEnergyTime).TotalSeconds;
            if (deltaTime < 0.01) {
                return;
            }

            Value = energyUnitMultiplier * unchecked(energyConsumed - _lastEnergyConsumed) / deltaTime;
            _lastEnergyTime = time;
            _lastEnergyConsumed = energyConsumed;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "W";
        }

        public override string GetSystemTag() {
            return $"power_{_powerNameId}";
        }
    }
}
