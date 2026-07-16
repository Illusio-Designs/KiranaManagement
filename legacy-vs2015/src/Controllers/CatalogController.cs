using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Kirana.WebApi.Data;
using Kirana.WebApi.Dtos;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Controllers
{
    // Shared master catalog (images reused across stores by product name).
    [RoutePrefix("api/catalog")]
    public class CatalogController : ApiController
    {
        // GET /api/catalog/image?name=Fresh%20Apples
        // Master image for a product name, if any — lets the store form auto-fill.
        [HttpGet, Route("image")]
        public IHttpActionResult Image(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return Ok((CatalogImageDto)null);

            var key = CatalogImage.Key(name);
            using (var db = new KiranaDbContext())
            {
                var m = db.CatalogImages.FirstOrDefault(c => c.NameKey == key);
                if (m == null) return Ok((CatalogImageDto)null);
                return Ok(new CatalogImageDto { Name = m.Name, ImageName = m.ImageName, ImageUrl = m.ImageUrl });
            }
        }

        // GET /api/catalog/images   (SuperAdmin) — all shared master images.
        [HttpGet, Route("images")]
        public IHttpActionResult Images()
        {
            var guard = RequireSuperAdmin();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var list = db.CatalogImages.OrderBy(c => c.Name)
                    .Select(c => new CatalogImageDto { Name = c.Name, ImageName = c.ImageName, ImageUrl = c.ImageUrl })
                    .ToList();
                return Ok(list);
            }
        }

        // PUT /api/catalog/image   (SuperAdmin) — add or REPLACE a shared image.
        // The new image also propagates to every product of that name that was
        // using the shared image (own custom images are left untouched).
        [HttpPut, Route("image")]
        public IHttpActionResult Upsert(UpsertCatalogImageRequest req)
        {
            var guard = RequireSuperAdmin();
            if (guard != null) return guard;
            if (req == null || string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.ImageUrl))
                return BadRequest("Name and image are required.");

            var key = CatalogImage.Key(req.Name);
            using (var db = new KiranaDbContext())
            {
                var m = db.CatalogImages.FirstOrDefault(c => c.NameKey == key);
                string oldUrl = m != null ? m.ImageUrl : null;

                if (m == null)
                {
                    m = new CatalogImage
                    {
                        NameKey = key,
                        Name = req.Name.Trim(),
                        ImageName = CatalogImage.DeriveImageName(req.Name),
                        ImageUrl = req.ImageUrl.Trim()
                    };
                    db.CatalogImages.Add(m);
                }
                else
                {
                    m.Name = req.Name.Trim();
                    m.ImageUrl = req.ImageUrl.Trim();
                }

                // Propagate the replacement to products that were using the shared image.
                var products = db.Products.ToList().Where(p => CatalogImage.Key(p.Name) == key).ToList();
                foreach (var p in products)
                    if (string.IsNullOrEmpty(p.ImageUrl) || p.ImageUrl == oldUrl)
                        p.ImageUrl = m.ImageUrl;

                db.SaveChanges();
                return Ok(new CatalogImageDto { Name = m.Name, ImageName = m.ImageName, ImageUrl = m.ImageUrl });
            }
        }

        private IHttpActionResult RequireSuperAdmin()
        {
            var user = AuthUtil.GetCurrentUser(Request);
            if (user == null || user.Role != UserRole.SuperAdmin)
                return Content(HttpStatusCode.Unauthorized, new { error = "Super admin login required." });
            return null;
        }
    }
}
