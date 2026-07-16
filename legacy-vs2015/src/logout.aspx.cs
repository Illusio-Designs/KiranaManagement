using System;

namespace Kirana.WebApi
{
    public partial class logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("home.aspx");
        }
    }
}
