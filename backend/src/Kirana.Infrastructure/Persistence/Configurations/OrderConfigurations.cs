using Kirana.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kirana.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(40);
        builder.Property(o => o.ContactName).IsRequired().HasMaxLength(200);
        builder.Property(o => o.ContactPhone).IsRequired().HasMaxLength(20);
        builder.Property(o => o.AddressLine).IsRequired().HasMaxLength(500);
        builder.Property(o => o.City).HasMaxLength(120);
        builder.Property(o => o.Pincode).HasMaxLength(12);
        builder.Property(o => o.PaymentMode).HasConversion<int>();
        builder.Property(o => o.PaymentStatus).HasConversion<int>();
        builder.Property(o => o.Status).HasConversion<int>();

        foreach (var p in new[] { nameof(Order.ItemsSubtotal), nameof(Order.DiscountTotal),
                     nameof(Order.DeliveryFee), nameof(Order.GrandTotal) })
            builder.Property(p).HasColumnType("decimal(18,2)");

        builder.HasMany(o => o.StoreOrders)
            .WithOne(s => s.Order!)
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.OrderNumber).IsUnique();
    }
}

public class StoreOrderConfiguration : IEntityTypeConfiguration<StoreOrder>
{
    public void Configure(EntityTypeBuilder<StoreOrder> builder)
    {
        builder.ToTable("store_orders");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.StoreName).HasMaxLength(200);
        builder.Property(s => s.Status).HasConversion<int>();

        foreach (var p in new[] { nameof(StoreOrder.Subtotal), nameof(StoreOrder.MrpTotal), nameof(StoreOrder.Discount) })
            builder.Property(p).HasColumnType("decimal(18,2)");

        builder.HasMany(s => s.Lines)
            .WithOne(l => l.StoreOrder!)
            .HasForeignKey(l => l.StoreOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => new { s.StoreId, s.Status });
        builder.HasIndex(s => s.OrderId);
    }
}

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("order_lines");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.ProductName).HasMaxLength(200);
        builder.Property(l => l.VariantName).HasMaxLength(120);

        foreach (var p in new[] { nameof(OrderLine.UnitMrp), nameof(OrderLine.UnitSellingPrice),
                     nameof(OrderLine.LineDiscount), nameof(OrderLine.LineTotal) })
            builder.Property(p).HasColumnType("decimal(18,2)");

        builder.HasIndex(l => l.StoreOrderId);
    }
}
