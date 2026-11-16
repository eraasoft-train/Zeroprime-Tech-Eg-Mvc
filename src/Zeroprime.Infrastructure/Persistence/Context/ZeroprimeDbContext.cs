using Microsoft.EntityFrameworkCore;
using Zeroprime.Domain.Entities;

namespace Zeroprime.Infrastructure.Persistence.Context;

public class ZeroprimeDbContext : DbContext
{
    public DbSet<CompanyProfile> CompanyProfiles { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ContactInquiry> ContactInquiries { get; set; } = null!;
    public DbSet<SocialMediaLink> SocialMediaLinks { get; set; } = null!;
    public DbSet<SiteSetting> SiteSettings { get; set; } = null!;

    public ZeroprimeDbContext(DbContextOptions<ZeroprimeDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // All entity configs + seed data live in the Configuration classes
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ZeroprimeDbContext).Assembly);
    }
}
