using Kirana.Application.Common.Interfaces;
using Kirana.Infrastructure.Identity;
using Kirana.Infrastructure.Integrations.Otp;
using Kirana.Infrastructure.Persistence;
using Kirana.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kirana.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<GoogleAuthOptions>(configuration.GetSection(GoogleAuthOptions.SectionName));

        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing connection string 'Default'.");

        // Fixed server version avoids a live DB connection at startup/design time.
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IFileStorage, LocalFileStorage>();
        services.AddSingleton<IGoogleTokenValidator, GoogleTokenValidator>();

        // OTP delivery: dev logs the code; swap for a real SMS provider in production.
        services.AddSingleton<IOtpSender, DevOtpSender>();

        return services;
    }
}
