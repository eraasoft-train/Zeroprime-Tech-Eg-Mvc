using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeroprime.Domain.Entities;

namespace Zeroprime.Infrastructure.Persistence.Configurations;

public class SocialMediaLinkConfiguration : IEntityTypeConfiguration<SocialMediaLink>
{
    private static readonly DateTime _seed = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<SocialMediaLink> builder)
    {
        builder.ToTable("SocialMediaLinks");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Platform).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Url).IsRequired().HasMaxLength(500);
        builder.Property(s => s.Icon).HasMaxLength(100);
        builder.Property(s => s.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(s => s.DisplayOrder).IsRequired().HasDefaultValue(0);
        builder.Property(s => s.CreatedAt).IsRequired();

        builder.HasIndex(s => s.IsActive);
        builder.HasIndex(s => s.DisplayOrder);

        builder.HasData(
            new SocialMediaLink { Id = 1, Platform = "LinkedIn", Url = "https://www.linkedin.com/company/zeroprime-technologies", Icon = "bi-linkedin", IsActive = true, DisplayOrder = 1, CreatedAt = _seed },
            new SocialMediaLink { Id = 2, Platform = "Instagram", Url = "https://www.instagram.com/zeroprime.tech/", Icon = "bi-instagram", IsActive = true, DisplayOrder = 2, CreatedAt = _seed },
            new SocialMediaLink { Id = 3, Platform = "Facebook", Url = "https://www.facebook.com/zeroprime.tech", Icon = "bi-facebook", IsActive = true, DisplayOrder = 3, CreatedAt = _seed }
        );
    }
}
