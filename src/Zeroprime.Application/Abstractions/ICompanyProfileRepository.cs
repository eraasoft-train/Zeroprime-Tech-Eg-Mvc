using Zeroprime.Domain.Entities;

namespace Zeroprime.Application.Abstractions;

public interface ICompanyProfileRepository
{
    Task<CompanyProfile?> GetAsync(CancellationToken ct = default);
    Task UpdateAsync(CompanyProfile profile, CancellationToken ct = default);
}
