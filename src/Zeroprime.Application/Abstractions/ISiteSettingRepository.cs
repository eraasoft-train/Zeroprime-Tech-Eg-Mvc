using Zeroprime.Domain.Entities;

namespace Zeroprime.Application.Abstractions;

public interface ISiteSettingRepository
{
    Task<string?> GetValueAsync(string key, CancellationToken ct = default);
    Task<IReadOnlyList<SiteSetting>> GetAllAsync(CancellationToken ct = default);
}
