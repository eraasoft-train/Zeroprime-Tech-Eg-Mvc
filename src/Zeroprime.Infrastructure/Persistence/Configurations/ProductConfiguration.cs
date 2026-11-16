using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeroprime.Domain.Entities;

namespace Zeroprime.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    private static readonly DateTime SeedTime = new(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(5000);
        builder.Property(p => p.Category).IsRequired().HasMaxLength(200);
        builder.Property(p => p.KeyFeatures).HasColumnType("text");
        builder.Property(p => p.TargetLocations).HasColumnType("text");
        builder.Property(p => p.ImageUrl).HasMaxLength(500);
        builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.DisplayOrder).IsRequired().HasDefaultValue(0);
        builder.Property(p => p.CreatedAt).IsRequired();

        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.DisplayOrder);
        builder.HasIndex(p => p.Category);

        // Seed data sourced from ZEROPRIME_COMPANY_RESEARCH.md (public sources, see docs/research).
        builder.HasData(new Product
        {
            Id = 1,
            Name = "CanBank",
            Description = "CanBank is a reverse vending machine (RVM) that motivates people to recycle by exchanging used beverage containers, such as aluminum cans and plastic bottles, for instant rewards such as mobile phone credit or promotional codes. Users can also choose to donate their rewards to charitable organizations. For B2B partners, CanBank doubles as a highly visible promotional platform. Machines are designed for deployment in universities, fitness clubs, shopping malls, supermarkets and other high-traffic public spaces. Collected materials are subsequently recycled, closing the sustainability loop. CanBank represents the first component of a comprehensive waste collection system being developed by ZeroPrime Technologies.",
            Category = "Reverse Vending Machine",
            KeyFeatures = "[\"Instant mobile phone credit rewards\",\"Promotional codes\",\"Optional charity donations\",\"B2B promotional platform\",\"Accepts aluminum cans and plastic bottles\"]",
            TargetLocations = "[\"Universities\",\"Fitness clubs\",\"Shopping malls\",\"Supermarkets\",\"Public spaces\"]",
            ImageUrl = "/images/canbank.svg",
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = SeedTime
        });
    }
}
