using System;
using System.Web;

namespace Kirana.WebApi.Data
{
    // Routes the site root ("/") to the correct landing page for the sub-domain
    // it was requested on:
    //   admin.kirana.com/  -> admin.aspx
    //   store.kirana.com/  -> store.aspx
    //   www / apex     /   -> home.aspx
    //
    // Register in web.config:
    //   <system.webServer><modules>
    //     <add name="PortalRoutingModule" type="Kirana.WebApi.Data.PortalRoutingModule"/>
    //   </modules></system.webServer>
    public class PortalRoutingModule : IHttpModule
    {
        public void Init(HttpApplication app)
        {
            app.BeginRequest += OnBeginRequest;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var ctx = app.Context;
            var path = ctx.Request.Path ?? "/";

            // Only rewrite the bare root / default document; leave real pages,
            // the /api/* Web API routes and static assets untouched.
            var isRoot = path == "/"
                || path.EndsWith("/default.aspx", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith("/index.aspx", StringComparison.OrdinalIgnoreCase);
            if (!isRoot) return;

            var portal = PortalResolver.Current(ctx.Request);
            var landing = PortalResolver.LandingPage(portal);
            ctx.Response.Redirect("~/" + landing, false);
            app.CompleteRequest();
        }

        public void Dispose() { }
    }
}
