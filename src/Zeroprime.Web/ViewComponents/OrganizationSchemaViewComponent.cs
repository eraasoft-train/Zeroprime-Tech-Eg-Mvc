using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Zeroprime.Application.Features.Company;
using Zeroprime.Application.Features.Settings;

namespace Zeroprime.Web.ViewComponents;

/// <summary>
/// Emits Schema.org Organization JSON-LD built strictly from verified company data
/// (see docs/research/ZEROPRIME_COMPANY_RESEARCH.md). No fields are fabricated:
/// missing values are omitted rather than guessed.
///
/// Must never fail the page: on any infrastructure error (e.g. database unreachable)
/// it falls back to the verified public facts so the error page can still render.
/// </summary>
public class OrganizationSchemaViewComponent : ViewComponent
{
    private readonly ICompanyProfileService _companyProfiles;
    private readonly ISocialMediaLinkService _socialLinks;
    private readonly ILogger<OrganizationSchemaViewComponent> _logger;

    public OrganizationSchemaViewComponent(
        ICompanyProfileService companyProfiles,
        ISocialMediaLinkService socialLinks,
        ILogger<OrganizationSchemaViewComponent> logger)
    {
        _companyProfiles = companyProfiles;
        _socialLinks = socialLinks;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        CompanyProfileDto company = FallbackCompany.Value;
        string[] sameAs = [];

        try
        {
            company = await _companyProfiles.GetProfileAsync(HttpContext.RequestAborted)
                      ?? FallbackCompany.Value;

            var links = await _socialLinks.GetActiveLinksAsync(HttpContext.RequestAborted);
            sameAs = links.Select(l => l.Url).ToArray();
        }
        catch (Exception ex)
        {
            // Never let SEO plumbing break page rendering (e.g. when the database is down).
            _logger.LogWarning(ex, "Organization schema falling back to static verified values");
        }

        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

        var schema = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Organization",
            ["name"] = company.Name,
            ["url"] = baseUrl,
            ["description"] = company.Description,
            ["foundingDate"] = company.FoundingYear.ToString(),
            ["email"] = company.ContactEmail,
            ["address"] = new Dictionary<string, string>
            {
                ["@type"] = "PostalAddress",
                ["addressLocality"] = "Cairo",
                ["addressCountry"] = "EG"
            }
        };

        if (sameAs.Length > 0)
        {
            schema["sameAs"] = sameAs;
        }
        else
        {
            schema["sameAs"] = new[]
            {
                "https://www.linkedin.com/company/zeroprime-technologies",
                "https://www.instagram.com/zeroprime.tech/",
                "https://www.facebook.com/zeroprime.tech"
            };
        }

        var json = JsonSerializer.Serialize(schema, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        });

        return View(model: json);
    }

    private static class FallbackCompany
    {
        public static readonly CompanyProfileDto Value = new(
            "ZeroPrime Technologies",
            "Egyptian startup specializing in smart sustainable cities infrastructure.",
            "To design and implement smart city infrastructure aligned with sustainable development goals.",
            "Sustainable cities powered by autonomous waste management.",
            "Mechanical or Industrial Engineering / Smart Cities",
            "Cairo, Egypt",
            "zeroprime.tech@gmail.com",
            2017);
    }
}
