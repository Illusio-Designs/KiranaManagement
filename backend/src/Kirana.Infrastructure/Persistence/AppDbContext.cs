using System.Reflection;
using Kirana.Application.Common.Interfaces;
using Kirana.Domain.Catalog;
using Kirana.Domain.Common;
using Kirana.Domain.Customers;
using Kirana.Domain.Geo;
using Kirana.Domain.Identity;
using Kirana.Domain.Inventory;
using Kirana.Domain.Orders;
using Kirana.Domain.Platform;
using Kirana.Domain.Purchasing;
using Kirana.Domain.Sales;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly Guid _tenantId;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenant tenant)
        : base(options)
    {
        _tenantId = tenant.StoreId;
    }

    public DbSet<Store> Stores => Set<Store>();
    public DbSet<StoreDocument> StoreDocuments => Set<StoreDocument>();
    public DbSet<User> Users => Set<User>();

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<StockLedgerEntry> StockLedger => Set<StockLedgerEntry>();

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<State> States => Set<State>();
    public DbSet<City> Cities => Set<City>();

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    public DbSet<SalesInvoice> SalesInvoices => Set<SalesInvoice>();
    public DbSet<SalesLine> SalesLines => Set<SalesLine>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply per-entity mapping from this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global multi-tenant filter: every ITenantEntity is scoped to the
        // current tenant automatically, so a store can never read another
        // store's rows. Platform entities (Store, User) are NOT filtered.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(ApplyTenantFilter), BindingFlags.Instance | BindingFlags.NonPublic)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    private void ApplyTenantFilter<TEntity>(ModelBuilder builder)
        where TEntity : class, ITenantEntity
    {
        // References the context field _tenantId, which EF re-evaluates per query.
        builder.Entity<TEntity>().HasQueryFilter(e => e.StoreId == _tenantId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditAndTenant();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditAndTenant();
        return base.SaveChanges();
    }

    private void ApplyAuditAndTenant()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    // Stamp tenant on new tenant-owned rows if the caller didn't set it.
                    if (entry.Entity is ITenantEntity tenantAdded &&
                        tenantAdded.StoreId == Guid.Empty && _tenantId != Guid.Empty)
                    {
                        tenantAdded.StoreId = _tenantId;
                    }
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }
}
