using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Modules.Base;

namespace server.Modules
{
    public interface IModuleLoader
    {
        void LoadFromFolder(string folderName);

        BaseModule LoadFromFile(string folderName);

        bool UnloadModule(BaseModule module);

        void UnloadAllModules();

        BaseCollectingServer GetServer();
    }
}
