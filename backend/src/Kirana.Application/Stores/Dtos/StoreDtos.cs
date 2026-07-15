using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Stores.Dtos;

/// <summary>Public self-registration payload (PRD FR-1.1). Location is chosen via
/// the country → state → city cascade (see /api/geo).</summary>
public record RegisterStoreRequest(
    string StoreName,
    string OwnerName,
    string Email,
    string Phone,
    string Password,
    string? AddressLine,
    string? Pincode,
    Guid? CountryId,
    Guid? StateId,
    Guid? CityId,
    string? Gstin,
    string? Pan);

public record StoreDto(
    Guid Id,
    string Name,
    string OwnerName,
    string Email,
    string Phone,
    string? AddressLine,
    string? Pincode,
    string? CountryName,
    string? StateName,
    string? CityName,
    string? Gstin,
    string? Pan,
    StoreStatus Status,
    DateTime CreatedAt,
    DateTime? ApprovedAt,
    string? RejectionReason);

public record RejectStoreRequest(string Reason);

public record StoreDocumentDto(
    Guid Id,
    Guid StoreId,
    DocumentType DocumentType,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAt);
