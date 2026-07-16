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
                    Address = req.Address.Trim()
                };

                var storeNames = db.Stores.ToDictionary(s => s.Id, s => s.Name);

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
                }

                order.Subtotal = order.Lines.Sum(l => l.LineTotal);
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

        // GET /api/orders/{id}
        [HttpGet, Route("{id:guid}")]
        public IHttpActionResult Get(Guid id)
        {
            using (var db = new KiranaDbContext())
            {
                var order = db.Orders.Include(o => o.Lines).FirstOrDefault(o => o.Id == id);
                if (order == null) return NotFound();
                return Ok(ToDto(order));
            }
        }

        private static OrderDto ToDto(Order o)
        {
            var dto = new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.CustomerName,
                Phone = o.Phone,
                Address = o.Address,
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
