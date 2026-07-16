using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Kirana.WebApi
{
    public partial class DashboardMaster : MasterPage
    {
        protected HtmlGenericControl side;
        protected Literal litUser;
        protected Literal litInitial;
        protected LinkButton btnLogout;

        protected void Page_Load(object sender, EventArgs e)
        {
            var role = Session["role"] as string;
            if (string.IsNullOrEmpty(role)) { Response.Redirect("login.aspx"); return; }

            var name = (Session["name"] as string) ?? "";
            litUser.Text = Server.HtmlEncode(name);
            litInitial.Text = Server.HtmlEncode(name.Length > 0 ? name.Substring(0, 1).ToUpper() : "U");
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("login.aspx");
        }
    }
}
