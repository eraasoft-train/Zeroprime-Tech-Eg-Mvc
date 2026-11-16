using Microsoft.Extensions.Caching.Memory;
using Zeroprime.Application.Abstractions;
using Zeroprime.Application.Common;

namespace Zeroprime.Application.Features.Settings;

public sealed class SocialMediaLinkService : ISocialMediaLinkService
{
    private readonly ISocialMediaLinkRepository _repository;
    private readonly IMemoryCache _cache;

    public SocialMediaLinkService(ISocialMediaLinkRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IReadOnlyList<SocialMediaLinkDto>> GetActiveLinksAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeys.SocialLinks, out IReadOnlyList<SocialMediaLinkDto>? cached) && cached is not null)
        {
            return cached;
        }

        var links = await _repository.GetAllActiveAsync(cancellationToken);
        var dtos = links
            .Select(l => new SocialMediaLinkDto(l.Platform, l.Url, l.Icon, l.DisplayOrder))
            .ToList();

        _cache.Set(CacheKeys.SocialLinks, dtos, CacheKeys.DefaultTtl);
        return dtos;
    }
}
