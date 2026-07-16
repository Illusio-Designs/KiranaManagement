using Kirana.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kirana.Infrastructure.Persistence.Configurations;

public class SalesInvoiceConfiguration : IEntityTypeConfiguration<SalesInvoice>
{
    public void Configure(EntityTypeBuilder<SalesInvoice> builder)
    {
        builder.ToTable("sales_invoices");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(40);
        builder.Property(i => i.CustomerName).HasMaxLength(200);
        builder.Property(i => i.CustomerPhone).HasMaxLength(20);
        builder.Property(i => i.PaymentMode).HasConversion<int>();
        builder.Property(i => i.Status).HasConversion<int>();

        foreach (var p in new[] { nameof(SalesInvoice.TaxableTotal), nameof(SalesInvoice.TaxTotal),
                     nameof(SalesInvoice.DiscountTotal), nameof(SalesInvoice.GrandTotal),
                     nameof(SalesInvoice.AmountPaid), nameof(SalesInvoice.ChangeDue) })
            builder.Property(p).HasColumnType("decimal(18,2)");

        builder.HasMany(i => i.Lines)
            .WithOne(l => l.SalesInvoice!)
            .HasForeignKey(l => l.SalesInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => i.StoreId);
        builder.HasIndex(i => new { i.StoreId, i.InvoiceNumber }).IsUnique();
    }
}

public class SalesLineConfiguration : IEntityTypeConfiguration<SalesLine>
{
    public void Configure(EntityTypeBuilder<SalesLine> builder)
    {
        builder.ToTable("sales_lines");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.ProductName).HasMaxLength(200);
        builder.Property(l => l.VariantName).HasMaxLength(120);

        foreach (var p in new[] { nameof(SalesLine.UnitMrp), nameof(SalesLine.UnitPrice),
                     nameof(SalesLine.LineDiscount), nameof(SalesLine.TaxAmount), nameof(SalesLine.LineTotal) })
            builder.Property(p).HasColumnType("decimal(18,2)");
        builder.Property(l => l.TaxRatePercent).HasColumnType("decimal(5,2)");

        builder.HasIndex(l => l.SalesInvoiceId);
    }
}
