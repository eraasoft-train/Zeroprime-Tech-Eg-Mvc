using Microsoft.AspNetCore.Mvc;
using Zeroprime.Application.Features.Company;
using Zeroprime.Web.ViewModels.About;

namespace Zeroprime.Web.Controllers;

public class AboutController : Controller
{
    private readonly ICompanyProfileService _companyProfiles;

    public AboutController(ICompanyProfileService companyProfiles)
    {
        _companyProfiles = companyProfiles;
    }

    [HttpGet("about")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new AboutViewModel(await _companyProfiles.GetProfileAsync(cancellationToken));
        return View(model);
    }
}
