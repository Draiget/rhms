using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules;
using server.Modules.Base;
using server.Modules.Extended;
using server.Utils.Logging;

namespace server
{
    public class RhmsCollectingServer : BaseCollectingServer
    {
        private readonly BaseModuleLoader _moduleLoader;

        public RhmsCollectingServer(){
            _moduleLoader = new ServerModuleLoader(this);
        }

        public override BaseModuleLoader GetModuleLoader(){
            return _moduleLoader;
        }
    }
}
