using FluentValidation;
using Microsoft.Extensions.Logging;
using Zeroprime.Application.Abstractions;
using Zeroprime.Domain.Entities;

namespace Zeroprime.Application.Features.Contact;

public sealed class ContactInquiryService : IContactInquiryService
{
    private readonly IContactInquiryRepository _repository;
    private readonly IValidator<CreateContactInquiryRequest> _validator;
    private readonly ILogger<ContactInquiryService> _logger;

    public ContactInquiryService(
        IContactInquiryRepository repository,
        IValidator<CreateContactInquiryRequest> validator,
        ILogger<ContactInquiryService> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<ContactSubmissionResult> SubmitAsync(
        CreateContactInquiryRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());

            return ContactSubmissionResult.Failure(errors);
        }

        var inquiry = ContactInquiry.Create(
            request.Name.Trim(),
            request.Email.Trim(),
            request.Subject.Trim(),
            request.Message.Trim(),
            string.IsNullOrWhiteSpace(request.Company) ? null : request.Company.Trim(),
            string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim());

        await _repository.AddAsync(inquiry, cancellationToken);

        // Deliberately log the id only - never message contents or full PII.
        _logger.LogInformation("Contact inquiry {InquiryId} received and stored", inquiry.Id);

        return ContactSubmissionResult.Success();
    }
}
