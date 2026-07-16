using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Kirana.WebApi.Data;
using Kirana.WebApi.Dtos;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Controllers
{
    // Public consumer-facing catalog across all active stores.
    [RoutePrefix("api/marketplace")]
    public class MarketplaceController : ApiController
    {
        // GET /api/marketplace/products
        [HttpGet, Route("products")]
        public IHttpActionResult Products()
        {
            using (var db = new KiranaDbContext())
            {
                var activeStores = db.Stores
                    .Where(s => s.Status == StoreStatus.Active)
                    .Select(s => new { s.Id, s.Name })
                    .ToList();
                var storeIds = activeStores.Select(s => s.Id).ToList();
                var storeNames = activeStores.ToDictionary(s => s.Id, s => s.Name);

                var products = db.Products
                    .Include(p => p.Variants)
                    .Where(p => p.IsActive && storeIds.Contains(p.StoreId))
                    .OrderBy(p => p.Name)
                    .ToList();

                var result = new List<MarketProductDto>();
                foreach (var p in products)
                {
                    var dto = new MarketProductDto
                    {
                        Id = p.Id,
                        StoreId = p.StoreId,
                        StoreName = storeNames.ContainsKey(p.StoreId) ? storeNames[p.StoreId] : "",
                        Name = p.Name,
                        Brand = p.Brand
                    };
                    foreach (var v in p.Variants.Where(x => x.IsActive))
                    {
                        dto.Variants.Add(new MarketVariantDto
                        {
                            Id = v.Id,
                            Name = v.Name,
                            Mrp = v.Mrp,
                            SellingPrice = v.SellingPrice,
                            DiscountPercent = v.DiscountPercent,
                            StockQuantity = v.StockQuantity
                        });
                    }
                    if (dto.Variants.Count > 0) result.Add(dto);
                }
                return Ok(result);
            }
        }
    }
}
