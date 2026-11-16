using Zeroprime.Domain.Entities;

namespace Zeroprime.Application.Abstractions;

public interface IContactInquiryRepository
{
    Task AddAsync(ContactInquiry inquiry, CancellationToken ct = default);
    Task<IReadOnlyList<ContactInquiry>> GetAllAsync(CancellationToken ct = default);
}
