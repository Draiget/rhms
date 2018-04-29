using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    [Serializable]
    public class RhmsSettings
    {
        public static RhmsSettings Default => new RhmsSettings {
            BindAddress = "0.0.0.0",
            InfluxOutput = new SectionInfluxDb("http://127.0.0.1:8086", "rhms") { Enabled = false },
            HardwareSensorsUpdateInterval = 500,
            PeerSignalServer = new RequiredSectionPeerSignalServer("http://srv-ps.rhms.zontwelg.com/", 4000),
            CollectingServerPeerName = "srv-dc1.cp-rig1",
            StunServers = new [] {
                new StunServerSettings("stun.l.google.com", 19302), 
                new StunServerSettings("stun1.l.google.com", 19302), 
                new StunServerSettings("stun2.l.google.com", 19302), 
                new StunServerSettings("stun3.l.google.com", 19302), 
                new StunServerSettings("stun4.l.google.com", 19302), 
                new StunServerSettings("stunserver.org"), 
                new StunServerSettings("stun.softjoys.com"), 
                new StunServerSettings("stun.xten.com"), 
                new StunServerSettings("stun.rixtelecom.se"),
            },
            ApiAccessKey = "rhms-dc-zontwelg-shared"
        };

        public string ApiAccessKey;
        public string CollectingServerPeerName;
        public string BindAddress;
        public SectionInfluxDb InfluxOutput;
        public int HardwareSensorsUpdateInterval;
        public RequiredSectionPeerSignalServer PeerSignalServer;
        public StunServerSettings[] StunServers;

        public abstract class Section
        {
            public bool Enabled;
        }

        [Serializable]
        public class SectionInfluxDb : Section
        {
            public string Host;
            public string Database;
            public string Retention = "two_months";

            public SectionInfluxDb(string influxDbHost, string influxDbName){
                Host = influxDbHost;
                Database = influxDbName;
            }
        }

        public class RequiredSectionPeerSignalServer
        {
            public string Host;
            public int SocketTimeoutMs;

            public RequiredSectionPeerSignalServer(string host, int timeoutMs) {
                Host = host;
                SocketTimeoutMs = timeoutMs;
            }
        }

        public class StunServerSettings
        {
            public string Host;
            public int Port;

            public StunServerSettings(string host, int port = 3478) {
                Host = host;
                Port = port;
            }
        }
    }
}
