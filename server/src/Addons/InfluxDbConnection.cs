using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace server.Addons
{
    public class InfluxDbConnection
    {
        private readonly string _remoteHost;
        private readonly string _dbTableName;
        private readonly string _retention;

        public InfluxDbConnection(string remoteHost, string databaseTable, string retention = ""){
            _remoteHost = remoteHost;
            _dbTableName = databaseTable;
            _retention = retention;
        }

        public void CheckCreateDatabase(){
            CreateDatabase();
        }

        public void Write(InfluxWriteData data) {
            InternalSendData(data.QueryRequest);
        }

        private void CreateDatabase(){
            HttpUtils.SendPost($"{_remoteHost}/query", $"q=CREATE DATABASE {_dbTableName}");
        }

        private void InternalSendData(string data){
            HttpUtils.SendPost($"{_remoteHost}/write?db={_dbTableName}" + (string.IsNullOrEmpty(_retention) ? $"&tp={_retention}" : ""), data);
        }
    }
}
