using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Zeroprime.Infrastructure.Persistence.Context;

/// <summary>
/// Used by EF Core tooling (<c>dotnet ef</c>) so migrations can be created and applied
/// without booting the web application or requiring a reachable database at design time.
/// The connection string is only used for commands that actually talk to a server
/// (e.g. <c>dotnet ef database update</c>).
/// </summary>
public sealed class ZeroprimeDbContextFactory : IDesignTimeDbContextFactory<ZeroprimeDbContext>
{
    public ZeroprimeDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? "Host=localhost;Port=5432;Database=zeroprime;Username=zeroprime;Password=zeroprime_dev_only";

        var options = new DbContextOptionsBuilder<ZeroprimeDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly("Zeroprime.Infrastructure"))
            .Options;

        return new ZeroprimeDbContext(options);
    }
}
