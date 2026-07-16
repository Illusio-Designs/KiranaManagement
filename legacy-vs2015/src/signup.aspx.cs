using System;
using System.Web.UI.WebControls;
using Kirana.WebApi.Data;
using Newtonsoft.Json.Linq;

namespace Kirana.WebApi
{
    public partial class signup : System.Web.UI.Page
    {
        protected TextBox txtName, txtEmail, txtPass;
        protected Button btnSignup;
        protected Literal litMsg;

        protected void btnSignup_Click(object sender, EventArgs e)
        {
            bool ok = false;
            try
            {
                var reg = new JObject();
                reg["fullName"] = txtName.Text.Trim();
                reg["email"] = txtEmail.Text.Trim();
                reg["password"] = txtPass.Text;
                ApiClient.Post(Request, "/api/auth/register-customer", reg, null);

                // auto-login
                var login = new JObject();
                login["email"] = txtEmail.Text.Trim();
                login["password"] = txtPass.Text;
                var res = ApiClient.Post(Request, "/api/auth/login", login, null);
                Session["token"] = (string)res["token"];
                Session["role"] = (string)res["role"];
                Session["name"] = (string)res["fullName"];
                ok = true;
            }
            catch (Exception ex)
            {
                litMsg.Text = "<div class=\"alert alert-err\">" + Server.HtmlEncode(ex.Message) + "</div>";
            }
            if (ok) Response.Redirect("shop.aspx");
        }
    }
}
