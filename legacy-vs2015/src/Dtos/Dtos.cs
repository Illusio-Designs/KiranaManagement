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
}
