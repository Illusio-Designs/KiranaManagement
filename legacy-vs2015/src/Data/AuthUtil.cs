using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Data
{
    // Reads the X-Auth-Token header and resolves the logged-in user.
    public static class AuthUtil
    {
        public const string TokenHeader = "X-Auth-Token";

        public static User GetCurrentUser(HttpRequestMessage request)
        {
            if (request == null) return null;

            IEnumerable<string> values;
            if (!request.Headers.TryGetValues(TokenHeader, out values))
                return null;

            var token = values.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(token))
                return null;

            using (var db = new KiranaDbContext())
            {
                return db.Users.FirstOrDefault(u => u.SessionToken == token && u.IsActive);
            }
        }
    }
}
