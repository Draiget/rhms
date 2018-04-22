using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server.Networking
{
    public class NetworkConnectionManager
    {
        private readonly Thread _networkThread;
        private readonly NetworkClient _networkClient;

        public NetworkConnectionManager() {
            _networkClient = new NetworkClient();
            _networkThread = new Thread(WorkerUpdateNetworkThread);
        }

        private void WorkerUpdateNetworkThread() {
            _networkClient.Initialize();
        }

        public void Initialize() {
            _networkThread.Name = "network_updater";
            _networkThread.Start();
        }

        public string GetSelfHostId() {
            return "dev-srv";
        }
    }
}
