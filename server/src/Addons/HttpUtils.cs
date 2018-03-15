using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace server.Addons
{
    public static class HttpUtils
    {
        public static HttpStatusCode SendPost(string host, string rawData) {
            var bytes = Encoding.UTF8.GetBytes(rawData);
            var request = WebRequest.Create(host);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = bytes.Length;

            var stream = request.GetRequestStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();

            var response = request.GetResponse();
            return ((HttpWebResponse) response).StatusCode;
        }
    }
}
