namespace Kirana.Application.Geo;

public interface IGeoService
{
    Task<IReadOnlyList<CountryDto>> GetCountriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<StateDto>> GetStatesAsync(Guid countryId, CancellationToken ct = default);
    Task<IReadOnlyList<CityDto>> GetCitiesAsync(Guid stateId, CancellationToken ct = default);
}
