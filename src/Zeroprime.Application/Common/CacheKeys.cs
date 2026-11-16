namespace Zeroprime.Application.Common;

/// <summary>
/// Shared in-memory cache keys for read-mostly brochure content.
/// </summary>
public static class CacheKeys
{
    public const string CompanyProfile = "zeroprime:company-profile";
    public const string ActiveProducts = "zeroprime:products:active";
    public const string SiteSettings = "zeroprime:site-settings";
    public const string SocialLinks = "zeroprime:social-links";

    public static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(5);
}
