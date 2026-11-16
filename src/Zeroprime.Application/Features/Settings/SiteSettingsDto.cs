namespace Zeroprime.Application.Features.Settings;

/// <summary>
/// Well-known site settings keys with safe fallbacks baked in so the site
/// renders sensibly even before the database is seeded.
/// </summary>
public sealed record SiteSettingsDto(
    string SiteName,
    string SiteDescription,
    string HeroTitle,
    string HeroSubtitle)
{
    public static SiteSettingsDto Fallback { get; } = new(
        "ZeroPrime Technologies",
        "Smart sustainable cities infrastructure and autonomous waste management solutions from Egypt.",
        "Building Smarter, Cleaner Cities",
        "Autonomous waste management solutions that reward recycling and power a sustainable future.");
}
