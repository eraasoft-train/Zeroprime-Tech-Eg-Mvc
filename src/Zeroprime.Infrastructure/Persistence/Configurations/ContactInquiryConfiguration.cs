using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zeroprime.Domain.Entities;
using Zeroprime.Domain.Enums;

namespace Zeroprime.Infrastructure.Persistence.Configurations;

public class ContactInquiryConfiguration : IEntityTypeConfiguration<ContactInquiry>
{
    public void Configure(EntityTypeBuilder<ContactInquiry> builder)
    {
        builder.ToTable("ContactInquiries");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(256);
        builder.Property(c => c.Company).HasMaxLength(200);
        builder.Property(c => c.Phone).HasMaxLength(50);
        builder.Property(c => c.Subject).IsRequired().HasMaxLength(500);
        builder.Property(c => c.Message).IsRequired().HasMaxLength(5000);
        builder.Property(c => c.Status).IsRequired().HasDefaultValue(InquiryStatus.New);
        builder.Property(c => c.SubmittedAt).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();

        builder.HasIndex(c => c.Email);
        builder.HasIndex(c => c.Status);
        builder.HasIndex(c => c.SubmittedAt);
    }
}
