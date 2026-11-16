using Microsoft.AspNetCore.Mvc;

namespace Zeroprime.Web.Controllers;

/// <summary>
/// Public page describing ZeroPrime Technologies' smart-cities infrastructure vision
/// and the role of autonomous waste management within it.
/// </summary>
public class SmartCitiesController : Controller
{
    [HttpGet("smart-cities")]
    public IActionResult Index() => View();
}
