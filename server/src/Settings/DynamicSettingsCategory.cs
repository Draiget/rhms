using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Settings
{
    /// <summary>
    /// Settings category that can be disabled thought json file
    /// </summary>
    [Serializable]
    public abstract class DynamicSettingsCategory : PersistentSettingsCategory
    {
        public bool Enabled = true;
    }
}
