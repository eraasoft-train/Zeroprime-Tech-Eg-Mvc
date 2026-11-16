using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Zeroprime.Infrastructure.Persistence.Context;

namespace Zeroprime.IntegrationTests;

/// <summary>
/// Boots the real MVC application against a disposable PostgreSQL container
/// (Testcontainers) so integration tests exercise the production data path,
/// migrations and seed data. Mirrors docker-compose for local development.
/// </summary>
public sealed class ZeroprimeWebFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("zeroprime_tests")
        .WithUsername("zeroprime")
        .WithPassword("zeroprime_test_only")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres"] = _postgres.GetConnectionString(),
                ["Database:MigrateOnStartup"] = "false"
            });
        });
    }

    public ZeroprimeDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ZeroprimeDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options);

    public ZeroprimeWebFactory()
    {
        _postgres.StartAsync().GetAwaiter().GetResult();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ZeroprimeDbContext>();
        db.Database.Migrate();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _postgres.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }
}
