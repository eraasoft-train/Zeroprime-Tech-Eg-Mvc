using Microsoft.Extensions.Caching.Memory;
using Zeroprime.Application.Abstractions;
using Zeroprime.Application.Common;

namespace Zeroprime.Application.Features.Settings;

public sealed class SiteSettingsService : ISiteSettingsService
{
    private readonly ISiteSettingRepository _repository;
    private readonly IMemoryCache _cache;

    public SiteSettingsService(ISiteSettingRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<SiteSettingsDto> GetSettingsAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeys.SiteSettings, out SiteSettingsDto? cached) && cached is not null)
        {
            return cached;
        }

        var settings = await _repository.GetAllAsync(cancellationToken);
        var map = settings.ToDictionary(s => s.Key, s => s.Value, StringComparer.OrdinalIgnoreCase);

        var dto = new SiteSettingsDto(
            Get(map, "SiteName", SiteSettingsDto.Fallback.SiteName),
            Get(map, "SiteDescription", SiteSettingsDto.Fallback.SiteDescription),
            Get(map, "HeroTitle", SiteSettingsDto.Fallback.HeroTitle),
            Get(map, "HeroSubtitle", SiteSettingsDto.Fallback.HeroSubtitle));

        _cache.Set(CacheKeys.SiteSettings, dto, CacheKeys.DefaultTtl);
        return dto;
    }

    private static string Get(IReadOnlyDictionary<string, string> map, string key, string fallback) =>
        map.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : fallback;
}
