using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Modules
{
    public interface IModuleLoader
    {
        void LoadFromFolder(string folderName);

        void UnloadAll();
    }
}
