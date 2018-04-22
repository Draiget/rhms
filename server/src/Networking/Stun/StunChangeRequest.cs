using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Networking.Stun
{
    public class StunChangeRequest
    {
        public StunChangeRequest() { }

        public StunChangeRequest(bool changeIp, bool changePort) {
            ChangeIp = changeIp;
            ChangePort = changePort;
        }

        public bool ChangeIp {
            get;
            set;
        } = true;

        /// <summary>
        /// Gets or sets if STUN server must send response to different port than request was received.
        /// </summary>
        public bool ChangePort {
            get;
            set;
        } = true;
    }
}
