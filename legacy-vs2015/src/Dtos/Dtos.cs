using System;
using System.Collections.Generic;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Dtos
{
    // ----- Store onboarding -----
    public class RegisterStoreRequest
    {
        public string StoreName { get; set; }
        public string OwnerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string Gstin { get; set; }
        public string Pan { get; set; }
    }

    public class StoreDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Gstin { get; set; }
        public string Pan { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string RejectionReason { get; set; }
    }

    public class RejectRequest
    {
        public string Reason { get; set; }
    }

    // ----- Auth -----
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Guid? StoreId { get; set; }
    }

    // ----- Catalog -----
    public class CreateVariantRequest
    {
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Barcode { get; set; }
        public UnitOfMeasure Unit { get; set; }
        public decimal PackSize { get; set; }
        public decimal Mrp { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal TaxRatePercent { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
    }

    public class CreateProductRequest
    {
        public Guid StoreId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public List<CreateVariantRequest> Variants { get; set; }
    }

    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public UnitOfMeasure Unit { get; set; }
        public decimal PackSize { get; set; }
        public decimal Mrp { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal TaxRatePercent { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductDto
    {
        public ProductDto()
        {
            Variants = new List<ProductVariantDto>();
        }

        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public bool IsActive { get; set; }
        public List<ProductVariantDto> Variants { get; set; }
    }

    // ----- Inventory -----
    public class AdjustStockRequest
    {
        public int ChangeQuantity { get; set; }   // + in, - out
        public string Reason { get; set; }
    }

    public class LowStockItemDto
    {
        public Guid VariantId { get; set; }
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
    }

    // ----- POS / Sales -----
    public class CreateSaleLineRequest
    {
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateSaleRequest
    {
        public string CustomerName { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public decimal? AmountPaid { get; set; }
        public List<CreateSaleLineRequest> Lines { get; set; }
    }

    public class SaleLineDto
    {
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitMrp { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineDiscount { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class SaleDto
    {
        public SaleDto() { Lines = new List<SaleLineDto>(); }
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string PaymentMode { get; set; }
        public decimal TaxableTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ChangeDue { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SaleLineDto> Lines { get; set; }
    }

    // ----- Suppliers / Purchases -----
    public class CreateSupplierRequest
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gstin { get; set; }
    }

    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Gstin { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreatePurchaseLineRequest
    {
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }

    public class CreatePurchaseOrderRequest
    {
        public Guid SupplierId { get; set; }
        public List<CreatePurchaseLineRequest> Lines { get; set; }
    }

    public class PurchaseLineDto
    {
        public string VariantName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class PurchaseOrderDto
    {
        public PurchaseOrderDto() { Lines = new List<PurchaseLineDto>(); }
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string PoNumber { get; set; }
        public string Status { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public List<PurchaseLineDto> Lines { get; set; }
    }
}
