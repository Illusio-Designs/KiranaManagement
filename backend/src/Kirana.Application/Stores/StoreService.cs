using Kirana.Application.Common.Exceptions;
using Kirana.Application.Common.Interfaces;
using Kirana.Application.Stores.Dtos;
using Kirana.Domain.Common.Enums;
using Kirana.Domain.Identity;
using Kirana.Domain.Platform;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Stores;

public class StoreService : IStoreService
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public StoreService(IAppDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<StoreDto> RegisterAsync(RegisterStoreRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var emailTaken = await _db.Users.AnyAsync(u => u.Email == email, ct);
        if (emailTaken)
            throw AppException.Conflict("An account with this email already exists.");

        var store = new Store
        {
            Name = request.StoreName.Trim(),
            OwnerName = request.OwnerName.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            AddressLine = request.AddressLine?.Trim(),
            City = request.City?.Trim(),
            Pincode = request.Pincode?.Trim(),
            Gstin = request.Gstin?.Trim(),
            Status = StoreStatus.Pending
        };

        var owner = new User
        {
            Email = email,
            FullName = request.OwnerName.Trim(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = UserRole.Owner,
            StoreId = store.Id,
            IsActive = true
        };

        _db.Stores.Add(store);
        _db.Users.Add(owner);
        await _db.SaveChangesAsync(ct);

        return Map(store);
    }

    public async Task<StoreDto> GetByIdAsync(Guid storeId, CancellationToken ct = default)
    {
        var store = await _db.Stores.FirstOrDefaultAsync(s => s.Id == storeId, ct)
                    ?? throw AppException.NotFound("Store not found.");
        return Map(store);
    }

    public async Task<IReadOnlyList<StoreDto>> GetPendingAsync(CancellationToken ct = default)
    {
        var stores = await _db.Stores
            .Where(s => s.Status == StoreStatus.Pending)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync(ct);
        return stores.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<StoreDto>> GetAllAsync(CancellationToken ct = default)
    {
        var stores = await _db.Stores.OrderByDescending(s => s.CreatedAt).ToListAsync(ct);
        return stores.Select(Map).ToList();
    }

    public async Task<StoreDto> ApproveAsync(Guid storeId, CancellationToken ct = default)
    {
        var store = await _db.Stores.FirstOrDefaultAsync(s => s.Id == storeId, ct)
                    ?? throw AppException.NotFound("Store not found.");

        if (store.Status != StoreStatus.Pending)
            throw new AppException($"Only pending stores can be approved (current: {store.Status}).");

        store.Status = StoreStatus.Active;
        store.ApprovedAt = DateTime.UtcNow;
        store.RejectionReason = null;
        await _db.SaveChangesAsync(ct);

        return Map(store);
    }

    public async Task<StoreDto> RejectAsync(Guid storeId, string reason, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new AppException("A rejection reason is required.");

        var store = await _db.Stores.FirstOrDefaultAsync(s => s.Id == storeId, ct)
                    ?? throw AppException.NotFound("Store not found.");

        if (store.Status != StoreStatus.Pending)
            throw new AppException($"Only pending stores can be rejected (current: {store.Status}).");

        store.Status = StoreStatus.Rejected;
        store.RejectionReason = reason.Trim();
        await _db.SaveChangesAsync(ct);

        return Map(store);
    }

    private static StoreDto Map(Store s) => new(
        s.Id, s.Name, s.OwnerName, s.Email, s.Phone, s.City, s.Gstin,
        s.Status, s.CreatedAt, s.ApprovedAt, s.RejectionReason);
}
