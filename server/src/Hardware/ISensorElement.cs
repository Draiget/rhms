using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Hardware
{
    public interface ISensorElement
    {
        double GetMax();

        double GetMin();

        double GetValue();

        string GetMeasurement();

        string GetSystemTag();

        bool IsActive();
    }
}
