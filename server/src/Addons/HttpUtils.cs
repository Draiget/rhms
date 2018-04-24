using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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

        public static JObject SendGet(string host, string args, out HttpWebResponse outResponse) {
            var request = WebRequest.Create($"{host}?{args}");

            request.Method = "GET";
            request.ContentType = "application/json";
            request.ContentLength = 0;

            try {
                var response = request.GetResponse();
                outResponse = (HttpWebResponse) response;
                return Parse(response);
            } catch (WebException webex) {
                outResponse = (HttpWebResponse)webex.Response;
                return Parse(webex.Response);
            }
        }

        private static JObject Parse(WebResponse response) {
            using (var responseStream = ((HttpWebResponse)response).GetResponseStream()) {
                if (responseStream == null) {
                    return null;
                }

                using (var sr = new StreamReader(responseStream)) {
                    var raw = sr.ReadToEnd();
                    return JObject.Parse(raw);
                }
            }
        }
    }
}
