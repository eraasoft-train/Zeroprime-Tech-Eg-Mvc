using System.Net;
using Xunit;

namespace Zeroprime.IntegrationTests;

public class ConsentAndPrivacyTests : IClassFixture<ZeroprimeWebFactory>
{
    private readonly ZeroprimeWebFactory _factory;

    public ConsentAndPrivacyTests(ZeroprimeWebFactory factory) => _factory = factory;

    [Fact]
    public async Task HomePage_RendersCookieConsentBanner()
    {
        using var client = _factory.CreateClient();

        var body = await client.GetStringAsync("/");

        Assert.Contains("id=\"zpCookieConsent\"", body);
        Assert.Contains("role=\"dialog\"", body);
        Assert.Contains("zpConsentAccept", body);
        Assert.Contains("zpConsentEssentials", body);
        Assert.Contains("cookie-consent.js", body);
    }

    [Fact]
    public async Task AllPublicPages_RenderCookieConsentBanner()
    {
        using var client = _factory.CreateClient();

        foreach (var url in new[] { "/", "/about", "/products", "/smart-cities", "/contact", "/privacy", "/terms" })
        {
            var body = await client.GetStringAsync(url);
            Assert.Contains("zpCookieConsent", body);
        }
    }

    [Fact]
    public async Task PrivacyPolicy_DocumentsRealCookiesAndStorage()
    {
        using var client = _factory.CreateClient();

        var body = await client.GetStringAsync("/privacy");

        Assert.Contains("zp_consent", body);
        Assert.Contains("zp-consent", body);
        Assert.Contains(".AspNetCore.Antiforgery", body);
        Assert.Contains("zp-theme", body);
    }
}
