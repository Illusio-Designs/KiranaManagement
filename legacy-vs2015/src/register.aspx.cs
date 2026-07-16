using System;
using System.Web.UI.WebControls;
using Kirana.WebApi.Data;
using Newtonsoft.Json.Linq;

namespace Kirana.WebApi
{
    public partial class register : System.Web.UI.Page
    {
        protected TextBox txtName, txtOwner, txtEmail, txtPhone, txtPass, txtCity, txtGstin, txtPan;
        protected Button btnReg;
        protected Literal litMsg;

        protected void btnReg_Click(object sender, EventArgs e)
        {
            try
            {
                var body = new JObject();
                body["storeName"] = txtName.Text.Trim();
                body["ownerName"] = txtOwner.Text.Trim();
                body["email"] = txtEmail.Text.Trim();
                body["phone"] = txtPhone.Text.Trim();
                body["password"] = txtPass.Text;
                body["city"] = txtCity.Text.Trim();
                body["gstin"] = txtGstin.Text.Trim();
                body["pan"] = txtPan.Text.Trim();

                var res = ApiClient.Post(Request, "/api/stores/register", body, null);
                litMsg.Text = "<div class=\"alert alert-ok\">Registered! Status: <strong>" +
                    Server.HtmlEncode((string)res["status"]) +
                    "</strong>. Please wait for admin approval, then <a href=\"login.aspx\">log in</a>.</div>";
            }
            catch (Exception ex)
            {
                litMsg.Text = "<div class=\"alert alert-err\">" + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }
    }
}
