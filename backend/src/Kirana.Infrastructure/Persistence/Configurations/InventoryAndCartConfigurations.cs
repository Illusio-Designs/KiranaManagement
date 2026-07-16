using Kirana.Domain.Inventory;
using Kirana.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kirana.Infrastructure.Persistence.Configurations;

public class StockLedgerEntryConfiguration : IEntityTypeConfiguration<StockLedgerEntry>
{
    public void Configure(EntityTypeBuilder<StockLedgerEntry> builder)
    {
        builder.ToTable("stock_ledger");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.MovementType).HasConversion<int>();
        builder.Property(e => e.Reason).HasMaxLength(300);
        builder.HasIndex(e => new { e.StoreId, e.ProductVariantId });
    }
}

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("carts");
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.CustomerId);
        builder.HasMany(c => c.Items)
            .WithOne(i => i.Cart!)
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("cart_items");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.StoreName).HasMaxLength(200);
        builder.Property(i => i.ProductName).HasMaxLength(200);
        builder.Property(i => i.VariantName).HasMaxLength(120);
        builder.Property(i => i.UnitMrp).HasColumnType("decimal(18,2)");
        builder.Property(i => i.UnitSellingPrice).HasColumnType("decimal(18,2)");
        builder.HasIndex(i => new { i.CartId, i.ProductVariantId });
    }
}
