using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Drivers.Presentation
{
    public abstract class GenericGpuSensor
    {
        public string Name {
            get;
        }

        protected GenericGpuSensor(string name){
            Name = name;
        }

        public abstract void Update();
    }
}
