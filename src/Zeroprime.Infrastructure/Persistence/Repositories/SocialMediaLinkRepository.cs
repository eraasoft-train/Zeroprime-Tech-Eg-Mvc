using Microsoft.EntityFrameworkCore;
using Zeroprime.Application.Abstractions;
using Zeroprime.Domain.Entities;
using Zeroprime.Infrastructure.Persistence.Context;

namespace Zeroprime.Infrastructure.Persistence.Repositories;

public class SocialMediaLinkRepository : ISocialMediaLinkRepository
{
    private readonly ZeroprimeDbContext _db;
    public SocialMediaLinkRepository(ZeroprimeDbContext db) => _db = db;

    public async Task<IReadOnlyList<SocialMediaLink>> GetAllActiveAsync(CancellationToken ct = default) =>
        await _db.SocialMediaLinks
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(ct);
}
