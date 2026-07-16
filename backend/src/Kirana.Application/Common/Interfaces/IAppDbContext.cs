using Kirana.Domain.Catalog;
using Kirana.Domain.Customers;
using Kirana.Domain.Geo;
using Kirana.Domain.Identity;
using Kirana.Domain.Inventory;
using Kirana.Domain.Orders;
using Kirana.Domain.Platform;
using Kirana.Domain.Purchasing;
using Kirana.Domain.Sales;
using Microsoft.EntityFrameworkCore;

namespace Kirana.Application.Common.Interfaces;

/// <summary>Abstraction over the EF Core DbContext so services stay persistence-agnostic.</summary>
public interface IAppDbContext
{
    DbSet<Store> Stores { get; }
    DbSet<StoreDocument> StoreDocuments { get; }
    DbSet<User> Users { get; }

    // Catalog & inventory (tenant-scoped)
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<StockLedgerEntry> StockLedger { get; }

    // Geo reference data (country → state → city cascade)
    DbSet<Country> Countries { get; }
    DbSet<State> States { get; }
    DbSet<City> Cities { get; }

    // Marketplace customers + OTP
    DbSet<Customer> Customers { get; }
    DbSet<OtpCode> OtpCodes { get; }

    // Marketplace cart (platform-level)
    DbSet<Cart> Carts { get; }
    DbSet<CartItem> CartItems { get; }

    // Sales / POS (tenant-scoped)
    DbSet<SalesInvoice> SalesInvoices { get; }
    DbSet<SalesLine> SalesLines { get; }

    // Purchasing (tenant-scoped)
    DbSet<Supplier> Suppliers { get; }
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseOrderLine> PurchaseOrderLines { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
