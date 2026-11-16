using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Zeroprime.Application.Features.Products;

namespace Zeroprime.Web.Controllers;

/// <summary>
/// Generates /sitemap.xml from the public routes plus database-backed product pages.
/// </summary>
public class SitemapController : Controller
{
    private readonly IProductService _products;

    public SitemapController(IProductService products)
    {
        _products = products;
    }

    [HttpGet("sitemap.xml")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client, NoStore = false)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var staticRoutes = new[]
        {
            "/", "/about", "/products", "/smart-cities", "/contact", "/privacy", "/terms"
        };

        var products = await _products.GetActiveProductsAsync(cancellationToken);
        var productRoutes = products.Select(p => $"/products/{p.Id}");

        var settings = new XmlWriterSettings
        {
            Async = true,
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false,
            Indent = false
        };

        using var stream = new MemoryStream();
        await using (var writer = XmlWriter.Create(stream, settings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

            foreach (var route in staticRoutes.Concat(productRoutes))
            {
                writer.WriteStartElement("url");
                writer.WriteElementString("loc", baseUrl + route);
                writer.WriteElementString("changefreq", route == "/" ? "weekly" : "monthly");
                writer.WriteElementString("priority", route == "/" ? "1.0" : "0.7");
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            await writer.WriteEndDocumentAsync();
        }

        return Content(Encoding.UTF8.GetString(stream.ToArray()), "application/xml", Encoding.UTF8);
    }
}
