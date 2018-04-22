using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware;

namespace server.Modules.Base
{
    public abstract class BaseHardwareModule : BaseModule, IHardwareModule
    {
        protected BaseHardwareModule(BaseModuleLoader loader)
            : base(loader) {
        }

        /// <summary>
        /// Method is calling after all hardware are updated and information collected.
        /// May be used to store sensors and hardware data to database, or use it with custom influx API handlers
        /// </summary>
        public virtual void AfterHardwareTick() { }

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
        /// <param name="hardware"></param>
        public void AddHardware(IHardware hardware) {
            Hardware.Add(hardware);
        }

        public override string ToString() {
            return $"BaseHardwareModule[logId='{GetLogIdentifer()}', name='{GetName()}']";
        }
    }
}
