using Kirana.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kirana.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.HasIndex(c => new { c.StoreId, c.Name });
    }
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.Brand).HasMaxLength(150);
        builder.Property(p => p.ImageUrl).HasMaxLength(500);

        builder.HasMany(p => p.Variants)
            .WithOne(v => v.Product!)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.StoreId);
        builder.HasIndex(p => new { p.StoreId, p.CategoryId });
    }
}

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Name).IsRequired().HasMaxLength(120);
        builder.Property(v => v.Sku).HasMaxLength(60);
        builder.Property(v => v.Barcode).HasMaxLength(60);
        builder.Property(v => v.Unit).HasConversion<int>();
        builder.Property(v => v.PackSize).HasColumnType("decimal(12,3)");
        builder.Property(v => v.Mrp).HasColumnType("decimal(18,2)");
        builder.Property(v => v.SellingPrice).HasColumnType("decimal(18,2)");
        builder.Property(v => v.TaxRatePercent).HasColumnType("decimal(5,2)");

        // Computed, not persisted.
        builder.Ignore(v => v.DiscountAmount);
        builder.Ignore(v => v.DiscountPercent);

        builder.HasIndex(v => v.StoreId);
        builder.HasIndex(v => v.ProductId);
        builder.HasIndex(v => new { v.StoreId, v.Sku });
    }
}
