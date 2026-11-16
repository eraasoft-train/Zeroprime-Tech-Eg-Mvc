using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeroprime.Domain.Entities;

namespace Zeroprime.Infrastructure.Persistence.Configurations;

public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
{
    private static readonly DateTime SeedTime = new(2026, 1, 16, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<CompanyProfile> builder)
    {
        builder.ToTable("CompanyProfiles");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.Mission).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.Vision).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.Industry).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Location).IsRequired().HasMaxLength(200);
        builder.Property(c => c.ContactEmail).IsRequired().HasMaxLength(256);
        builder.Property(c => c.FoundingYear).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.HasIndex(c => c.ContactEmail);

        // Seed data sourced from ZEROPRIME_COMPANY_RESEARCH.md (public sources, see docs/research).
        builder.HasData(new CompanyProfile
        {
            Id = 1,
            Name = "ZeroPrime Technologies",
            Description = "ZeroPrime Technologies is an Egyptian startup founded in 2017, specializing in smart sustainable cities infrastructure. The company concentrates on autonomous waste management solutions as part of its broader smart city vision.",
            Mission = "To design and implement smart city infrastructure that creates the urban environment we all want to live in, aligned with sustainable development goals.",
            Vision = "Sustainable cities powered by autonomous waste management, where recycling is accessible, rewarding and seamless for every citizen.",
            Industry = "Mechanical or Industrial Engineering / Smart Cities",
            Location = "Cairo, Egypt",
            ContactEmail = "zeroprime.tech@gmail.com",
            FoundingYear = 2017,
            CreatedAt = SeedTime
        });
    }
}
