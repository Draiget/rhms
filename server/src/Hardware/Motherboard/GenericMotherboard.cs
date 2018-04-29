using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware.CPU;

namespace server.Hardware.Motherboard
{
    public class GenericMotherboard : BaseSensorsHarwdare<BaseMotherboardSensor>, IHardware
    {
        public override HardwareIdentifer Identify() {
            throw new NotImplementedException();
        }

        public override void TickUpdate() {
            throw new NotImplementedException();
        }
    }
}
