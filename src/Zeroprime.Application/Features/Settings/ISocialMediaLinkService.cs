namespace Zeroprime.Application.Features.Settings;

public interface ISocialMediaLinkService
{
    Task<IReadOnlyList<SocialMediaLinkDto>> GetActiveLinksAsync(CancellationToken cancellationToken = default);
}
