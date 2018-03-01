using System;
using server.Modules.Base;

namespace server.Modules.Extended
{
    public class ServerModuleLoader : BaseModuleLoader
    {
        public ServerModuleLoader(BaseCollectingServer server)
            : base(server){
        }

        public override bool UnloadModule(BaseModule module) {
            throw new NotImplementedException();
        }

        public override void UnloadAllModules(){
            throw new NotImplementedException();
        }
    }
}
