using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using Kirana.WebApi.Data;
using Kirana.WebApi.Dtos;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : ApiController
    {
        // Returns the logged-in store owner/manager, or null (with a 401 in 'error').
        private User CurrentStoreUser(out IHttpActionResult error)
        {
            error = null;
            var user = AuthUtil.GetCurrentUser(Request);
            if (user == null || (user.Role != UserRole.Owner && user.Role != UserRole.Manager))
            {
                error = Content(HttpStatusCode.Unauthorized, new { error = "Store owner login required." });
                return null;
            }
            if (!user.StoreId.HasValue)
            {
                error = Content(HttpStatusCode.BadRequest, new { error = "This account is not linked to a store." });
                return null;
            }
            return user;
        }

        // POST /api/products  (owner/manager; uses their own store)
        [HttpPost, Route("")]
        public IHttpActionResult Create(CreateProductRequest req)
        {
            IHttpActionResult error;
            var user = CurrentStoreUser(out error);
            if (error != null) return error;

            if (req == null || string.IsNullOrWhiteSpace(req.Name))
                return BadRequest("Product name is required.");
            if (req.Variants == null || req.Variants.Count == 0)
                return BadRequest("A product needs at least one variant.");

            foreach (var v in req.Variants)
            {
                if (v.Mrp <= 0) return BadRequest("MRP must be greater than zero.");
                if (v.SellingPrice <= 0) return BadRequest("Selling price must be greater than zero.");
                if (v.SellingPrice > v.Mrp) return BadRequest("Selling price cannot exceed MRP.");
            }

            var storeId = user.StoreId.Value;

            using (var db = new KiranaDbContext())
            {
                var product = new Product
                {
                    StoreId = storeId,
                    Name = req.Name.Trim(),
                    Description = req.Description,
                    Brand = req.Brand
                };

                foreach (var v in req.Variants)
                {
                    product.Variants.Add(new ProductVariant
                    {
                        StoreId = storeId,
                        Name = (v.Name ?? "").Trim(),
                        Sku = v.Sku,
                        Barcode = v.Barcode,
                        Unit = v.Unit,
                        PackSize = v.PackSize <= 0 ? 1m : v.PackSize,
                        Mrp = v.Mrp,
                        SellingPrice = v.SellingPrice,
                        TaxRatePercent = v.TaxRatePercent,
                        StockQuantity = v.StockQuantity < 0 ? 0 : v.StockQuantity,
                        ReorderLevel = v.ReorderLevel < 0 ? 0 : v.ReorderLevel
                    });
                }

                db.Products.Add(product);
                db.SaveChanges();
                return Ok(ToDto(product));
            }
        }

        // GET /api/products  (owner/manager; lists their own store's products)
        [HttpGet, Route("")]
        public IHttpActionResult List()
        {
            IHttpActionResult error;
            var user = CurrentStoreUser(out error);
            if (error != null) return error;

            var storeId = user.StoreId.Value;

            using (var db = new KiranaDbContext())
            {
                var products = db.Products
                    .Include(p => p.Variants)
                    .Where(p => p.StoreId == storeId)
                    .OrderBy(p => p.Name)
                    .ToList();
                return Ok(products.Select(ToDto).ToList());
            }
        }

        private static ProductDto ToDto(Product p)
        {
            var dto = new ProductDto
            {
                Id = p.Id,
                StoreId = p.StoreId,
                Name = p.Name,
                Description = p.Description,
                Brand = p.Brand,
                IsActive = p.IsActive
            };

            var variants = p.Variants ?? new List<ProductVariant>();
            foreach (var v in variants)
            {
                dto.Variants.Add(new ProductVariantDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Sku = v.Sku,
                    Unit = v.Unit,
                    PackSize = v.PackSize,
                    Mrp = v.Mrp,
                    SellingPrice = v.SellingPrice,
                    DiscountAmount = v.DiscountAmount,
                    DiscountPercent = v.DiscountPercent,
                    TaxRatePercent = v.TaxRatePercent,
                    StockQuantity = v.StockQuantity,
                    IsActive = v.IsActive
                });
            }
            return dto;
        }
    }
}
