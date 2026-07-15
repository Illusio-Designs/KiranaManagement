using Kirana.Domain.Geo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kirana.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("countries");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Iso2).IsRequired().HasMaxLength(2);
        builder.Property(c => c.PhoneCode).HasMaxLength(8);
        builder.HasMany(c => c.States).WithOne(s => s.Country!).HasForeignKey(s => s.CountryId);
    }
}

public class StateConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.ToTable("states");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(120);
        builder.Property(s => s.Code).HasMaxLength(8);
        builder.HasIndex(s => s.CountryId);
        builder.HasMany(s => s.Cities).WithOne(c => c.State!).HasForeignKey(c => c.StateId);
    }
}

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("cities");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(120);
        builder.HasIndex(c => c.StateId);
    }
}
