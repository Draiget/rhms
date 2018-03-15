using server.Hardware;
using server.Hardware.GPU;

namespace gpu_nvidia.Hardware
{
    public class NVidiaGpu : GenericGpu
    {
        public NVidiaGpu(){
            
        }

        public override void InitializeInformation(){
            
        }

        public override HardwareIdentifer Identify(){
            throw new System.NotImplementedException();
        }

        public override void TickUpdate() {
            throw new System.NotImplementedException();
        }
    }
}
