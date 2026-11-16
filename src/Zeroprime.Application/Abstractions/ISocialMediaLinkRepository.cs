using Zeroprime.Domain.Entities;

namespace Zeroprime.Application.Abstractions;

public interface ISocialMediaLinkRepository
{
    Task<IReadOnlyList<SocialMediaLink>> GetAllActiveAsync(CancellationToken ct = default);
}
