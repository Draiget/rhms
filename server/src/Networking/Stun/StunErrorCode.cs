using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Networking.Stun
{
    public class StunErrorCode
    {
        public StunErrorCode(int code, string reasonText) {
            Code = code;
            ReasonText = reasonText;
        }

        /// <summary>
        /// Gets or sets error code.
        /// </summary>
        public int Code {
            get;
            set;
        }

        /// <summary>
        /// Gets reason text.
        /// </summary>
        public string ReasonText {
            get;
            set;
        }
    }
}
