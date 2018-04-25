using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules.Base;

namespace server.Hardware
{
    public interface ISensorBase
    {
        bool Initialize();

        bool IsAvaliable();

        bool IsActive();

        void Tick();

        SensorType GetSensorType();

        string GetDisplayName();

        string GetSystemName();

        BaseModule GetModuleHandle();
    }
}
