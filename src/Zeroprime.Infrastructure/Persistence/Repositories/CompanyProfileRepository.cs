using Microsoft.EntityFrameworkCore;
using Zeroprime.Application.Abstractions;
using Zeroprime.Domain.Entities;
using Zeroprime.Infrastructure.Persistence.Context;

namespace Zeroprime.Infrastructure.Persistence.Repositories;

public class CompanyProfileRepository : ICompanyProfileRepository
{
    private readonly ZeroprimeDbContext _db;
    public CompanyProfileRepository(ZeroprimeDbContext db) => _db = db;

    public Task<CompanyProfile?> GetAsync(CancellationToken ct = default) =>
        _db.CompanyProfiles.AsNoTracking().OrderBy(c => c.Id).FirstOrDefaultAsync(ct);

    public async Task UpdateAsync(CompanyProfile profile, CancellationToken ct = default)
    {
        _db.CompanyProfiles.Update(profile);
        await _db.SaveChangesAsync(ct);
    }
}
