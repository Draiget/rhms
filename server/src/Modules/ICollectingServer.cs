using server.Hardware;
using server.Modules.Base;
using server.Networking;

namespace server.Modules
{
    public interface ICollectingServer
    {
        RhmsSettings GetSettings();

        bool HasActiveRemoteConnection();

        BaseModuleLoader GetModuleLoader();

        void Shutdown();

        void RegisterHardware(IHardware hardware);

        NetworkConnectionManager GetNetConnectionManager();
    }
}
