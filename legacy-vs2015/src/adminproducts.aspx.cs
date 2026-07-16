using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Kirana.WebApi.Data;
using Newtonsoft.Json.Linq;

namespace Kirana.WebApi
{
    public class PendingProductRow
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string StoreName { get; set; }
        public int Variants { get; set; }
        public string PriceRange { get; set; }
    }

    public partial class adminproducts : System.Web.UI.Page
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
                var arr = ApiClient.Get(Request, "/api/products/pending-approval", Token()) as JArray;
                var rows = new List<PendingProductRow>();
                if (arr != null)
                {
                    foreach (var p in arr)
                    {
                        var variants = p["variants"] as JArray;
                        var prices = new List<decimal>();
                        if (variants != null)
                            foreach (var v in variants)
                            {
                                decimal sp;
                                if (decimal.TryParse((string)v["sellingPrice"], NumberStyles.Any, CultureInfo.InvariantCulture, out sp))
                                    prices.Add(sp);
                            }
                        string range = "-";
                        if (prices.Count > 0)
                            range = prices.Min() == prices.Max()
                                ? "₹" + prices.Min().ToString("0.##")
                                : "₹" + prices.Min().ToString("0.##") + " - ₹" + prices.Max().ToString("0.##");

                        rows.Add(new PendingProductRow
                        {
                            Id = (string)p["id"],
                            Name = (string)p["name"],
                            Brand = (string)p["brand"],
                            Category = (string)p["category"],
                            StoreName = (string)p["storeName"],
                            Variants = variants != null ? variants.Count : 0,
                            PriceRange = range
                        });
                    }
                }
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
                    ApiClient.Post(Request, "/api/products/" + id + "/approve", null, Token());
                    litMsg.Text = "<div class=\"alert alert-ok\">Product approved — it is now live on the marketplace.</div>";
                }
                else if (e.CommandName == "Reject")
                {
                    var b = new JObject(); b["reason"] = "Rejected by admin";
                    ApiClient.Post(Request, "/api/products/" + id + "/reject", b, Token());
                    litMsg.Text = "<div class=\"alert alert-ok\">Product rejected.</div>";
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
