using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Settings
{
    public struct SettingsIoResult
    {
        public bool IsOk;
        public string ErrorMessage;
        public Exception Error;

        public bool HasNoErrors() {
            return IsOk;
        }
    }
}
