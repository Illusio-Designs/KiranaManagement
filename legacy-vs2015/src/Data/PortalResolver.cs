using System.Web;

namespace Kirana.WebApi.Data
{
    // Which portal a request belongs to, decided by the sub-domain:
    //   admin.kirana.com  -> Admin      (Super Admin)
    //   store.kirana.com  -> Store      (store owners / managers, + Google)
    //   www / apex        -> Consumer   (public storefront, OTP + email)
    public enum Portal { Consumer = 0, Store = 1, Admin = 2 }

    public static class PortalResolver
    {
        public static Portal Current(HttpRequest request)
        {
            if (request == null || request.Url == null) return Portal.Consumer;
            // ?portal=admin|store|consumer is a dev convenience on localhost.
            var over = request.QueryString != null ? request.QueryString["portal"] : null;
            return FromHost(request.Url.Host, over);
        }

        public static Portal FromHost(string host, string portalOverride)
        {
            if (!string.IsNullOrEmpty(portalOverride))
            {
                switch (portalOverride.Trim().ToLowerInvariant())
                {
                    case "admin": return Portal.Admin;
                    case "store": case "seller": return Portal.Store;
                    case "consumer": case "shop": case "public": case "www": return Portal.Consumer;
                }
            }

            host = (host ?? "").ToLowerInvariant();
            var first = host.Split('.')[0];
            if (first == "admin") return Portal.Admin;
            if (first == "store" || first == "stores" || first == "seller") return Portal.Store;
            return Portal.Consumer; // www, apex, localhost, 127.0.0.1
        }

        // Landing page for a portal's root ("/").
        public static string LandingPage(Portal p)
        {
            switch (p)
            {
                case Portal.Admin: return "admin.aspx";
                case Portal.Store: return "store.aspx";
                default: return "home.aspx";
            }
        }

        public static string Title(Portal p)
        {
            switch (p)
            {
                case Portal.Admin: return "Super Admin sign in";
                case Portal.Store: return "Store owner sign in";
                default: return "Sign in to Kirana";
            }
        }
    }
}
