namespace Zeroprime.Application.Features.Settings;

public interface ISiteSettingsService
{
    Task<SiteSettingsDto> GetSettingsAsync(CancellationToken cancellationToken = default);
}
