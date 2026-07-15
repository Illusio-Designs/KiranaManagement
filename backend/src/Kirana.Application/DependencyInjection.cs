using Kirana.Application.Auth;
using Kirana.Application.Customers;
using Kirana.Application.Geo;
using Kirana.Application.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Kirana.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IStoreDocumentService, StoreDocumentService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGeoService, GeoService>();
        services.AddScoped<ICustomerAuthService, CustomerAuthService>();
        return services;
    }
}
