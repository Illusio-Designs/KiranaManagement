using System.Linq;
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
        // Returns the master image for a product name, if one exists — lets the
        // store product form auto-fill / preview a reusable image.
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
    }
}
