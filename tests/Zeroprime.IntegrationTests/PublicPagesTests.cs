using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Zeroprime.IntegrationTests;

public class PublicPagesTests : IClassFixture<ZeroprimeWebFactory>
{
    private readonly ZeroprimeWebFactory _factory;

    public PublicPagesTests(ZeroprimeWebFactory factory) => _factory = factory;

    [Theory]
    [InlineData("/", "ZeroPrime")]
    [InlineData("/about", "ZeroPrime")]
    [InlineData("/products", "CanBank")]
    [InlineData("/products/1", "CanBank")]
    [InlineData("/smart-cities", "smart")]
    [InlineData("/contact", "zeroprime.tech@gmail.com")]
    [InlineData("/privacy", "Privacy Policy")]
    [InlineData("/terms", "Terms")]
    public async Task PublicPages_ReturnSuccessWithExpectedContent(string url, string expectedText)
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(expectedText, body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ProductDetails_WithUnknownId_ReturnsNotFound()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/products/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UnknownRoute_ReturnsCustom404Page()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/definitely-not-a-page");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Page not found", body);
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", body);
    }

    [Fact]
    public async Task Sitemap_IsValidAndReferencesProductPages()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/sitemap.xml");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<urlset", body);
        Assert.Contains("/products/1", body);
    }

    [Fact]
    public async Task RobotsTxt_ReferencesSitemap()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/robots.txt");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Sitemap:", body);
    }

    [Fact]
    public async Task Responses_IncludeSecurityHeaders()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
        Assert.True(response.Headers.Contains("Content-Security-Policy"));
        Assert.True(response.Headers.Contains("Referrer-Policy"));
    }
}
