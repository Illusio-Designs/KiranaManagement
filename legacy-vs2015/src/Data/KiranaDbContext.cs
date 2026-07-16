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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariant>().Property(v => v.Mrp).HasPrecision(18, 2);
            modelBuilder.Entity<ProductVariant>().Property(v => v.SellingPrice).HasPrecision(18, 2);
            modelBuilder.Entity<ProductVariant>().Property(v => v.TaxRatePercent).HasPrecision(5, 2);
            modelBuilder.Entity<ProductVariant>().Property(v => v.PackSize).HasPrecision(12, 3);
            base.OnModelCreating(modelBuilder);
        }

        // Seeds a platform SuperAdmin on first run. Call from Global.asax.
        public static void Seed()
        {
            using (var db = new KiranaDbContext())
            {
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
            }
        }
    }
}
