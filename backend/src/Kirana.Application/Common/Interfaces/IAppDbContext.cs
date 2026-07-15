using Kirana.Domain.Catalog;
using Kirana.Domain.Customers;
using Kirana.Domain.Geo;
using Kirana.Domain.Identity;
using Kirana.Domain.Platform;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Common.Interfaces;

/// <summary>Abstraction over the EF Core DbContext so services stay persistence-agnostic.</summary>
public interface IAppDbContext
{
    DbSet<Store> Stores { get; }
    DbSet<StoreDocument> StoreDocuments { get; }
    DbSet<User> Users { get; }
    DbSet<Product> Products { get; }

    // Geo reference data (country → state → city cascade)
    DbSet<Country> Countries { get; }
    DbSet<State> States { get; }
    DbSet<City> Cities { get; }

    // Marketplace customers + OTP
    DbSet<Customer> Customers { get; }
    DbSet<OtpCode> OtpCodes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
