using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace Kirana.WebApi.Data
{
    // Server-side helper: code-behind uses this to call the Web API (same site)
    // and get back parsed JSON. Errors surface the API's { "error": "..." } message.
    public static class ApiClient
    {
        private static string BaseUrl(HttpRequest req)
        {
            return req.Url.GetLeftPart(UriPartial.Authority);
        }

        public static JToken Get(HttpRequest req, string path, string token)
        {
            return Send(req, "GET", path, null, token);
        }

        public static JToken Post(HttpRequest req, string path, JObject body, string token)
        {
            return Send(req, "POST", path, body == null ? "{}" : body.ToString(), token);
        }

        private static JToken Send(HttpRequest req, string method, string path, string jsonBody, string token)
        {
            var http = (HttpWebRequest)WebRequest.Create(BaseUrl(req) + path);
            http.Method = method;
            http.ContentType = "application/json";
            http.Accept = "application/json";
            if (!string.IsNullOrEmpty(token)) http.Headers["X-Auth-Token"] = token;

            if (jsonBody != null && method != "GET")
            {
                var bytes = Encoding.UTF8.GetBytes(jsonBody);
                http.ContentLength = bytes.Length;
                using (var s = http.GetRequestStream()) s.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (var resp = (HttpWebResponse)http.GetResponse())
                using (var rd = new StreamReader(resp.GetResponseStream()))
                {
                    var body = rd.ReadToEnd();
                    return string.IsNullOrEmpty(body) ? null : JToken.Parse(body);
                }
            }
            catch (WebException ex)
            {
                var msg = "Request failed.";
                if (ex.Response != null)
                {
                    using (var rd = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        var body = rd.ReadToEnd();
                        try { var j = JToken.Parse(body); if (j["error"] != null) msg = j["error"].ToString(); }
                        catch { if (!string.IsNullOrEmpty(body)) msg = body; }
                    }
                }
                throw new Exception(msg);
            }
        }
    }
}
