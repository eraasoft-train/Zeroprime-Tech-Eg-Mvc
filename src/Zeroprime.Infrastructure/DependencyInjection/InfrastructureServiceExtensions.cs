using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zeroprime.Application.Abstractions;
using Zeroprime.Infrastructure.Persistence.Context;
using Zeroprime.Infrastructure.Persistence.Repositories;

namespace Zeroprime.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    private const string ConnectionStringName = "Postgres";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        // Resolve the connection string lazily through DI at DbContext creation time.
        // Host/application configuration callbacks (e.g. test factories, hosted environments)
        // may still be layered on top of the configuration captured at registration.
        services.AddDbContext<ZeroprimeDbContext>((serviceProvider, options) =>
        {
            var effectiveConfiguration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = effectiveConfiguration.GetConnectionString(ConnectionStringName)
                ?? throw new InvalidOperationException(
                    $"Connection string '{ConnectionStringName}' is not configured. " +
                    "Set it via the ConnectionStrings__Postgres environment variable or appsettings.json.");

            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsAssembly("Zeroprime.Infrastructure"));
        });

        services.AddScoped<ICompanyProfileRepository, CompanyProfileRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IContactInquiryRepository, ContactInquiryRepository>();
        services.AddScoped<ISocialMediaLinkRepository, SocialMediaLinkRepository>();
        services.AddScoped<ISiteSettingRepository, SiteSettingRepository>();

        return services;
    }
}
