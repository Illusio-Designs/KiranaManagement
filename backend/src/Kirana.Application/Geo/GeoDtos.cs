namespace Kirana.Application.Geo;

public record CountryDto(Guid Id, string Name, string Iso2, string PhoneCode);
public record StateDto(Guid Id, string Name, string? Code);
public record CityDto(Guid Id, string Name);
