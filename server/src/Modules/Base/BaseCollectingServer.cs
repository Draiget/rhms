using System;

namespace server.Modules.Base
{
    public abstract class BaseCollectingServer : ICollectingServer
    {
        public bool HasActiveRemoteConnection(){
            throw new NotImplementedException();
        }

        public abstract BaseModuleLoader GetModuleLoader();
    }
}
