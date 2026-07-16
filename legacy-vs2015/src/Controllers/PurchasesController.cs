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
    // Suppliers + purchase orders + goods receipt (all in one controller).
    [RoutePrefix("api/purchases")]
    public class PurchasesController : StoreApiController
    {
        // ---- Suppliers ----
        [HttpPost, Route("suppliers")]
        public IHttpActionResult CreateSupplier(CreateSupplierRequest req)
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;
            if (req == null || string.IsNullOrWhiteSpace(req.Name))
                return BadRequest("Supplier name is required.");

            using (var db = new KiranaDbContext())
            {
                var s = new Supplier
                {
                    StoreId = CurrentStoreId,
                    Name = req.Name.Trim(),
                    Phone = req.Phone,
                    Email = req.Email,
                    Gstin = req.Gstin
                };
                db.Suppliers.Add(s);
                db.SaveChanges();
                return Ok(ToDto(s));
            }
        }

        [HttpGet, Route("suppliers")]
        public IHttpActionResult ListSuppliers()
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var list = db.Suppliers.Where(s => s.StoreId == CurrentStoreId).OrderBy(s => s.Name).ToList();
                return Ok(list.Select(ToDto).ToList());
            }
        }

        // ---- Purchase orders ----
        [HttpPost, Route("orders")]
        public IHttpActionResult CreateOrder(CreatePurchaseOrderRequest req)
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;
            if (req == null || req.Lines == null || req.Lines.Count == 0)
                return BadRequest("A purchase order needs at least one line.");

            using (var db = new KiranaDbContext())
            {
                if (!db.Suppliers.Any(s => s.Id == req.SupplierId && s.StoreId == CurrentStoreId))
                    return BadRequest("Supplier not found.");

                var po = new PurchaseOrder
                {
                    StoreId = CurrentStoreId,
                    SupplierId = req.SupplierId,
                    PoNumber = "PO-" + DateTime.UtcNow.ToString("yyMMddHHmmss")
                };

                foreach (var line in req.Lines)
                {
                    if (line.Quantity <= 0) return BadRequest("Line quantity must be at least 1.");
                    var variant = db.ProductVariants.Include(v => v.Product)
                        .FirstOrDefault(v => v.Id == line.ProductVariantId && v.StoreId == CurrentStoreId);
                    if (variant == null) return BadRequest("A product in the order was not found.");

                    po.Lines.Add(new PurchaseOrderLine
                    {
                        StoreId = CurrentStoreId,
                        ProductVariantId = variant.Id,
                        VariantName = (variant.Product != null ? variant.Product.Name + " - " : "") + variant.Name,
                        Quantity = line.Quantity,
                        UnitCost = line.UnitCost,
                        LineTotal = line.UnitCost * line.Quantity
                    });
                }

                po.TotalCost = po.Lines.Sum(l => l.LineTotal);
                db.PurchaseOrders.Add(po);
                db.SaveChanges();
                return Ok(ToDto(po));
            }
        }

        [HttpGet, Route("orders")]
        public IHttpActionResult ListOrders()
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var list = db.PurchaseOrders.Include(p => p.Lines)
                    .Where(p => p.StoreId == CurrentStoreId)
                    .OrderByDescending(p => p.CreatedAt).ToList();
                return Ok(list.Select(ToDto).ToList());
            }
        }

        // POST /api/purchases/orders/{id}/receive  — goods receipt, increases stock.
        [HttpPost, Route("orders/{id:guid}/receive")]
        public IHttpActionResult Receive(Guid id)
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var po = db.PurchaseOrders.Include(p => p.Lines)
                    .FirstOrDefault(p => p.Id == id && p.StoreId == CurrentStoreId);
                if (po == null) return NotFound();
                if (po.Status != PurchaseStatus.Draft)
                    return BadRequest("Only draft purchase orders can be received.");

                foreach (var line in po.Lines)
                {
                    var variant = db.ProductVariants.FirstOrDefault(v => v.Id == line.ProductVariantId);
                    if (variant == null) continue;
                    variant.StockQuantity += line.Quantity;
                }

                po.Status = PurchaseStatus.Received;
                po.ReceivedAt = DateTime.UtcNow;
                db.SaveChanges();
                return Ok(ToDto(po));
            }
        }

        private static SupplierDto ToDto(Supplier s)
        {
            return new SupplierDto { Id = s.Id, Name = s.Name, Phone = s.Phone, Email = s.Email, Gstin = s.Gstin, IsActive = s.IsActive };
        }

        private static PurchaseOrderDto ToDto(PurchaseOrder p)
        {
            var dto = new PurchaseOrderDto
            {
                Id = p.Id,
                SupplierId = p.SupplierId,
                PoNumber = p.PoNumber,
                Status = p.Status.ToString(),
                TotalCost = p.TotalCost,
                CreatedAt = p.CreatedAt,
                ReceivedAt = p.ReceivedAt
            };
            var lines = p.Lines ?? new List<PurchaseOrderLine>();
            foreach (var l in lines)
            {
                dto.Lines.Add(new PurchaseLineDto
                {
                    VariantName = l.VariantName,
                    Quantity = l.Quantity,
                    UnitCost = l.UnitCost,
                    LineTotal = l.LineTotal
                });
            }
            return dto;
        }
    }
}
