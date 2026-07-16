using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Sales;

public record CreateSaleLineRequest(Guid ProductVariantId, int Quantity, decimal? UnitPriceOverride);

public record CreateSaleRequest(
    string? CustomerName,
    string? CustomerPhone,
    PaymentMode PaymentMode,
    decimal? AmountPaid,
    IReadOnlyList<CreateSaleLineRequest> Lines);

public record SalesLineDto(
    Guid Id,
    Guid ProductVariantId,
    string ProductName,
    string VariantName,
    int Quantity,
    decimal UnitMrp,
    decimal UnitPrice,
    decimal TaxRatePercent,
    decimal LineDiscount,
    decimal TaxAmount,
    decimal LineTotal);

public record SalesInvoiceDto(
    Guid Id,
    string InvoiceNumber,
    string? CustomerName,
    string? CustomerPhone,
    decimal TaxableTotal,
    decimal TaxTotal,
    decimal DiscountTotal,
    decimal GrandTotal,
    PaymentMode PaymentMode,
    decimal AmountPaid,
    decimal ChangeDue,
    SaleStatus Status,
    DateTime CreatedAt,
    IReadOnlyList<SalesLineDto> Lines);
