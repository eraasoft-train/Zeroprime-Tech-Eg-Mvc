namespace Zeroprime.Application.Features.Contact;

public interface IContactInquiryService
{
    Task<ContactSubmissionResult> SubmitAsync(CreateContactInquiryRequest request, CancellationToken cancellationToken = default);
}
