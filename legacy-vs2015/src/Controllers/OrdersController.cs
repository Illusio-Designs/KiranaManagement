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
    // Customer checkout (login required): place an order (can span stores) and view it.
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        // POST /api/orders  (customer must be logged in — no guest orders)
        [HttpPost, Route("")]
        public IHttpActionResult Place(PlaceOrderRequest req)
        {
            var customer = AuthUtil.GetCurrentUser(Request);
            if (customer == null || customer.Role != UserRole.Customer)
                return Content(HttpStatusCode.Unauthorized, new { error = "Please log in as a customer to place an order." });

            if (req == null || req.Lines == null || req.Lines.Count == 0)
                return BadRequest("Your cart is empty.");
            if (string.IsNullOrWhiteSpace(req.CustomerName) || string.IsNullOrWhiteSpace(req.Phone)
                || string.IsNullOrWhiteSpace(req.Address))
                return BadRequest("Name, phone and address are required.");

            using (var db = new KiranaDbContext())
            {
                var order = new Order
                {
                    CustomerId = customer.Id,
                    OrderNumber = "ORD-" + DateTime.UtcNow.ToString("yyMMddHHmmss"),
                    CustomerName = req.CustomerName.Trim(),
                    Phone = req.Phone.Trim(),
                    Address = req.Address.Trim(),
                    City = (req.City ?? "").Trim(),
                    Pincode = (req.Pincode ?? "").Trim(),
                    Latitude = req.Latitude,
                    Longitude = req.Longitude,
                    Status = OrderStatus.Placed
                };

                var stores = db.Stores.ToList();
                var storeNames = stores.ToDictionary(s => s.Id, s => s.Name);
                var involvedStoreIds = new HashSet<Guid>();

                foreach (var line in req.Lines)
                {
                    if (line.Quantity <= 0) return BadRequest("Invalid quantity.");

                    var variant = db.ProductVariants.Include(v => v.Product)
                        .FirstOrDefault(v => v.Id == line.ProductVariantId && v.IsActive);
                    if (variant == null) return BadRequest("A product in your cart is no longer available.");
                    if (variant.StockQuantity < line.Quantity)
                        return BadRequest("'" + variant.Name + "' has only " + variant.StockQuantity + " left.");

                    order.Lines.Add(new OrderLine
                    {
                        OrderId = order.Id,
                        StoreId = variant.StoreId,
                        StoreName = storeNames.ContainsKey(variant.StoreId) ? storeNames[variant.StoreId] : "",
                        ProductVariantId = variant.Id,
                        ProductName = variant.Product != null ? variant.Product.Name : "",
                        VariantName = variant.Name,
                        Quantity = line.Quantity,
                        UnitPrice = variant.SellingPrice,
                        LineTotal = variant.SellingPrice * line.Quantity
                    });

                    variant.StockQuantity -= line.Quantity;
                    involvedStoreIds.Add(variant.StoreId);
                }

                order.Subtotal = order.Lines.Sum(l => l.LineTotal);

                // --- Geo-based delivery estimate ---
                // Farthest fulfilling store from the delivery point drives the ETA.
                if (order.Latitude.HasValue && order.Longitude.HasValue)
                {
                    double? maxDist = null;
                    foreach (var s in stores.Where(s => involvedStoreIds.Contains(s.Id)
                        && s.Latitude.HasValue && s.Longitude.HasValue))
                    {
                        var d = GeoUtil.DistanceKm(s.Latitude.Value, s.Longitude.Value,
                            order.Latitude.Value, order.Longitude.Value);
                        if (!maxDist.HasValue || d > maxDist.Value) maxDist = d;
                    }
                    if (maxDist.HasValue)
                    {
                        order.DistanceKm = Math.Round(maxDist.Value, 2);
                        order.EtaMinutes = GeoUtil.EtaMinutes(maxDist.Value);
                        // Distance-based fee: free within 3 km, then ₹8/km, capped.
                        var fee = maxDist.Value <= 3.0 ? 0m : (decimal)Math.Ceiling((maxDist.Value - 3.0) * 8.0);
                        if (fee > 120m) fee = 120m;
                        order.DeliveryFee = fee;
                    }
                }

                // Fallback fee when no geo is available (original subtotal rule).
                if (!order.DistanceKm.HasValue)
                    order.DeliveryFee = order.Subtotal >= 500m ? 0m : 40m;

                order.GrandTotal = order.Subtotal + order.DeliveryFee;

                db.Orders.Add(order);
                db.SaveChanges();
                return Ok(ToDto(order));
            }
        }

        // GET /api/orders/mine  (logged-in customer's order history)
        [HttpGet, Route("mine")]
        public IHttpActionResult Mine()
        {
            var customer = AuthUtil.GetCurrentUser(Request);
            if (customer == null || customer.Role != UserRole.Customer)
                return Content(HttpStatusCode.Unauthorized, new { error = "Please log in." });

            using (var db = new KiranaDbContext())
            {
                var orders = db.Orders.Include(o => o.Lines)
                    .Where(o => o.CustomerId == customer.Id)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToList();
                return Ok(orders.Select(ToDto).ToList());
            }
        }

        // GET /api/orders/{id}  (only the owning customer may view full details)
        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            var customer = AuthUtil.GetCurrentUser(Request);
            if (customer == null || customer.Role != UserRole.Customer)
                return Content(HttpStatusCode.Unauthorized, new { error = "Please log in." });

            using (var db = new KiranaDbContext())
            {
                var order = db.Orders.Include(o => o.Lines).FirstOrDefault(o => o.Id == id);
                if (order == null) return NotFound();
                if (order.CustomerId != customer.Id)
                    return Content(HttpStatusCode.Forbidden, new { error = "This order does not belong to you." });
                return Ok(ToDto(order));
            }
        }

        private static OrderDto ToDto(Order o)
        {
            var dto = new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                Status = o.Status.ToString(),
                CustomerName = o.CustomerName,
                Phone = o.Phone,
                Address = o.Address,
                City = o.City,
                Pincode = o.Pincode,
                Latitude = o.Latitude,
                Longitude = o.Longitude,
                DistanceKm = o.DistanceKm,
                EtaMinutes = o.EtaMinutes,
                Subtotal = o.Subtotal,
                DeliveryFee = o.DeliveryFee,
                GrandTotal = o.GrandTotal,
                CreatedAt = o.CreatedAt
            };
            var lines = o.Lines ?? new List<OrderLine>();
            foreach (var l in lines)
            {
                dto.Lines.Add(new OrderLineDto
                {
                    StoreName = l.StoreName,
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
