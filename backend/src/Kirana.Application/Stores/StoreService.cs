using System.Text.RegularExpressions;
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
    private static readonly Regex PanRegex = new("^[A-Z]{5}[0-9]{4}[A-Z]$", RegexOptions.Compiled);

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

        var pan = request.Pan?.Trim().ToUpperInvariant();
        if (!string.IsNullOrEmpty(pan) && !PanRegex.IsMatch(pan))
            throw new AppException("PAN must be 10 characters in the format AAAAA9999A.");

        var store = new Store
        {
            Name = request.StoreName.Trim(),
            OwnerName = request.OwnerName.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            AddressLine = request.AddressLine?.Trim(),
            Pincode = request.Pincode?.Trim(),
            Gstin = request.Gstin?.Trim().ToUpperInvariant(),
            Pan = pan,
            Status = StoreStatus.Pending
        };

        await ResolveLocationAsync(store, request, ct);

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

    private async Task ResolveLocationAsync(Store store, RegisterStoreRequest request, CancellationToken ct)
    {
        if (request.CountryId is Guid countryId)
        {
            var country = await _db.Countries.FirstOrDefaultAsync(c => c.Id == countryId, ct)
                          ?? throw AppException.NotFound("Selected country not found.");
            store.CountryId = country.Id;
            store.CountryName = country.Name;
        }

        if (request.StateId is Guid stateId)
        {
            var state = await _db.States.FirstOrDefaultAsync(s => s.Id == stateId, ct)
                        ?? throw AppException.NotFound("Selected state not found.");
            store.StateId = state.Id;
            store.StateName = state.Name;
        }

        if (request.CityId is Guid cityId)
        {
            var city = await _db.Cities.FirstOrDefaultAsync(c => c.Id == cityId, ct)
                       ?? throw AppException.NotFound("Selected city not found.");
            store.CityId = city.Id;
            store.CityName = city.Name;
        }
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
        s.Id, s.Name, s.OwnerName, s.Email, s.Phone,
        s.AddressLine, s.Pincode, s.CountryName, s.StateName, s.CityName,
        s.Gstin, s.Pan, s.Status, s.CreatedAt, s.ApprovedAt, s.RejectionReason);
}
