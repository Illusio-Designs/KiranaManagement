using System.Data.Entity;
using System.Linq;
using Kirana.WebApi.Models;

namespace Kirana.WebApi.Data
{
    // EF6 Code First context. Connection string name "KiranaDb" (see web.config).
    public class KiranaDbContext : DbContext
    {
        public KiranaDbContext() : base("name=KiranaDb")
        {
        }

        public DbSet<Store> Stores { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesLine> SalesLines { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariant>().Property(v => v.Mrp).HasPrecision(18, 2);
            modelBuilder.Entity<ProductVariant>().Property(v => v.SellingPrice).HasPrecision(18, 2);
            modelBuilder.Entity<ProductVariant>().Property(v => v.TaxRatePercent).HasPrecision(5, 2);
            modelBuilder.Entity<ProductVariant>().Property(v => v.PackSize).HasPrecision(12, 3);
            base.OnModelCreating(modelBuilder);
        }

        // Helper: build a single-variant product for the marketplace seed.
        private static Product MakeProduct(System.Guid storeId, string name, string brand,
            string category, string variantName, string sku, UnitOfMeasure unit,
            decimal packSize, decimal mrp, decimal price, int stock)
        {
            var p = new Product { StoreId = storeId, Name = name, Brand = brand, Category = category };
            p.Variants.Add(new ProductVariant
            {
                StoreId = storeId, Name = variantName, Sku = sku,
                Unit = unit, PackSize = packSize, Mrp = mrp, SellingPrice = price,
                TaxRatePercent = 5m, StockQuantity = stock, ReorderLevel = 10
            });
            return p;
        }

        // Seeds a SuperAdmin + a ready-to-use sample store on first run.
        // Called from Global.asax; runs automatically every startup (safe — it
        // only inserts when the rows are missing).
        public static void Seed()
        {
            using (var db = new KiranaDbContext())
            {
                // 1) Platform Super Admin
                if (!db.Users.Any(u => u.Role == UserRole.SuperAdmin))
                {
                    db.Users.Add(new User
                    {
                        Email = "superadmin@kirana.local",
                        FullName = "Platform Super Admin",
                        Role = UserRole.SuperAdmin,
                        PasswordHash = PasswordHasher.Hash("Admin@12345"),
                        IsActive = true
                    });
                    db.SaveChanges();
                }

                // 2) Sample approved store + owner + a product with variants,
                //    so there's data to explore immediately.
                if (!db.Stores.Any())
                {
                    var store = new Store
                    {
                        Name = "Demo Kirana Store",
                        OwnerName = "Demo Owner",
                        Email = "demo@store.local",
                        Phone = "+919999999999",
                        City = "Ahmedabad",
                        Gstin = "24ABCDE1234F1Z5",
                        Pan = "ABCDE1234F",
                        Status = StoreStatus.Active,          // already approved
                        ApprovedAt = System.DateTime.UtcNow,
                        Latitude = 23.0225,                   // Ahmedabad
                        Longitude = 72.5714
                    };
                    db.Stores.Add(store);

                    db.Users.Add(new User
                    {
                        Email = "demo@store.local",
                        FullName = "Demo Owner",
                        Role = UserRole.Owner,
                        StoreId = store.Id,
                        PasswordHash = PasswordHasher.Hash("Demo@12345"),
                        IsActive = true
                    });

                    var product = new Product
                    {
                        StoreId = store.Id,
                        Name = "Aashirvaad Atta",
                        Brand = "Aashirvaad",
                        Category = "Pulses & Grains"
                    };
                    product.Variants.Add(new ProductVariant
                    {
                        StoreId = store.Id, Name = "1 kg", Sku = "ATTA-1KG",
                        Unit = UnitOfMeasure.Kilogram, PackSize = 1m,
                        Mrp = 60m, SellingPrice = 55m, TaxRatePercent = 5m,
                        StockQuantity = 100, ReorderLevel = 10
                    });
                    product.Variants.Add(new ProductVariant
                    {
                        StoreId = store.Id, Name = "5 kg", Sku = "ATTA-5KG",
                        Unit = UnitOfMeasure.Kilogram, PackSize = 5m,
                        Mrp = 280m, SellingPrice = 260m, TaxRatePercent = 5m,
                        StockQuantity = 40, ReorderLevel = 5
                    });
                    db.Products.Add(product);

                    // A few more products across categories so consumer
                    // search / category filter has something to show.
                    db.Products.Add(MakeProduct(store.Id, "Fresh Apples", "Farm", "Fruits",
                        "1 kg", "APPLE-1KG", UnitOfMeasure.Kilogram, 1m, 159m, 119m, 80));
                    db.Products.Add(MakeProduct(store.Id, "Amul Milk", "Amul", "Dairy & Eggs",
                        "1 L", "MILK-1L", UnitOfMeasure.Litre, 1m, 62m, 54m, 60));
                    db.Products.Add(MakeProduct(store.Id, "Tata Salt", "Tata", "Cooking Essentials",
                        "1 kg", "SALT-1KG", UnitOfMeasure.Kilogram, 1m, 28m, 25m, 120));
                    db.Products.Add(MakeProduct(store.Id, "Lay's Chips", "Lay's", "Snacks",
                        "52 g", "LAYS-52G", UnitOfMeasure.Gram, 52m, 20m, 18m, 200));
                    db.Products.Add(MakeProduct(store.Id, "Fresh Tomatoes", "Farm", "Vegetables",
                        "1 kg", "TOM-1KG", UnitOfMeasure.Kilogram, 1m, 59m, 39m, 90));

                    db.SaveChanges();
                }
            }
        }
    }
}
