using System.ComponentModel.DataAnnotations;

namespace Zeroprime.Application.Features.Contact;

/// <summary>
/// Inbound contact-form payload. Reused by the MVC form binding; validated server-side
/// by <see cref="Validators.CreateContactInquiryRequestValidator"/> (FluentValidation) and
/// mirrored with DataAnnotations so jQuery Unobtrusive Validation works on the client.
/// </summary>
public sealed class CreateContactInquiryRequest
{
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(254)]
    public string Email { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Company { get; set; }

    [StringLength(30)]
    [RegularExpression(@"^[\d\s()+.\-#*]*$", ErrorMessage = "Phone contains invalid characters.")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Please choose a subject.")]
    [StringLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a message.")]
    [MinLength(10, ErrorMessage = "Message must be at least 10 characters.")]
    [StringLength(4000)]
    public string Message { get; set; } = string.Empty;
}
