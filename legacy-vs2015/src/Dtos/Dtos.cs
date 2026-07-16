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

    public class RegisterCustomerRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Store-owner Google sign-in: browser obtains a Google ID token and posts it.
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }
    }

    // Consumer OTP login.
    public class RequestOtpRequest
    {
        public string Phone { get; set; }
    }

    public class VerifyOtpRequest
    {
        public string Phone { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; } // optional, used on first sign-in
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
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public List<CreateVariantRequest> Variants { get; set; }
    }

    public class CatalogImageDto
    {
        public string Name { get; set; }
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
    }

    public class UpsertCatalogImageRequest
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
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
        public string StoreName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string RejectionReason { get; set; }
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

    // ----- Consumer marketplace -----
    public class MarketVariantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Mrp { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public int StockQuantity { get; set; }
    }

    public class CategoryCountDto
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class MarketProductDto
    {
        public MarketProductDto() { Variants = new List<MarketVariantDto>(); }
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxDiscountPercent { get; set; }
        public List<MarketVariantDto> Variants { get; set; }
    }

    // ----- Consumer order (guest checkout) -----
    public class PlaceOrderLineRequest
    {
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }

    public class PlaceOrderRequest
    {
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        // Geo-location of the delivery address (from the browser / map picker).
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<PlaceOrderLineRequest> Lines { get; set; }
    }

    public class OrderLineDto
    {
        public string StoreName { get; set; }
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class OrderDto
    {
        public OrderDto() { Lines = new List<OrderLineDto>(); }
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? DistanceKm { get; set; }
        public int? EtaMinutes { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderLineDto> Lines { get; set; }
    }

    // ----- Store-facing order view (consumer identity intentionally hidden) -----
    // A store owner sees only what they need to fulfil their part of an order:
    // items, quantities, this store's total, the delivery AREA (city + pincode)
    // and an approximate distance/ETA. Name, phone and full address are omitted.
    public class StoreOrderLineDto
    {
        public string ProductName { get; set; }
        public string VariantName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public string Status { get; set; }
    }

    public class StoreOrderDto
    {
        public StoreOrderDto() { Lines = new List<StoreOrderLineDto>(); }
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public string DeliveryArea { get; set; }   // e.g. "Andheri West, Mumbai 400058"
        public double? DistanceKm { get; set; }
        public int? EtaMinutes { get; set; }
        public int ItemCount { get; set; }
        public decimal StoreTotal { get; set; }    // total for THIS store's lines only
        public DateTime CreatedAt { get; set; }
        public List<StoreOrderLineDto> Lines { get; set; }
    }
}
