using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Catalog;

// ---- Categories ----
public record CategoryDto(Guid Id, string Name, Guid? ParentCategoryId, bool IsActive);
public record CreateCategoryRequest(string Name, Guid? ParentCategoryId);

// ---- Variants ----
public record CreateVariantRequest(
    string Name,
    string? Sku,
    string? Barcode,
    UnitOfMeasure Unit,
    decimal PackSize,
    decimal Mrp,
    decimal SellingPrice,
    decimal TaxRatePercent,
    int StockQuantity,
    int ReorderLevel);

public record UpdateVariantRequest(
    string Name,
    decimal Mrp,
    decimal SellingPrice,
    decimal TaxRatePercent,
    int ReorderLevel,
    bool IsActive);

public record ProductVariantDto(
    Guid Id,
    Guid ProductId,
    string Name,
    string? Sku,
    string? Barcode,
    UnitOfMeasure Unit,
    decimal PackSize,
    decimal Mrp,
    decimal SellingPrice,
    decimal DiscountAmount,
    decimal DiscountPercent,
    decimal TaxRatePercent,
    int StockQuantity,
    int ReorderLevel,
    bool IsActive);

// ---- Products ----
public record CreateProductRequest(
    string Name,
    string? Description,
    Guid? CategoryId,
    string? Brand,
    string? ImageUrl,
    IReadOnlyList<CreateVariantRequest> Variants);

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    Guid? CategoryId,
    string? Brand,
    string? ImageUrl,
    bool IsActive,
    IReadOnlyList<ProductVariantDto> Variants);
