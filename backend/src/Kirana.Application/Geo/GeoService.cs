using Kirana.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Geo;

public class GeoService : IGeoService
{
    private readonly IAppDbContext _db;

    public GeoService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CountryDto>> GetCountriesAsync(CancellationToken ct = default)
    {
        return await _db.Countries
            .OrderBy(c => c.Name)
            .Select(c => new CountryDto(c.Id, c.Name, c.Iso2, c.PhoneCode))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<StateDto>> GetStatesAsync(Guid countryId, CancellationToken ct = default)
    {
        return await _db.States
            .Where(s => s.CountryId == countryId)
            .OrderBy(s => s.Name)
            .Select(s => new StateDto(s.Id, s.Name, s.Code))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CityDto>> GetCitiesAsync(Guid stateId, CancellationToken ct = default)
    {
        return await _db.Cities
            .Where(c => c.StateId == stateId)
            .OrderBy(c => c.Name)
            .Select(c => new CityDto(c.Id, c.Name))
            .ToListAsync(ct);
    }
}
