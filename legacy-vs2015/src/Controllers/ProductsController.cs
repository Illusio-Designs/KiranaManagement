using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Kirana.WebApi.Data;
using Kirana.WebApi.Dtos;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : StoreApiController
    {
        // POST /api/products
        [HttpPost, Route("")]
        public IHttpActionResult Create(CreateProductRequest req)
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;

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

            using (var db = new KiranaDbContext())
            {
                var product = new Product
                {
                    StoreId = CurrentStoreId,
                    Name = req.Name.Trim(),
                    Description = req.Description,
                    Brand = req.Brand,
                    Category = (req.Category ?? "").Trim()
                    // Status defaults to Pending — goes live only after admin approval.
                };

                foreach (var v in req.Variants)
                {
                    product.Variants.Add(new ProductVariant
                    {
                        StoreId = CurrentStoreId,
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

        // GET /api/products
        [HttpGet, Route("")]
        public IHttpActionResult List()
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var products = db.Products
                    .Include(p => p.Variants)
                    .Where(p => p.StoreId == CurrentStoreId)
                    .OrderBy(p => p.Name)
                    .ToList();
                return Ok(products.Select(ToDto).ToList());
            }
        }

        // POST /api/products/variants/{id}/adjust  (inventory stock change)
        [HttpPost, Route("variants/{variantId:guid}/adjust")]
        public IHttpActionResult AdjustStock(Guid variantId, AdjustStockRequest req)
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;
            if (req == null || req.ChangeQuantity == 0)
                return BadRequest("Change quantity cannot be zero.");

            using (var db = new KiranaDbContext())
            {
                var variant = db.ProductVariants.FirstOrDefault(v => v.Id == variantId && v.StoreId == CurrentStoreId);
                if (variant == null) return NotFound();

                var newBalance = variant.StockQuantity + req.ChangeQuantity;
                if (newBalance < 0)
                    return BadRequest("Insufficient stock for this adjustment.");

                variant.StockQuantity = newBalance;
                db.SaveChanges();
                return Ok(new { variantId = variant.Id, stockQuantity = variant.StockQuantity });
            }
        }

        // GET /api/products/low-stock
        [HttpGet, Route("low-stock")]
        public IHttpActionResult LowStock()
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var items = db.ProductVariants
                    .Where(v => v.StoreId == CurrentStoreId && v.IsActive && v.StockQuantity <= v.ReorderLevel)
                    .Include(v => v.Product)
                    .OrderBy(v => v.StockQuantity)
                    .ToList()
                    .Select(v => new LowStockItemDto
                    {
                        VariantId = v.Id,
                        ProductName = v.Product != null ? v.Product.Name : "",
                        VariantName = v.Name,
                        StockQuantity = v.StockQuantity,
                        ReorderLevel = v.ReorderLevel
                    })
                    .ToList();
                return Ok(items);
            }
        }

        // ---------- SuperAdmin product moderation ----------

        private IHttpActionResult RequireSuperAdmin()
        {
            var user = AuthUtil.GetCurrentUser(Request);
            if (user == null || user.Role != UserRole.SuperAdmin)
                return Content(System.Net.HttpStatusCode.Unauthorized, new { error = "Super admin login required." });
            return null;
        }

        // GET /api/products/pending-approval  (SuperAdmin) — products awaiting review, all stores.
        [HttpGet, Route("pending-approval")]
        public IHttpActionResult PendingApproval()
        {
            var guard = RequireSuperAdmin();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var storeNames = db.Stores.ToDictionary(s => s.Id, s => s.Name);
                var products = db.Products.Include(p => p.Variants)
                    .Where(p => p.Status == ProductStatus.Pending)
                    .OrderBy(p => p.CreatedAt)
                    .ToList();
                var list = products.Select(p =>
                {
                    var dto = ToDto(p);
                    dto.StoreName = storeNames.ContainsKey(p.StoreId) ? storeNames[p.StoreId] : "";
                    return dto;
                }).ToList();
                return Ok(list);
            }
        }

        // POST /api/products/{id}/approve  (SuperAdmin)
        [HttpPost, Route("{id:guid}/approve")]
        public IHttpActionResult Approve(Guid id)
        {
            var guard = RequireSuperAdmin();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var product = db.Products.Find(id);
                if (product == null) return NotFound();
                product.Status = ProductStatus.Approved;
                product.ApprovedAt = DateTime.UtcNow;
                product.RejectionReason = null;
                db.SaveChanges();
                return Ok(new { id = product.Id, status = product.Status.ToString() });
            }
        }

        // POST /api/products/{id}/reject  (SuperAdmin)
        [HttpPost, Route("{id:guid}/reject")]
        public IHttpActionResult Reject(Guid id, RejectRequest req)
        {
            var guard = RequireSuperAdmin();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var product = db.Products.Find(id);
                if (product == null) return NotFound();
                product.Status = ProductStatus.Rejected;
                product.RejectionReason = req != null ? req.Reason : "Rejected by admin";
                db.SaveChanges();
                return Ok(new { id = product.Id, status = product.Status.ToString() });
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
                Category = p.Category,
                IsActive = p.IsActive,
                Status = p.Status.ToString(),
                RejectionReason = p.RejectionReason
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
