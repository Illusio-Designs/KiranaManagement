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
    [RoutePrefix("api/sales")]
    public class SalesController : StoreApiController
    {
        // POST /api/sales  — ring up a POS sale (decrements stock).
        [HttpPost, Route("")]
        public IHttpActionResult Create(CreateSaleRequest req)
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;

            if (req == null || req.Lines == null || req.Lines.Count == 0)
                return BadRequest("A sale needs at least one line.");

            using (var db = new KiranaDbContext())
            {
                var invoice = new SalesInvoice
                {
                    StoreId = CurrentStoreId,
                    InvoiceNumber = "INV-" + DateTime.UtcNow.ToString("yyMMddHHmmss"),
                    CustomerName = req.CustomerName,
                    PaymentMode = req.PaymentMode
                };

                foreach (var line in req.Lines)
                {
                    if (line.Quantity <= 0) return BadRequest("Line quantity must be at least 1.");

                    var variant = db.ProductVariants
                        .Include(v => v.Product)
                        .FirstOrDefault(v => v.Id == line.ProductVariantId && v.StoreId == CurrentStoreId);
                    if (variant == null) return BadRequest("A product in the sale was not found.");
                    if (variant.StockQuantity < line.Quantity)
                        return BadRequest("Insufficient stock for '" + variant.Name + "' (" + variant.StockQuantity + " left).");

                    var lineTotal = variant.SellingPrice * line.Quantity;
                    var taxAmount = variant.TaxRatePercent > 0
                        ? Math.Round(lineTotal * variant.TaxRatePercent / (100m + variant.TaxRatePercent), 2)
                        : 0m;

                    invoice.Lines.Add(new SalesLine
                    {
                        StoreId = CurrentStoreId,
                        ProductVariantId = variant.Id,
                        ProductName = variant.Product != null ? variant.Product.Name : "",
                        VariantName = variant.Name,
                        Quantity = line.Quantity,
                        UnitMrp = variant.Mrp,
                        UnitPrice = variant.SellingPrice,
                        TaxRatePercent = variant.TaxRatePercent,
                        LineDiscount = (variant.Mrp - variant.SellingPrice) * line.Quantity,
                        TaxAmount = taxAmount,
                        LineTotal = lineTotal
                    });

                    variant.StockQuantity -= line.Quantity;
                }

                invoice.GrandTotal = invoice.Lines.Sum(l => l.LineTotal);
                invoice.TaxTotal = invoice.Lines.Sum(l => l.TaxAmount);
                invoice.TaxableTotal = invoice.GrandTotal - invoice.TaxTotal;
                invoice.DiscountTotal = invoice.Lines.Sum(l => l.LineDiscount);
                var paid = req.AmountPaid.HasValue ? req.AmountPaid.Value : invoice.GrandTotal;
                invoice.AmountPaid = paid;
                invoice.ChangeDue = paid > invoice.GrandTotal ? paid - invoice.GrandTotal : 0m;

                db.SalesInvoices.Add(invoice);
                db.SaveChanges();
                return Ok(ToDto(invoice));
            }
        }

        // GET /api/sales  — recent sales for this store.
        [HttpGet, Route("")]
        public IHttpActionResult List()
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var invoices = db.SalesInvoices
                    .Include(i => i.Lines)
                    .Where(i => i.StoreId == CurrentStoreId)
                    .OrderByDescending(i => i.CreatedAt)
                    .Take(50)
                    .ToList();
                return Ok(invoices.Select(ToDto).ToList());
            }
        }

        private static SaleDto ToDto(SalesInvoice i)
        {
            var dto = new SaleDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                CustomerName = i.CustomerName,
                PaymentMode = i.PaymentMode.ToString(),
                TaxableTotal = i.TaxableTotal,
                TaxTotal = i.TaxTotal,
                DiscountTotal = i.DiscountTotal,
                GrandTotal = i.GrandTotal,
                AmountPaid = i.AmountPaid,
                ChangeDue = i.ChangeDue,
                CreatedAt = i.CreatedAt
            };
            var lines = i.Lines ?? new List<SalesLine>();
            foreach (var l in lines)
            {
                dto.Lines.Add(new SaleLineDto
                {
                    ProductName = l.ProductName,
                    VariantName = l.VariantName,
                    Quantity = l.Quantity,
                    UnitMrp = l.UnitMrp,
                    UnitPrice = l.UnitPrice,
                    LineDiscount = l.LineDiscount,
                    LineTotal = l.LineTotal
                });
            }
            return dto;
        }
    }
}
