using System;
using System.IO;
using System.Net;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;

namespace Kirana.WebApi.Data
{
    // Result of validating a Google ID token.
    public class GoogleUser
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public bool EmailVerified { get; set; }
    }

    // Verifies a Google Sign-In ID token WITHOUT any extra NuGet package by
    // calling Google's public tokeninfo endpoint. Course-friendly: works with a
    // plain .NET Framework 4.6.2 project (only Newtonsoft.Json is needed, which
    // the project already references).
    public static class GoogleTokenValidator
    {
        // Returns the verified Google user, or null if the token is invalid.
        public static GoogleUser Validate(string idToken)
        {
            if (string.IsNullOrWhiteSpace(idToken)) return null;

            try
            {
                var url = "https://oauth2.googleapis.com/tokeninfo?id_token="
                          + Uri.EscapeDataString(idToken);
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "GET";
                req.Timeout = 8000;

                using (var resp = (HttpWebResponse)req.GetResponse())
                using (var reader = new StreamReader(resp.GetResponseStream()))
                {
                    if (resp.StatusCode != HttpStatusCode.OK) return null;
                    var json = JObject.Parse(reader.ReadToEnd());

                    // If a client id is configured, enforce the audience.
                    var clientId = WebConfigurationManager.AppSettings["GoogleClientId"];
                    if (!string.IsNullOrWhiteSpace(clientId))
                    {
                        var aud = (string)json["aud"];
                        if (!string.Equals(aud, clientId, StringComparison.Ordinal)) return null;
                    }

                    var email = (string)json["email"];
                    if (string.IsNullOrWhiteSpace(email)) return null;

                    var verifiedRaw = (string)json["email_verified"];
                    var verified = string.Equals(verifiedRaw, "true", StringComparison.OrdinalIgnoreCase);

                    return new GoogleUser
                    {
                        Email = email.Trim().ToLowerInvariant(),
                        Name = (string)json["name"] ?? "",
                        EmailVerified = verified
                    };
                }
            }
            catch (WebException)
            {
                return null; // 4xx from tokeninfo == invalid token
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
