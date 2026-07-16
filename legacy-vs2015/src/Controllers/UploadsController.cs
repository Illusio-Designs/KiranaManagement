using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using Kirana.WebApi.Data;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Controllers
{
    // Real file uploads. Saves images under ~/uploads/products and returns the
    // public URL, which is then stored on the product / master catalog image.
    [RoutePrefix("api/uploads")]
    public class UploadsController : ApiController
    {
        private static readonly string[] Allowed = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        // POST /api/uploads/image   (store owner / manager / super admin)
        // multipart/form-data with a single file field.
        [HttpPost, Route("image")]
        public async Task<IHttpActionResult> Image()
        {
            var user = AuthUtil.GetCurrentUser(Request);
            if (user == null ||
                (user.Role != UserRole.Owner && user.Role != UserRole.Manager && user.Role != UserRole.SuperAdmin))
                return Content(HttpStatusCode.Unauthorized, new { error = "Login required to upload." });

            if (!Request.Content.IsMimeMultipartContent())
                return BadRequest("Expected multipart/form-data.");

            var dir = HostingEnvironment.MapPath("~/uploads/products");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var provider = new MultipartFormDataStreamProvider(dir);
            await Request.Content.ReadAsMultipartAsync(provider);

            var file = provider.FileData.FirstOrDefault();
            if (file == null) return BadRequest("No file uploaded.");

            var original = (file.Headers.ContentDisposition.FileName ?? "image").Trim('"');
            var ext = Path.GetExtension(original).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext)) ext = ".jpg";
            if (!Allowed.Contains(ext))
            {
                File.Delete(file.LocalFileName);
                return BadRequest("Only image files (jpg, png, gif, webp) are allowed.");
            }

            var name = Guid.NewGuid().ToString("N") + ext;
            var finalPath = Path.Combine(dir, name);
            if (File.Exists(finalPath)) File.Delete(finalPath);
            File.Move(file.LocalFileName, finalPath);

            return Ok(new { url = "/uploads/products/" + name });
        }
    }
}
