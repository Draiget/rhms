using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace server.Settings
{
    [Serializable]
    public abstract class BaseSettings
    {
        [NonSerialized]
        public BaseSettings Default;

        [JsonIgnore]
        public string FileName {
            get;
            protected set;
        }

        public virtual void Save() {
            SettingsManager.UpdateOrCreate(this);
        }
    }
}
