using Microsoft.EntityFrameworkCore;
using Zeroprime.Application.Abstractions;
using Zeroprime.Domain.Entities;
using Zeroprime.Infrastructure.Persistence.Context;

namespace Zeroprime.Infrastructure.Persistence.Repositories;

public class ContactInquiryRepository : IContactInquiryRepository
{
    private readonly ZeroprimeDbContext _db;
    public ContactInquiryRepository(ZeroprimeDbContext db) => _db = db;

    public async Task AddAsync(ContactInquiry inquiry, CancellationToken ct = default)
    {
        await _db.ContactInquiries.AddAsync(inquiry, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ContactInquiry>> GetAllAsync(CancellationToken ct = default) =>
        await _db.ContactInquiries
            .AsNoTracking()
            .OrderByDescending(c => c.SubmittedAt)
            .ToListAsync(ct);
}
