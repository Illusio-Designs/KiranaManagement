using System;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using Kirana.WebApi.Data;
using Newtonsoft.Json.Linq;

namespace Kirana.WebApi
{
    public partial class login : System.Web.UI.Page
    {
        protected TextBox txtEmail;
        protected TextBox txtPass;
        protected Button btnLogin;
        protected Button btnEstablish;
        protected HiddenField hidToken;
        protected Literal litMsg;
        protected Literal litTitle;
        protected Literal litSub;

        // Portal is decided by the sub-domain (admin. / store. / www).
        protected Portal CurrentPortal { get { return PortalResolver.Current(Request); } }

        protected string GoogleClientId
        {
            get { return WebConfigurationManager.AppSettings["GoogleClientId"] ?? ""; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            litTitle.Text = PortalResolver.Title(CurrentPortal);
            switch (CurrentPortal)
            {
                case Portal.Admin: litSub.Text = "Platform administration portal"; break;
                case Portal.Store: litSub.Text = "Manage your store on Kirana"; break;
                default: litSub.Text = "Shop fresh groceries near you"; break;
            }
        }

        // Email + password sign-in (all portals).
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string target = null;
            try
            {
                var body = new JObject();
                body["email"] = txtEmail.Text.Trim();
                body["password"] = txtPass.Text;
                var res = ApiClient.Post(Request, "/api/auth/login", body, null);
                target = Establish((string)res["token"], (string)res["role"],
                                    (string)res["fullName"], (string)res["storeId"]);
            }
            catch (Exception ex)
            {
                litMsg.Text = Err(ex.Message);
            }
            if (target != null) Response.Redirect(target);
        }

        // Called by JS after Google / OTP returns a token; re-validate it
        // server-side (via /api/auth/me) before trusting it and setting Session.
        protected void btnEstablish_Click(object sender, EventArgs e)
        {
            string target = null;
            try
            {
                var token = hidToken.Value;
                if (string.IsNullOrWhiteSpace(token)) { litMsg.Text = Err("Sign-in was not completed."); return; }
                var me = ApiClient.Get(Request, "/api/auth/me", token);
                target = Establish((string)me["token"], (string)me["role"],
                                   (string)me["fullName"], (string)me["storeId"]);
            }
            catch (Exception ex) { litMsg.Text = Err(ex.Message); }
            if (target != null) Response.Redirect(target);
        }

        private string Establish(string token, string role, string name, string storeId)
        {
            if (!RoleAllowedForPortal(role))
            {
                litMsg.Text = Err(WrongPortalMessage());
                return null;
            }
            Session["token"] = token;
            Session["role"] = role;
            Session["name"] = name;
            Session["storeId"] = storeId;

            if (role == "SuperAdmin") return "admin.aspx";
            if (role == "Customer") return "shop.aspx";
            return "store.aspx";
        }

        // Each sub-domain only admits its own kind of user.
        private bool RoleAllowedForPortal(string role)
        {
            switch (CurrentPortal)
            {
                case Portal.Admin: return role == "SuperAdmin";
                case Portal.Store: return role == "Owner" || role == "Manager";
                default: return role == "Customer";
            }
        }

        private string WrongPortalMessage()
        {
            switch (CurrentPortal)
            {
                case Portal.Admin:
                    return "This is the admin portal. Please use the store or shopping site to sign in.";
                case Portal.Store:
                    return "This is the store portal. Customers shop on the main site; admins use the admin portal.";
                default:
                    return "This is the shopping site. Store owners sign in on the store portal; admins on the admin portal.";
            }
        }

        private string Err(string m)
        {
            return "<div class=\"alert alert-err\">" + Server.HtmlEncode(m) + "</div>";
        }
    }
}
