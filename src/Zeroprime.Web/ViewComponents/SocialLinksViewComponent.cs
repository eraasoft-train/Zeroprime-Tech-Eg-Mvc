using Microsoft.AspNetCore.Mvc;
using Zeroprime.Application.Features.Settings;

namespace Zeroprime.Web.ViewComponents;

public class SocialLinksViewComponent : ViewComponent
{
    private readonly ISocialMediaLinkService _links;

    public SocialLinksViewComponent(ISocialMediaLinkService links) => _links = links;

    public async Task<IViewComponentResult> InvokeAsync() =>
        View(await _links.GetActiveLinksAsync(HttpContext.RequestAborted));
}
