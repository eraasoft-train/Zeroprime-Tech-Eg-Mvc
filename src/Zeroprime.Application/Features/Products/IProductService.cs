namespace Zeroprime.Application.Features.Products;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetActiveProductsAsync(CancellationToken cancellationToken = default);

    Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
}
