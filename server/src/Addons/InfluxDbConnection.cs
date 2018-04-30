using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using server.Utils.Logging;

namespace server.Addons
{
    public class InfluxDbConnection
    {
        private readonly string _remoteHost;
        private readonly string _dbTableName;
        private readonly string _retention;
        private readonly bool _enableDebugOutput;
        private readonly RhmsCollectingServer _server;

        public InfluxDbConnection(RhmsSettings.SectionInfluxDb settings, RhmsCollectingServer server){
            _remoteHost = settings.Host;
            _dbTableName = settings.Database;
            _retention = settings.Retention;
            _enableDebugOutput = settings.EnableDebugOutput;
            _server = server;
        }

        public void CheckCreateDatabase(){
            CreateDatabase();
        }

        public void Write(InfluxWriteData data) {
            if (!data.IsValidQuery()) {
                return;
            }

            data.AddTags(new InfluxDataPair("host", _server.GetSettings().CollectingServerPeerName));

            InternalSendData(data.QueryRequest);
        }

        private void CreateDatabase(){
            HttpUtils.SendGet($"{_remoteHost}/query", $"q=CREATE DATABASE {_dbTableName}", out var resp);
            if (_enableDebugOutput) {
                Logger.Debug($"Create database response: {resp.StatusCode}");
            }
        }

        private void InternalSendData(string data){
            if (_enableDebugOutput) {
                Logger.Debug($"data: {data}");
            }

            HttpUtils.SendPost($"{_remoteHost}/write?db={_dbTableName}" + (string.IsNullOrEmpty(_retention) ? $"&tp={_retention}" : ""), data, out var resp);
            if (resp.StatusCode != HttpStatusCode.OK && _enableDebugOutput) {
                Logger.Debug($"Unable to write data: {resp.StatusCode}");
            }
        }
    }
}
