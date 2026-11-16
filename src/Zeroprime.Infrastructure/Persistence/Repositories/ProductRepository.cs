using Microsoft.EntityFrameworkCore;
using Zeroprime.Application.Abstractions;
using Zeroprime.Domain.Entities;
using Zeroprime.Infrastructure.Persistence.Context;

namespace Zeroprime.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ZeroprimeDbContext _db;
    public ProductRepository(ZeroprimeDbContext db) => _db = db;

    public async Task<IReadOnlyList<Product>> GetAllActiveAsync(CancellationToken ct = default) =>
        await _db.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(ct);

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
}
