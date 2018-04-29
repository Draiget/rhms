using gpu_radeon.Api.Structures;
using server.Hardware;
using server.Hardware.GPU;
using server.Modules.Base;

namespace gpu_radeon.Api.Hardware.Sensors.Base
{
    public abstract class SensorBaseRadeonLoad : BaseGpuSensor, ISingleValueSensor
    {
        protected new RadeonGpu Gpu;
        protected AdlpmActivity Activity;
        protected double Value;

        protected SensorBaseRadeonLoad(RadeonGpu gpu)
            : base(gpu) {
            Gpu = gpu;
        }

        public override bool InitSensor() {
            return true;
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

        public override BaseModule GetModuleHandle() {
            return ModuleGpuRadeon.ModuleHandle;
        }

        public abstract ISensorElement GetElement();
    }
}
