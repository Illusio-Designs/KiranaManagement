using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Common.Enums;
using Kirana.Domain.Delivery;
using Kirana.Domain.Orders;
using Kirana.Domain.Platform;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Delivery;

public class DeliveryService : IDeliveryService
{
    private readonly IAppDbContext _db;
    private readonly IDeliveryProvider _provider;

    public DeliveryService(IAppDbContext db, IDeliveryProvider provider)
    {
        _db = db;
        _provider = provider;
    }

    public async Task<DeliveryDto> BookAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _db.Orders
            .IgnoreQueryFilters()
            .Include(o => o.StoreOrders)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw AppException.NotFound("Order not found.");

        if (order.Status != OrderStatus.ReadyForPickup)
            throw new AppException($"Order is not ready for delivery (status: {order.Status}).");

        if (await _db.DeliveryTasks.AnyAsync(t => t.OrderId == orderId, ct))
            throw AppException.Conflict("A delivery has already been booked for this order.");

        var activeStores = order.StoreOrders.Where(s => s.Status != StoreOrderStatus.Cancelled).ToList();
        if (activeStores.Count == 0)
            throw new AppException("This order has no active store parts to deliver.");

        var pickups = new List<PickupPoint>();
        var pickupStops = new List<GeoStop>();
        foreach (var so in activeStores)
        {
            var store = await _db.Stores.FirstOrDefaultAsync(s => s.Id == so.StoreId, ct);
            var address = store is null ? so.StoreName : FormatStoreAddress(store);
            pickups.Add(new PickupPoint
            {
                StoreId = so.StoreId,
                StoreName = so.StoreName,
                Address = address,
                Phone = store?.Phone
            });
            pickupStops.Add(new GeoStop(so.StoreName, address, store?.Phone));
        }

        var drop = new GeoStop(order.ContactName,
            $"{order.AddressLine}, {order.City} {order.Pincode}".Trim(), order.ContactPhone);
        var cod = order.IsCod ? order.GrandTotal : 0m;

        var result = await _provider.CreateTaskAsync(
            new DeliveryRequest(order.OrderNumber, pickupStops, drop, cod), ct);

        var task = new DeliveryTask
        {
            OrderId = order.Id,
            Provider = _provider.Name,
            ExternalTaskId = result.ExternalTaskId,
            TrackingUrl = result.TrackingUrl,
            Status = DeliveryStatus.Searching,
            Fee = result.Fee,
            CodAmount = cod,
            Pickups = pickups
        };

        _db.DeliveryTasks.Add(task);
        await _db.SaveChangesAsync(ct);

        return Map(task);
    }

    public async Task<DeliveryDto> GetForOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var task = await _db.DeliveryTasks.Include(t => t.Pickups)
            .FirstOrDefaultAsync(t => t.OrderId == orderId, ct)
            ?? throw AppException.NotFound("No delivery has been booked for this order.");
        return Map(task);
    }

    public async Task HandleWebhookAsync(string provider, DeliveryWebhookRequest payload, CancellationToken ct = default)
    {
        var task = await _db.DeliveryTasks.Include(t => t.Pickups)
            .FirstOrDefaultAsync(t => t.Provider == provider && t.ExternalTaskId == payload.ExternalTaskId, ct);
        if (task is null) return; // unknown task — ignore (idempotent)

        if (!Enum.TryParse<DeliveryStatus>(payload.Status, ignoreCase: true, out var status))
            return; // unrecognized status — ignore

        task.Status = status;
        if (!string.IsNullOrWhiteSpace(payload.RiderName)) task.RiderName = payload.RiderName;
        if (!string.IsNullOrWhiteSpace(payload.RiderPhone)) task.RiderPhone = payload.RiderPhone;
        if (!string.IsNullOrWhiteSpace(payload.PodReference)) task.PodReference = payload.PodReference;

        if (status is DeliveryStatus.PickedUp or DeliveryStatus.OutForDelivery or DeliveryStatus.Delivered)
            foreach (var p in task.Pickups) p.Collected = true;

        if (status == DeliveryStatus.Delivered)
        {
            task.DeliveredAt = DateTime.UtcNow;
            if (task.CodAmount > 0) task.CodCollected = true;
        }

        await SyncOrderStatusAsync(task.OrderId, status, ct);
        await _db.SaveChangesAsync(ct);
    }

    private async Task SyncOrderStatusAsync(Guid orderId, DeliveryStatus status, CancellationToken ct)
    {
        var order = await _db.Orders.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Id == orderId, ct);
        if (order is null) return;

        switch (status)
        {
            case DeliveryStatus.PickedUp:
            case DeliveryStatus.OutForDelivery:
                order.Status = OrderStatus.OutForDelivery;
                break;
            case DeliveryStatus.Delivered:
                order.Status = OrderStatus.Delivered;
                if (order.PaymentStatus != PaymentStatus.Paid) order.PaymentStatus = PaymentStatus.Paid;
                break;
        }
    }

    private static string FormatStoreAddress(Store s)
        => string.Join(", ", new[] { s.AddressLine, s.CityName, s.Pincode }
            .Where(x => !string.IsNullOrWhiteSpace(x)));

    private static DeliveryDto Map(DeliveryTask t) => new(
        t.Id, t.OrderId, t.Provider, t.ExternalTaskId, t.TrackingUrl, t.Status, t.Fee,
        t.RiderName, t.RiderPhone, t.CodAmount, t.CodCollected, t.PodReference, t.DeliveredAt,
        t.Pickups.Select(p => new PickupPointDto(p.Id, p.StoreId, p.StoreName, p.Address, p.Phone, p.Collected)).ToList());
}
