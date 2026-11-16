using Microsoft.Extensions.Caching.Memory;
using Zeroprime.Application.Abstractions;
using Zeroprime.Application.Common;
using Zeroprime.Domain.Entities;

namespace Zeroprime.Application.Features.Company;

public sealed class CompanyProfileService : ICompanyProfileService
{
    private readonly ICompanyProfileRepository _repository;
    private readonly IMemoryCache _cache;

    public CompanyProfileService(ICompanyProfileRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<CompanyProfileDto?> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeys.CompanyProfile, out CompanyProfileDto? cached))
        {
            return cached;
        }

        var profile = await _repository.GetAsync(cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var dto = Map(profile);
        _cache.Set(CacheKeys.CompanyProfile, dto, CacheKeys.DefaultTtl);
        return dto;
    }

    private static CompanyProfileDto Map(CompanyProfile profile) => new(
        profile.Name,
        profile.Description,
        profile.Mission,
        profile.Vision,
        profile.Industry,
        profile.Location,
        profile.ContactEmail,
        profile.FoundingYear);
}
