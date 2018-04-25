using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cpu_intel.Api.Hardware.Sensors.Base;
using server.Drivers.Presentation;
using server.Hardware;
using server.Hardware.CPU;
using server.Utils;

namespace cpu_intel.Api.Hardware.Sensors
{
    [SensorRegister]
    public class SensorIntelCoresLoad : SensorBaseIntelCpuMulti
    {
        private readonly SensorElementIntelCpuCoreLoad[] _coreSensors;
        private readonly SensorElementIntelCpuTotalLoad _cpuTotalLoad;
        private readonly ISensorElement[] _sensors;

        public SensorIntelCoresLoad(IntelCpu cpu)
            : base(cpu) 
        {
            _coreSensors = new SensorElementIntelCpuCoreLoad[cpu.CoreCount];
            for (ushort i = 0; i < _coreSensors.Length; i++) {
                _coreSensors[i] = new SensorElementIntelCpuCoreLoad(i);
            }

            _cpuTotalLoad = new SensorElementIntelCpuTotalLoad();

            _sensors = new ISensorElement[_coreSensors.Length + 1];
            for (var i = 0; i < _coreSensors.Length; i++) {
                _sensors[i] = _coreSensors[i];
            }

            _sensors[_coreSensors.Length] = _cpuTotalLoad;

            InitLoadData();
        }

        public override void TickSpecificLoad(CpuidProcessorInfo info) {
            UpdateLoadData();

            for (var coreId = 0; coreId < _coreSensors.Length; coreId++) {
                _coreSensors[coreId].Update(_coreLoads[coreId]);
                _coreSensors[coreId].SetActive(IsSensorActive);
            }

            _cpuTotalLoad.Update(_totalLoad);
        }

        public override ISensorElement[] GetElements() {
            return _sensors;
        }

        public override string GetDisplayName() {
            return "Cores Load Info";
        }

        public override string GetSystemName() {
            return GetDisplayName().ConvertToIdString();
        }

        private float[] _coreLoads;
        private long[] _idleTimes;
        private long[] _totalTimes;
        private float _totalLoad;

        private void InitLoadData() {
            _coreLoads = new float[Cpu.CoreCount];
            _totalLoad = 0;

            try {
                CpuidProcessorInfo.GetCpuLoadTime(out _idleTimes, out _totalTimes);
            } catch {
                _idleTimes = null;
                _totalTimes = null;
            }

            IsSensorActive = _idleTimes != null;
        }

        private void UpdateLoadData() {
            if (_idleTimes == null || !IsSensorActive) {
                return;
            }

            if (!CpuidProcessorInfo.GetCpuLoadTime(out var newIdleTimes, out var newTotalTimes)) {
                return;
            }

            for (var i = 0; i < Math.Min(newTotalTimes.Length, _totalTimes.Length); i++) {
                if (newTotalTimes[i] - _totalTimes[i] < 100000) {
                    return;
                }
            }

            if (newIdleTimes == null) {
                return;
            }

            var cores = Cpu.GetCpuidProcessorInfo().Cores;
            var total = 0f;
            var count = 0;
            for (var coreId = 0u; coreId < cores.Count; coreId++) {
                var value = 0f;
                for (var threadId = 0; threadId < cores[coreId].Count; threadId++) {
                    var index = cores[coreId][threadId].ThreadId;
                    if (index < newIdleTimes.Length && index < _totalTimes.Length) {
                        var idle =
                            (newIdleTimes[index] - _idleTimes[index]) /
                            (float)(newTotalTimes[index] - _totalTimes[index]);

                        value += idle;
                        total += idle;
                        count++;
                    }
                }

                value = 1.0f - value / cores[coreId].Count;
                value = value < 0 ? 0 : value;
                _coreLoads[coreId] = value * 100;
            }

            if (count > 0) {
                total = 1.0f - total / count;
                total = total < 0 ? 0 : total;
            } else {
                total = 0;
            }

            _totalLoad = total * 100;
            _totalTimes = newTotalTimes;
            _idleTimes = newIdleTimes;
        }
    }

    internal class SensorElementIntelCpuCoreLoad : SensorElementBaseIntelCpu<float>
    {
        private readonly uint _coreIndex;

        public SensorElementIntelCpuCoreLoad(uint coreIndex) {
            _coreIndex = coreIndex;
        }

        public override void Update(float load) {
            Value = load;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "%";
        }

        public override string GetSystemTag() {
            return $"core_{_coreIndex}_load";
        }
    }

    internal class SensorElementIntelCpuTotalLoad : SensorElementBaseIntelCpu<float>
    {
        public override void Update(float load) {
            Value = load;
            AfterUpdate();
        }

        public override string GetMeasurement() {
            return "%";
        }

        public override string GetSystemTag() {
            return "core_total_load";
        }
    }
}
