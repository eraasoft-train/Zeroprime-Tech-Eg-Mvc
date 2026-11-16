namespace Zeroprime.Application.Features.Company;

public interface ICompanyProfileService
{
    /// <summary>Returns the singleton company profile, or null when the database has not been seeded yet.</summary>
    Task<CompanyProfileDto?> GetProfileAsync(CancellationToken cancellationToken = default);
}
