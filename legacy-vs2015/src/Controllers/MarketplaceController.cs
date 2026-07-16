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
        // GET /api/marketplace/products?q=&category=&sort=
        //   q        - free-text search over name / brand / category
        //   category - exact category filter (e.g. "Fruits")
        //   sort     - "popular" (default) | "price_asc" | "price_desc" | "discount" | "name"
        [HttpGet, Route("products")]
        public IHttpActionResult Products(string q = null, string category = null, string sort = null)
        {
            using (var db = new KiranaDbContext())
            {
                var activeStores = db.Stores
                    .Where(s => s.Status == StoreStatus.Active)
                    .Select(s => new { s.Id, s.Name })
                    .ToList();
                var storeIds = activeStores.Select(s => s.Id).ToList();
                var storeNames = activeStores.ToDictionary(s => s.Id, s => s.Name);

                var query = db.Products
                    .Include(p => p.Variants)
                    .Where(p => p.IsActive && storeIds.Contains(p.StoreId));

                if (!string.IsNullOrWhiteSpace(category))
                {
                    var c = category.Trim();
                    query = query.Where(p => p.Category == c);
                }

                if (!string.IsNullOrWhiteSpace(q))
                {
                    var term = q.Trim();
                    query = query.Where(p =>
                        p.Name.Contains(term)
                        || (p.Brand != null && p.Brand.Contains(term))
                        || (p.Category != null && p.Category.Contains(term)));
                }

                var products = query.ToList();

                var result = new List<MarketProductDto>();
                foreach (var p in products)
                {
                    var dto = new MarketProductDto
                    {
                        Id = p.Id,
                        StoreId = p.StoreId,
                        StoreName = storeNames.ContainsKey(p.StoreId) ? storeNames[p.StoreId] : "",
                        Name = p.Name,
                        Brand = p.Brand,
                        Category = p.Category
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
                    if (dto.Variants.Count == 0) continue;

                    dto.MinPrice = dto.Variants.Min(x => x.SellingPrice);
                    dto.MaxDiscountPercent = dto.Variants.Max(x => x.DiscountPercent);
                    result.Add(dto);
                }

                // Apply sort in memory (works over the computed MinPrice / discount).
                switch ((sort ?? "").Trim().ToLowerInvariant())
                {
                    case "price_asc":
                        result = result.OrderBy(r => r.MinPrice).ToList();
                        break;
                    case "price_desc":
                        result = result.OrderByDescending(r => r.MinPrice).ToList();
                        break;
                    case "discount":
                        result = result.OrderByDescending(r => r.MaxDiscountPercent).ToList();
                        break;
                    case "name":
                    case "popular":
                    default:
                        result = result.OrderBy(r => r.Name).ToList();
                        break;
                }

                return Ok(result);
            }
        }

        // GET /api/marketplace/categories  -> distinct categories with product counts.
        [HttpGet, Route("categories")]
        public IHttpActionResult Categories()
        {
            using (var db = new KiranaDbContext())
            {
                var storeIds = db.Stores
                    .Where(s => s.Status == StoreStatus.Active)
                    .Select(s => s.Id)
                    .ToList();

                var cats = db.Products
                    .Where(p => p.IsActive && storeIds.Contains(p.StoreId)
                        && p.Category != null && p.Category != "")
                    .GroupBy(p => p.Category)
                    .Select(g => new { name = g.Key, count = g.Count() })
                    .OrderBy(x => x.name)
                    .ToList();

                return Ok(cats);
            }
        }
    }
}
