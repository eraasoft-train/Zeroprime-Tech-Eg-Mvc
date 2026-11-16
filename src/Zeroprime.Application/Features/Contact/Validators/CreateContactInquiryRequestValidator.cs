using FluentValidation;

namespace Zeroprime.Application.Features.Contact.Validators;

public sealed class CreateContactInquiryRequestValidator : AbstractValidator<CreateContactInquiryRequest>
{
    public CreateContactInquiryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Please enter your name.")
            .MaximumLength(120).WithMessage("Name must be 120 characters or fewer.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Please enter your email address.")
            .EmailAddress().WithMessage("Please enter a valid email address.")
            .MaximumLength(254);

        RuleFor(x => x.Company)
            .MaximumLength(150).WithMessage("Company must be 150 characters or fewer.");

        RuleFor(x => x.Phone)
            .MaximumLength(30).WithMessage("Phone must be 30 characters or fewer.")
            .Matches(@"^[\d\s()+.\-#*]*$").WithMessage("Phone contains invalid characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Please choose a subject.")
            .MaximumLength(200).WithMessage("Subject must be 200 characters or fewer.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Please enter a message.")
            .MinimumLength(10).WithMessage("Message must be at least 10 characters.")
            .MaximumLength(4000).WithMessage("Message must be 4000 characters or fewer.");
    }
}
