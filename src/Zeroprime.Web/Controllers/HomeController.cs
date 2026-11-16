using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Zeroprime.Application.Features.Company;
using Zeroprime.Application.Features.Products;
using Zeroprime.Application.Features.Settings;
using Zeroprime.Web.Models;
using Zeroprime.Web.ViewModels;
using Zeroprime.Web.ViewModels.Home;

namespace Zeroprime.Web.Controllers;

public class HomeController : Controller
{
    private readonly ICompanyProfileService _companyProfiles;
    private readonly IProductService _products;
    private readonly ISiteSettingsService _settings;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ICompanyProfileService companyProfiles,
        IProductService products,
        ISiteSettingsService settings,
        ILogger<HomeController> logger)
    {
        _companyProfiles = companyProfiles;
        _products = products;
        _settings = settings;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new HomeViewModel(
            await _companyProfiles.GetProfileAsync(cancellationToken),
            await _products.GetActiveProductsAsync(cancellationToken),
            await _settings.GetSettingsAsync(cancellationToken));

        return View(model);
    }

    [HttpGet("privacy")]
    public IActionResult Privacy() => View();

    [HttpGet("terms")]
    public IActionResult Terms() => View();

    // Accepts ALL verbs: UseStatusCodePagesWithReExecute re-executes with the ORIGINAL
    // method (a POST that triggered a 4xx/5xx re-enters this action as POST).
    [Route("Home/StatusCode/{code:int}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult StatusCodePage(int code)
    {
        Response.StatusCode = code;
        return View("StatusCode", new StatusCodeViewModel(code));
    }

    // Accepts ALL verbs: UseExceptionHandler re-executes with the ORIGINAL method.
    [Route("Home/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
