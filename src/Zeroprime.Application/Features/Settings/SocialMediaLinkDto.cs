namespace Zeroprime.Application.Features.Settings;

public sealed record SocialMediaLinkDto(
    string Platform,
    string Url,
    string Icon,
    int DisplayOrder);
