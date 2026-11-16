using Zeroprime.Domain.Entities;

namespace Zeroprime.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllActiveAsync(CancellationToken ct = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
}
