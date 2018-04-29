using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace server.Settings
{
    /// <summary>
    /// Settings category that always enabled and visible to usage object
    /// </summary>
    [Serializable]
    public abstract class PersistentSettingsCategory
    {
        public virtual string GetDescription() {
            return null;
        }

        [JsonProperty]
        public string Description => GetDescription();
    }
}
