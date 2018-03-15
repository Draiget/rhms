using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Hardware
{
    public interface ISensor
    {
        bool Initialize();

        bool IsAvaliable();

        bool IsActive();

        double GetMax();

        double GetMin();

        double GetValue();

        void Tick();

        SensorType GetSensorType();

        string GetDisplayName();

        string GetSystemName();
    }
}
