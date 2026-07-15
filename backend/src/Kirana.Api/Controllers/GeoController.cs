using Kirana.Application.Geo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kirana.Api.Controllers;

/// <summary>
/// Public reference data for the country → state → city cascade used on store
/// registration (pick a country → load its states → load that state's cities).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class GeoController : ControllerBase
{
    private readonly IGeoService _geo;

    public GeoController(IGeoService geo)
    {
        _geo = geo;
    }

    [HttpGet("countries")]
    public async Task<ActionResult<IReadOnlyList<CountryDto>>> Countries(CancellationToken ct)
        => Ok(await _geo.GetCountriesAsync(ct));

    [HttpGet("countries/{countryId:guid}/states")]
    public async Task<ActionResult<IReadOnlyList<StateDto>>> States(Guid countryId, CancellationToken ct)
        => Ok(await _geo.GetStatesAsync(countryId, ct));

    [HttpGet("states/{stateId:guid}/cities")]
    public async Task<ActionResult<IReadOnlyList<CityDto>>> Cities(Guid stateId, CancellationToken ct)
        => Ok(await _geo.GetCitiesAsync(stateId, ct));
}
