using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using Kirana.WebApi.Data;
using Newtonsoft.Json.Linq;

namespace Kirana.WebApi
{
    public class CatalogImageRow
    {
        public string Name { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
    }

    public partial class adminimages : System.Web.UI.Page
    {
        protected TextBox txtName;
        protected TextBox txtUrl;
        protected FileUpload fileImg;
        protected Button btnSave;
        protected Button btnRefresh;
        protected Literal litMsg;
        protected Literal litEmpty;
        protected Repeater rpt;

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
                var arr = ApiClient.Get(Request, "/api/catalog/images", Token()) as JArray;
                var rows = new List<CatalogImageRow>();
                if (arr != null)
                    foreach (var c in arr)
                        rows.Add(new CatalogImageRow
                        {
                            Name = (string)c["name"],
                            ImageName = (string)c["imageName"],
                            ImageUrl = (string)c["imageUrl"]
                        });
                rpt.DataSource = rows;
                rpt.DataBind();
                litEmpty.Text = rows.Count == 0 ? "<p class=\"muted\">No shared images yet.</p>" : "";
            }
            catch (Exception ex)
            {
                litEmpty.Text = "<div class=\"alert alert-err\">" + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e) { Bind(); }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var name = (txtName.Text ?? "").Trim();
            if (name.Length == 0) { litMsg.Text = Err("Enter the product name."); return; }

            try
            {
                string url = (txtUrl.Text ?? "").Trim();

                // If a file was chosen, save it under ~/uploads/products and use its URL.
                if (fileImg.HasFile)
                {
                    var ext = Path.GetExtension(fileImg.FileName).ToLowerInvariant();
                    if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif" && ext != ".webp")
                    { litMsg.Text = Err("Only image files (jpg, png, gif, webp) are allowed."); return; }

                    var dir = Server.MapPath("~/uploads/products");
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    var fileName = Guid.NewGuid().ToString("N") + ext;
                    fileImg.SaveAs(Path.Combine(dir, fileName));
                    url = "/uploads/products/" + fileName;
                }

                if (string.IsNullOrEmpty(url)) { litMsg.Text = Err("Upload a file or paste an image URL."); return; }

                var body = new JObject();
                body["name"] = name;
                body["imageUrl"] = url;
                ApiClient.Put(Request, "/api/catalog/image", body, Token());

                litMsg.Text = "<div class=\"alert alert-ok\">Saved. \"" + Server.HtmlEncode(name)
                    + "\" now uses this image everywhere it's sold.</div>";
                txtName.Text = ""; txtUrl.Text = "";
                Bind();
            }
            catch (Exception ex)
            {
                litMsg.Text = Err(ex.Message);
            }
        }

        private string Err(string m)
        {
            return "<div class=\"alert alert-err\">" + Server.HtmlEncode(m) + "</div>";
        }
    }
}
