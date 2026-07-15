using System.Security.Claims;
using System.Text;
using Kirana.Api.Middleware;
using Kirana.Api.Services;
using Kirana.Application;
using Kirana.Application.Common.Interfaces;
using Kirana.Infrastructure;
using Kirana.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Services -------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "KiranaManagement API", Version = "v1" });
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the JWT from /api/auth/login (no 'Bearer' prefix needed).",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    options.AddSecurityDefinition("Bearer", scheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = Array.Empty<string>() });
});

// Current user/tenant resolved from the JWT (one instance serves both interfaces).
builder.Services.AddScoped<CurrentUser>();
builder.Services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUser>());
builder.Services.AddScoped<ICurrentTenant>(sp => sp.GetRequiredService<CurrentUser>());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// --- Auth -----------------------------------------------------------------
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"]
    ?? throw new InvalidOperationException("Missing configuration 'Jwt:Key'.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false; // keep 'sub', 'store_id' claim names as-issued
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = "sub"
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// --- Pipeline -------------------------------------------------------------
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await BootstrapDatabaseAsync(app);
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Dev-only bootstrap: create the schema (EnsureCreated) and seed the SuperAdmin.
// Production should switch to EF migrations (see docs/DEVELOPMENT.md §7).
static async Task BootstrapDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        var hasher = services.GetRequiredService<IPasswordHasher>();
        var cfg = services.GetRequiredService<IConfiguration>();
        var adminEmail = cfg["Seed:AdminEmail"] ?? "superadmin@kirana.local";
        var adminPassword = cfg["Seed:AdminPassword"] ?? "Admin@12345";
        await SeedData.SeedAsync(db, hasher, adminEmail, adminPassword);
        logger.LogInformation("Database ready; SuperAdmin seeded ({Email}).", adminEmail);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Database bootstrap/seed skipped — is MySQL running and the connection string correct?");
    }
}

// Exposed so integration tests can reference the entry-point type.
public partial class Program { }
