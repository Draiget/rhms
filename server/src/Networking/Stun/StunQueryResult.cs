using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace server.Networking.Stun
{
    public class StunQueryResult
    {
        public StunQueryResult(StunNetworkType type, IPEndPoint publicEp = null) {
            NetworkType = type;
            PublicEndPoint = publicEp;
        }

        public StunNetworkType NetworkType {
            get;
        }

        public IPEndPoint PublicEndPoint {
            get;
        }
    }
}
