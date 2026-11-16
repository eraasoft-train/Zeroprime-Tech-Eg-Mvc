using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Zeroprime.Application.DependencyInjection;
using Zeroprime.Infrastructure.DependencyInjection;
using Zeroprime.Infrastructure.Persistence.Context;
using Zeroprime.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------- Logging
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProcessId()
    .WriteTo.Console());

// ------------------------------------------------------------------- MVC
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

// -------------------------------------------------------- Infrastructure
// EF Core (PostgreSQL) + repositories. Throws at startup if the connection
// string is missing - fail fast on misconfiguration.
builder.Services.AddInfrastructure();

// ------------------------------------------------------------- Application
builder.Services.AddApplicationServices();

// ---------------------------------------------------------- Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ZeroprimeDbContext>("postgres", tags: ["db", "ready"]);

// ----------------------------------------------------------- Rate limiting
// Protects the public contact form against spam and abuse.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("contact", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromMinutes(10),
                PermitLimit = 5,
                QueueLimit = 0
            }));
});

var app = builder.Build();

// ------------------------------------------- Migrate & seed on startup
// Controlled by the "Database:MigrateOnStartup" setting (default: true).
// Failures are logged but do not prevent the app from starting; /health
// reports database readiness.
var migrateOnStartup = builder.Configuration.GetValue("Database:MigrateOnStartup", true);
if (migrateOnStartup)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ZeroprimeDbContext>();
        logger.LogInformation("Applying database migrations...");
        await db.Database.MigrateAsync();
        logger.LogInformation("Database is up to date.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Database migration failed on startup. The site will render its error page until the database is reachable.");
    }
}

// --------------------------------------------------------------- Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSerilogRequestLogging();
app.UseSecurityHeaders();

app.UseStatusCodePagesWithReExecute("/Home/StatusCode/{0}");

app.UseHttpsRedirection();

// Static assets: long-lived caching (file names are versioned via asp-append-version).
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        var path = context.File.Name;
        if (path.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".webp", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase))
        {
            context.Context.Response.Headers.CacheControl = "public, max-age=604800";
        }
    }
});

app.UseRouting();
app.UseRateLimiter();
app.UseAuthorization();

// SEO-friendly slug routes are attribute-routed on controllers.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHealthChecks("/health");

app.Run();

public partial class Program;
