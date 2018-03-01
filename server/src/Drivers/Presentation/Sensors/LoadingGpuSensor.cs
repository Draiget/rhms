using System;

namespace server.Drivers.Presentation.Sensors
{
    public class LoadingGpuSensor : GenericGpuSensor
    {
        public float Load;

        public LoadingGpuSensor(string name)
            : base(name){
        }

        public override void Update() {
            throw new NotImplementedException();
        }
    }
}
