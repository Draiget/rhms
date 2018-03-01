using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Modules
{
    public class ModuleLoadingException : Exception
    {
        public ModuleLoadingException(string msg, Exception inner = null)
            : base(msg, inner){
        }
    }
}
