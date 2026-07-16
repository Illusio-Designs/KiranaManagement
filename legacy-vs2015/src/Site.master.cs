using System;
using System.Web.UI;

namespace Kirana.WebApi
{
    public partial class SiteMaster : MasterPage
    {
        protected global::System.Web.UI.WebControls.Literal litAccount;

        protected void Page_Load(object sender, EventArgs e)
        {
            var role = Session["role"] as string;
            if (role == "Customer")
                litAccount.Text = "<a class=\"navlink\" href=\"orders.aspx\">My orders</a> " +
                    "<a class=\"navlink\" href=\"logout.aspx\">Hi " + Server.HtmlEncode((Session["name"] as string) ?? "you") + " · Logout</a>";
            else
                litAccount.Text = "<a class=\"navlink\" href=\"login.aspx\">Login</a> <a class=\"navlink\" href=\"signup.aspx\">Sign up</a>";
        }
    }
}
