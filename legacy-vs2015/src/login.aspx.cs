using System;
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
        protected Literal litMsg;

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string target = null;
            try
            {
                var body = new JObject();
                body["email"] = txtEmail.Text.Trim();
                body["password"] = txtPass.Text;
                var res = ApiClient.Post(Request, "/api/auth/login", body, null);

                Session["token"] = (string)res["token"];
                Session["role"] = (string)res["role"];
                Session["name"] = (string)res["fullName"];
                Session["storeId"] = (string)res["storeId"];

                var role = (string)res["role"];
                if (role == "SuperAdmin") target = "admin.aspx";
                else if (role == "Customer") target = "shop.aspx";
                else target = "store.aspx";
            }
            catch (Exception ex)
            {
                litMsg.Text = "<div class=\"alert alert-err\">" + Server.HtmlEncode(ex.Message) + "</div>";
            }
            if (target != null) Response.Redirect(target);
        }
    }
}
