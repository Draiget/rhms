using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    [Serializable]
    public class RhmsSettings
    {
        public static RhmsSettings Default => new RhmsSettings {
            BindAddress = "0.0.0.0",
            InfluxOutput = new SectionInfluxDb("http://127.0.0.1:8086", "rhms") { Enabled = false },
            HardwareSensorsUpdateInterval = 500
        };

        public string BindAddress;
        public SectionInfluxDb InfluxOutput;
        public int HardwareSensorsUpdateInterval;

        public abstract class Section
        {
            public bool Enabled;
        }

        [Serializable]
        public class SectionInfluxDb : Section
        {
            public string Host;
            public string Database;
            public string Retention = "two_months";

            public SectionInfluxDb(string influxDbHost, string influxDbName){
                Host = influxDbHost;
                Database = influxDbName;
            }
        }
    }
}
