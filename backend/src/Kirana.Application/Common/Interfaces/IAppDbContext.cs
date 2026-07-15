using Kirana.Domain.Catalog;
using Kirana.Domain.Identity;
using Kirana.Domain.Platform;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Common.Interfaces;

/// <summary>Abstraction over the EF Core DbContext so services stay persistence-agnostic.</summary>
public interface IAppDbContext
{
    DbSet<Store> Stores { get; }
    DbSet<User> Users { get; }
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
