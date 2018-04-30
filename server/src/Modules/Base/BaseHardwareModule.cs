using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Addons;
using server.Hardware;

namespace server.Modules.Base
{
    public abstract class BaseHardwareModule : BaseModule, IHardwareModule
    {
        protected BaseHardwareModule(BaseModuleLoader loader)
            : base(loader) {
        }

        /// <summary>
        /// <p>Method is calling after all modules are updated when specific schedule time is spent (see interval in config <see cref="RhmsSettings.ShceduledModuleUpdateInterval"/>)</p>
        /// <p>May be used to store sensors and hardware data to database, or use it with custom influx API handlers</p>
        /// </summary>
        public virtual void ScheduledModuleUpdateTick() { }

        /// <summary>
        /// Specific hardware initialization stage, can be overriden and using for loading external API or calling exporing functions
        /// </summary>
        /// <returns>State if all hardware that supports this module are loaded well</returns>
        public abstract bool InitializeHardware();

        /// <summary>
        /// Retreive all hardware that this module can support and operate with
        /// </summary>
        /// <returns>List of hardware interfaces</returns>
        public List<IHardware> GetHardware() {
            return Hardware;
        }

        /// <summary>
        /// <p>Adds hardware that can be supported with this specific module.</p>
        /// <p>Hardware can't be removed from this list, use <see cref="InitializeHardware"/> to prevent module loading</p>
        /// </summary>
        /// <param name="hardware">Reference on adding hardware</param>
        public void AddHardware(IHardware hardware) {
            Hardware.Add(hardware);
        }

        public override string ToString() {
            return $"BaseHardwareModule[logId='{GetLogIdentifer()}', name='{GetName()}']";
        }

        /// <summary>
        /// <p>Method is calling after all hardware are updated and information collected.</p>
        /// <p>Notice: Calling every TICK (see interval as config value <see cref="RhmsSettings.HardwareSensorsUpdateInterval"/>)</p>
        /// </summary>
        public virtual void PostHardwareTick(BaseCollectingServer server) { }

        public void ExportDataToGrafana(BaseCollectingServer server) {
            if (!server.GetSettings().InfluxOutput.Enabled) {
                return;
            }

            var influxDb = server.GetInfluxDbConnection();
            foreach (var hardware in Hardware) {
                foreach (var sensor in hardware.GetSensors()) {
                    if (!sensor.IsAvaliable()) {
                        continue;
                    }

                    var influxData = new List<InfluxDataPair> {
                        new InfluxDataTag("vendor", hardware.Identify().GetVendor()),
                        new InfluxDataTag("hw_id", hardware.Identify().GetHardwareId()),
                        new InfluxDataTag("sensor", sensor.GetSystemName()),
                        new InfluxDataTag("sensor_type", sensor.GetSensorType().ToString().ToLower())
                    };

                    if (sensor is IMultiValueSensor multi) {
                        foreach (var sensorElement in multi.GetElements()) {
                            influxData.Add(new InfluxDataPair(sensorElement.GetSystemTag(), $"{sensorElement.GetValue():F4}"));
                        }
                    }

                    if (sensor is ISingleValueSensor single) {
                        var sensorElement = single.GetElement();
                        influxData.Add(new InfluxDataPair(sensorElement.GetSystemTag(), $"{sensorElement.GetValue():F4}"));
                    }

                    influxDb.Write(new InfluxWriteData(hardware.Identify().GetHardwareType().ToString().ToLower(), influxData.ToArray()));
                }
            }
        }
    }
}
