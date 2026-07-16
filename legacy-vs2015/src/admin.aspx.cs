using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Kirana.WebApi.Data;
using Newtonsoft.Json.Linq;

namespace Kirana.WebApi
{
    public class StoreRow
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Gstin { get; set; }
    }

    public partial class admin : System.Web.UI.Page
    {
        protected GridView grid;
        protected Literal litMsg;

        private string Token() { return Session["token"] as string; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((Session["role"] as string) != "SuperAdmin") { Response.Redirect("login.aspx"); return; }
            if (!IsPostBack) Bind();
        }

        private void Bind()
        {
            try
            {
                var arr = ApiClient.Get(Request, "/api/stores/pending", Token()) as JArray;
                var rows = new List<StoreRow>();
                if (arr != null)
                    foreach (var s in arr)
                        rows.Add(new StoreRow
                        {
                            Id = (string)s["id"],
                            Name = (string)s["name"],
                            OwnerName = (string)s["ownerName"],
                            Email = (string)s["email"],
                            City = (string)s["city"],
                            Gstin = (string)s["gstin"]
                        });
                grid.DataSource = rows;
                grid.DataBind();
            }
            catch (Exception ex)
            {
                litMsg.Text = "<div class=\"alert alert-err\">" + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var id = (string)e.CommandArgument;
            try
            {
                if (e.CommandName == "Approve")
                {
                    ApiClient.Post(Request, "/api/stores/" + id + "/approve", null, Token());
                    litMsg.Text = "<div class=\"alert alert-ok\">Store approved.</div>";
                }
                else if (e.CommandName == "Reject")
                {
                    var b = new JObject(); b["reason"] = "Rejected by admin";
                    ApiClient.Post(Request, "/api/stores/" + id + "/reject", b, Token());
                    litMsg.Text = "<div class=\"alert alert-ok\">Store rejected.</div>";
                }
                Bind();
            }
            catch (Exception ex)
            {
                litMsg.Text = "<div class=\"alert alert-err\">" + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }
    }
}
