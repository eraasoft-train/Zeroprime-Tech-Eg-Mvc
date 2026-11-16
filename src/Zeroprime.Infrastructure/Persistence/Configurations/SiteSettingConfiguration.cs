using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeroprime.Domain.Entities;

namespace Zeroprime.Infrastructure.Persistence.Configurations;

public class SiteSettingConfiguration : IEntityTypeConfiguration<SiteSetting>
{
    private static readonly DateTime _seed = new(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<SiteSetting> builder)
    {
        builder.ToTable("SiteSettings");
        builder.HasKey(s => s.Key);

        builder.Property(s => s.Key).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Value).IsRequired().HasMaxLength(2000);
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.UpdatedAt).IsRequired();

        builder.HasIndex(s => s.Key).IsUnique();

        builder.HasData(
            new SiteSetting { Key = "SiteName", Value = "ZeroPrime Technologies", Description = "Website name displayed in browser tab", UpdatedAt = _seed },
            new SiteSetting { Key = "SiteDescription", Value = "Smart Sustainable Cities Infrastructure", Description = "Meta description used for SEO", UpdatedAt = _seed },
            new SiteSetting { Key = "ContactEmail", Value = "zeroprime.tech@gmail.com", Description = "Primary contact email shown on website", UpdatedAt = _seed },
            new SiteSetting { Key = "HeroTitle", Value = "Building Smarter, Cleaner Cities", Description = "Hero section main heading", UpdatedAt = _seed },
            new SiteSetting { Key = "HeroSubtitle", Value = "Autonomous waste management solutions that reward recycling and power a sustainable future.", Description = "Hero section subtitle", UpdatedAt = _seed }
        );
    }
}
