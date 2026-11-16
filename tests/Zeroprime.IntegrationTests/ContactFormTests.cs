using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Zeroprime.IntegrationTests;

public partial class ContactFormTests : IClassFixture<ZeroprimeWebFactory>
{
    private readonly ZeroprimeWebFactory _factory;

    public ContactFormTests(ZeroprimeWebFactory factory) => _factory = factory;

    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex AntiforgeryTokenRegex();

    private async Task<(HttpClient Client, string Token)> GetFormAndTokenAsync()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var html = await client.GetStringAsync("/contact");
        var match = AntiforgeryTokenRegex().Match(html);
        Assert.True(match.Success, "Contact form anti-forgery token was not rendered.");

        return (client, match.Groups[1].Value);
    }

    private static StringContent BuildForm(string token, IDictionary<string, string> fields) =>
        new(string.Join("&",
                new Dictionary<string, string>(fields)
                {
                    ["__RequestVerificationToken"] = token
                }.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}")),
            Encoding.UTF8,
            "application/x-www-form-urlencoded");

    [Fact]
    public async Task ContactForm_WithValidData_RedirectsAndPersistsInquiry()
    {
        var (client, token) = await GetFormAndTokenAsync();
        var message = new string('x', 32);

        var response = await client.PostAsync("/contact", BuildForm(token, new Dictionary<string, string>
        {
            ["Name"] = "Integration Test User",
            ["Email"] = "integration.test@example.com",
            ["Company"] = "TestCo",
            ["Subject"] = "CanBank deployment",
            ["Message"] = message
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        using var db = _factory.CreateDbContext();
        var inquiry = await db.ContactInquiries
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Email == "integration.test@example.com");

        Assert.NotNull(inquiry);
        Assert.Equal(message, inquiry.Message);
        Assert.Equal(Zeroprime.Domain.Enums.InquiryStatus.New, inquiry.Status);
    }

    [Fact]
    public async Task ContactForm_WithMissingRequiredFields_RedisplaysWithErrors()
    {
        var (client, token) = await GetFormAndTokenAsync();

        var response = await client.PostAsync("/contact", BuildForm(token, new Dictionary<string, string>
        {
            ["Name"] = "",
            ["Email"] = "not-an-email",
            ["Subject"] = "",
            ["Message"] = ""
        }));

        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Please enter your name", body);

        using var db = _factory.CreateDbContext();
        Assert.False(await db.ContactInquiries.AsNoTracking().AnyAsync(i => i.Email == "not-an-email"));
    }

    [Fact]
    public async Task ContactForm_WithoutAntiforgeryToken_IsRejected()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.PostAsync("/contact", new StringContent(
            "Name=Jane&Email=jane@example.com&Subject=Hi&Message=Hello%20there%20world",
            Encoding.UTF8,
            "application/x-www-form-urlencoded"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
