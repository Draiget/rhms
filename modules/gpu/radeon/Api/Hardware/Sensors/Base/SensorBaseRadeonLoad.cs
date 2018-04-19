using gpu_radeon.Api.Structures;
using server.Hardware;
using server.Hardware.GPU;
using server.Modules.Base;

namespace gpu_radeon.Api.Hardware.Sensors.Base
{
    public abstract class SensorBaseRadeonLoad : BaseGpuSensor
    {
        protected new RadeonGpu Gpu;
        protected AdlpmActivity Activity;
        protected double Value;

        protected bool MinMaxSet;
        protected double MinValue;
        protected double MaxValue;

        protected SensorBaseRadeonLoad(RadeonGpu gpu)
            : base(gpu) {
            Gpu = gpu;
            MinMaxSet = false;
        }

        public override double GetValue() {
            return Value;
        }

        public override bool InitSensor() {
            return true;
        }

        public override double GetMin() {
            return MinValue;
        }

        public override double GetMax() {
            return MaxValue;
        }

        public override void Tick() {
            if (ObtainPmActivity()) {
                TickSpecificLoad(Activity);
            }
        }

        public abstract void TickSpecificLoad(AdlpmActivity activity);

        public bool ObtainPmActivity() {
            return AdlApi.AdlGetCurrentActivity(Gpu.AdapterIndex, ref Activity) == AdlApi.AdlOk;
        }

        public override SensorType GetSensorType() {
            return SensorType.Utilization;
        }

        public override BaseModule GetModuleHandle() {
            return ModuleGpuRadeon.ModuleHandle;
        }
    }
}
