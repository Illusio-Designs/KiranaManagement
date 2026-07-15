using Kirana.Domain.Common.Enums;

namespace Kirana.Application.Stores.Dtos;

/// <summary>Public self-registration payload (PRD FR-1.1).</summary>
public record RegisterStoreRequest(
    string StoreName,
    string OwnerName,
    string Email,
    string Phone,
    string Password,
    string? AddressLine,
    string? City,
    string? Pincode,
    string? Gstin);

public record StoreDto(
    Guid Id,
    string Name,
    string OwnerName,
    string Email,
    string Phone,
    string? City,
    string? Gstin,
    StoreStatus Status,
    DateTime CreatedAt,
    DateTime? ApprovedAt,
    string? RejectionReason);

public record RejectStoreRequest(string Reason);
