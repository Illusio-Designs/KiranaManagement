using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Common.Enums;
using Kirana.Domain.Inventory;
using Kirana.Domain.Sales;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Sales;

public class SalesService : ISalesService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentTenant _tenant;

    public SalesService(IAppDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    private Guid StoreId => _tenant.HasTenant
        ? _tenant.StoreId
        : throw AppException.Unauthorized("No store context.");

    public async Task<SalesInvoiceDto> CreateSaleAsync(CreateSaleRequest request, CancellationToken ct = default)
    {
        if (request.Lines is null || request.Lines.Count == 0)
            throw new AppException("A sale needs at least one line.");

        var storeId = StoreId;
        var invoice = new SalesInvoice
        {
            StoreId = storeId,
            InvoiceNumber = GenerateInvoiceNumber(),
            CustomerName = request.CustomerName?.Trim(),
            CustomerPhone = request.CustomerPhone?.Trim(),
            PaymentMode = request.PaymentMode
        };

        foreach (var line in request.Lines)
        {
            if (line.Quantity <= 0)
                throw new AppException("Line quantity must be at least 1.");

            var variant = await _db.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == line.ProductVariantId, ct)
                ?? throw AppException.NotFound($"Variant {line.ProductVariantId} not found.");

            if (!variant.IsActive)
                throw new AppException($"'{variant.Name}' is not available for sale.");

            var unitPrice = line.UnitPriceOverride ?? variant.SellingPrice;
            if (unitPrice <= 0) throw new AppException("Unit price must be greater than zero.");
            if (unitPrice > variant.Mrp) throw new AppException("Unit price cannot exceed MRP.");
            if (variant.StockQuantity < line.Quantity)
                throw new AppException($"Insufficient stock for '{variant.Name}': {variant.StockQuantity} left.");

            var lineTotal = unitPrice * line.Quantity;
            var taxAmount = variant.TaxRatePercent > 0
                ? Math.Round(lineTotal * variant.TaxRatePercent / (100m + variant.TaxRatePercent), 2)
                : 0m;

            invoice.Lines.Add(new SalesLine
            {
                StoreId = storeId,
                ProductVariantId = variant.Id,
                ProductName = variant.Product!.Name,
                VariantName = variant.Name,
                Quantity = line.Quantity,
                UnitMrp = variant.Mrp,
                UnitPrice = unitPrice,
                TaxRatePercent = variant.TaxRatePercent,
                LineDiscount = (variant.Mrp - unitPrice) * line.Quantity,
                TaxAmount = taxAmount,
                LineTotal = lineTotal
            });

            // Decrement stock + ledger.
            variant.StockQuantity -= line.Quantity;
            _db.StockLedger.Add(new StockLedgerEntry
            {
                StoreId = storeId,
                ProductVariantId = variant.Id,
                ChangeQuantity = -line.Quantity,
                BalanceAfter = variant.StockQuantity,
                MovementType = StockMovementType.Sale,
                Reason = $"Sale {invoice.InvoiceNumber}"
            });
        }

        invoice.GrandTotal = invoice.Lines.Sum(l => l.LineTotal);
        invoice.TaxTotal = invoice.Lines.Sum(l => l.TaxAmount);
        invoice.TaxableTotal = invoice.GrandTotal - invoice.TaxTotal;
        invoice.DiscountTotal = invoice.Lines.Sum(l => l.LineDiscount);

        var amountPaid = request.AmountPaid ?? invoice.GrandTotal;
        invoice.AmountPaid = amountPaid;
        invoice.ChangeDue = amountPaid > invoice.GrandTotal ? amountPaid - invoice.GrandTotal : 0m;

        _db.SalesInvoices.Add(invoice);
        await _db.SaveChangesAsync(ct);

        return Map(invoice);
    }

    public async Task<IReadOnlyList<SalesInvoiceDto>> GetSalesAsync(CancellationToken ct = default)
    {
        var invoices = await _db.SalesInvoices
            .Include(i => i.Lines)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(ct);
        return invoices.Select(Map).ToList();
    }

    public async Task<SalesInvoiceDto> GetSaleAsync(Guid invoiceId, CancellationToken ct = default)
    {
        var invoice = await _db.SalesInvoices
            .Include(i => i.Lines)
            .FirstOrDefaultAsync(i => i.Id == invoiceId, ct)
            ?? throw AppException.NotFound("Sale not found.");
        return Map(invoice);
    }

    private static string GenerateInvoiceNumber()
        => $"INV-{DateTime.UtcNow:yyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..4].ToUpperInvariant()}";

    private static SalesInvoiceDto Map(SalesInvoice i) => new(
        i.Id, i.InvoiceNumber, i.CustomerName, i.CustomerPhone,
        i.TaxableTotal, i.TaxTotal, i.DiscountTotal, i.GrandTotal,
        i.PaymentMode, i.AmountPaid, i.ChangeDue, i.Status, i.CreatedAt,
        i.Lines.Select(l => new SalesLineDto(
            l.Id, l.ProductVariantId, l.ProductName, l.VariantName, l.Quantity,
            l.UnitMrp, l.UnitPrice, l.TaxRatePercent, l.LineDiscount, l.TaxAmount, l.LineTotal)).ToList());
}
