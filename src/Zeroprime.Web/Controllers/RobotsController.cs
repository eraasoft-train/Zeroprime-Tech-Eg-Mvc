using Microsoft.AspNetCore.Mvc;

namespace Zeroprime.Web.Controllers;

/// <summary>
/// Generates /robots.txt with an absolute sitemap URL based on the request host,
/// so the file is valid in every environment (dev, staging, production).
/// </summary>
public class RobotsController : Controller
{
    [HttpGet("robots.txt")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client, NoStore = false)]
    public IActionResult Index()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var content = $"""
        User-agent: *
        Allow: /

        Disallow: /health
        Disallow: /Home/StatusCode
        Disallow: /Home/Error

        Sitemap: {baseUrl}/sitemap.xml
        """;

        return Content(content, "text/plain", System.Text.Encoding.UTF8);
    }
}
