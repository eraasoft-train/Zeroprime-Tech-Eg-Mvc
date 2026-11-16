using Microsoft.EntityFrameworkCore;
using Zeroprime.Application.Abstractions;
using Zeroprime.Domain.Entities;
using Zeroprime.Infrastructure.Persistence.Context;

namespace Zeroprime.Infrastructure.Persistence.Repositories;

public class SiteSettingRepository : ISiteSettingRepository
{
    private readonly ZeroprimeDbContext _db;
    public SiteSettingRepository(ZeroprimeDbContext db) => _db = db;

    public async Task<string?> GetValueAsync(string key, CancellationToken ct = default) =>
        await _db.SiteSettings
            .AsNoTracking()
            .Where(s => s.Key == key)
            .Select(s => s.Value)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<SiteSetting>> GetAllAsync(CancellationToken ct = default) =>
        await _db.SiteSettings.AsNoTracking().OrderBy(s => s.Key).ToListAsync(ct);
}
