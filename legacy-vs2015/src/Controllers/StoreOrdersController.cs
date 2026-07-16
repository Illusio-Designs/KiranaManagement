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
    // Store-owner view of marketplace orders. Deliberately exposes NO consumer
    // personal data (no name / phone / full address). A store only sees the items
    // it must fulfil, the delivery AREA (city + pincode), and an approximate
    // distance / ETA. This keeps shopper identity private in a marketplace model.
    [RoutePrefix("api/store/orders")]
    public class StoreOrdersController : StoreApiController
    {
        // GET /api/store/orders  -> orders that include at least one of this store's items.
        [HttpGet, Route("")]
        public IHttpActionResult Mine()
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var storeId = CurrentStoreId;

                // Only orders that contain a line for THIS store.
                var orders = db.Orders.Include(o => o.Lines)
                    .Where(o => o.Lines.Any(l => l.StoreId == storeId))
                    .OrderByDescending(o => o.CreatedAt)
                    .ToList();

                var result = orders.Select(o => ToStoreDto(o, storeId)).ToList();
                return Ok(result);
            }
        }

        // GET /api/store/orders/{id}  -> single order, this store's slice only.
        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;

            using (var db = new KiranaDbContext())
            {
                var storeId = CurrentStoreId;
                var order = db.Orders.Include(o => o.Lines).FirstOrDefault(o => o.Id == id);
                if (order == null || order.Lines.All(l => l.StoreId != storeId))
                    return NotFound();
                return Ok(ToStoreDto(order, storeId));
            }
        }

        // PUT /api/store/orders/{id}/status  body: { "status": "Packed" }
        [HttpPut, Route("{id:guid}/status")]
        public IHttpActionResult UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest req)
        {
            var guard = EnsureStoreUser();
            if (guard != null) return guard;
            if (req == null || string.IsNullOrWhiteSpace(req.Status))
                return BadRequest("Status is required.");

            OrderStatus status;
            if (!Enum.TryParse(req.Status.Trim(), true, out status))
                return BadRequest("Unknown status.");

            using (var db = new KiranaDbContext())
            {
                var storeId = CurrentStoreId;
                var order = db.Orders.Include(o => o.Lines).FirstOrDefault(o => o.Id == id);
                if (order == null || order.Lines.All(l => l.StoreId != storeId))
                    return NotFound();

                order.Status = status;
                db.SaveChanges();
                return Ok(ToStoreDto(order, storeId));
            }
        }

        private static StoreOrderDto ToStoreDto(Order o, Guid storeId)
        {
            var mine = (o.Lines ?? new List<OrderLine>()).Where(l => l.StoreId == storeId).ToList();

            var area = string.Join(" ",
                new[] { (o.City ?? "").Trim(), (o.Pincode ?? "").Trim() }
                .Where(s => !string.IsNullOrEmpty(s)));
            if (string.IsNullOrWhiteSpace(area)) area = "Delivery area not specified";

            var dto = new StoreOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status.ToString(),
                DeliveryArea = area,          // city + pincode only — no street address
                DistanceKm = o.DistanceKm,
                EtaMinutes = o.EtaMinutes,
                ItemCount = mine.Sum(l => l.Quantity),
                StoreTotal = mine.Sum(l => l.LineTotal),
                CreatedAt = o.CreatedAt
            };
            foreach (var l in mine)
            {
                dto.Lines.Add(new StoreOrderLineDto
                {
                    ProductName = l.ProductName,
                    VariantName = l.VariantName,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    LineTotal = l.LineTotal
                });
            }
            return dto;
        }
    }
}
