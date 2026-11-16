using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Zeroprime.Application.Features.Contact;
using Zeroprime.Application.Features.Settings;

namespace Zeroprime.Web.Controllers;

public class ContactController : Controller
{
    private readonly IContactInquiryService _inquiries;
    private readonly ISiteSettingsService _settings;

    public ContactController(
        IContactInquiryService inquiries,
        ISiteSettingsService settings)
    {
        _inquiries = inquiries;
        _settings = settings;
    }

    [HttpGet("contact")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Submitted"] = TempData["Submitted"] is true;
        ViewData["Settings"] = await _settings.GetSettingsAsync(cancellationToken);
        return View(new CreateContactInquiryRequest());
    }

    [HttpPost("contact")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("contact")]
    public async Task<IActionResult> Index(CreateContactInquiryRequest model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Settings"] = await _settings.GetSettingsAsync(cancellationToken);
            return View(model);
        }

        var result = await _inquiries.SubmitAsync(model, cancellationToken);
        if (!result.Succeeded)
        {
            foreach (var (key, messages) in result.Errors)
            {
                foreach (var message in messages)
                {
                    ModelState.AddModelError(key, message);
                }
            }

            ViewData["Settings"] = await _settings.GetSettingsAsync(cancellationToken);
            return View(model);
        }

        TempData["Submitted"] = true;
        return RedirectToAction(nameof(Index));
    }
}
