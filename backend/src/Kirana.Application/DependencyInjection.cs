using Kirana.Application.Auth;
using Kirana.Application.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Kirana.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
