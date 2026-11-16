using Microsoft.AspNetCore.Mvc;
using Zeroprime.Application.Features.Settings;

namespace Zeroprime.Web.ViewComponents;

public class SocialLinksViewComponent : ViewComponent
{
    private readonly ISocialMediaLinkService _links;
    private readonly ILogger<SocialLinksViewComponent> _logger;

    public SocialLinksViewComponent(ISocialMediaLinkService links, ILogger<SocialLinksViewComponent> logger)
    {
        _links = links;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            var links = await _links.GetActiveLinksAsync(HttpContext.RequestAborted);
            return View(links);
        }
        catch (Exception ex)
        {
            // Social icons must never break page rendering (e.g. database unreachable).
            _logger.LogWarning(ex, "Social links could not be loaded; rendering fallback list");
            return View(FallbackLinks.All);
        }
    }

    // Verified public profiles (docs/research/ZEROPRIME_COMPANY_RESEARCH.md).
    private static class FallbackLinks
    {
        public static readonly IReadOnlyList<SocialMediaLinkDto> All =
        [
            new("LinkedIn", "https://www.linkedin.com/company/zeroprime-technologies", "bi-linkedin", 1),
            new("Instagram", "https://www.instagram.com/zeroprime.tech/", "bi-instagram", 2),
            new("Facebook", "https://www.facebook.com/zeroprime.tech", "bi-facebook", 3)
        ];
    }
}
