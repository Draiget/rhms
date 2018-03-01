using System;

namespace server.Drivers.Presentation.Sensors
{
    public class ThermalGpuSensor : GenericGpuSensor
    {
        public float Temperature;

        public ThermalGpuSensor(string name)
            : base(name){
        }

        public override void Update() {
            throw new NotImplementedException();
        }
    }
}
