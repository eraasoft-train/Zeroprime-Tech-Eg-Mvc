using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Zeroprime.Application.Abstractions;
using Zeroprime.Application.Common;
using Zeroprime.Domain.Entities;

namespace Zeroprime.Application.Features.Products;

public sealed class ProductService : IProductService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IProductRepository _repository;
    private readonly IMemoryCache _cache;

    public ProductService(IProductRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IReadOnlyList<ProductDto>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(CacheKeys.ActiveProducts, out IReadOnlyList<ProductDto>? cached) && cached is not null)
        {
            return cached;
        }

        var products = await _repository.GetAllActiveAsync(cancellationToken);
        var dtos = products.Select(Map).ToList();
        _cache.Set(CacheKeys.ActiveProducts, dtos, CacheKeys.DefaultTtl);
        return dtos;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return null;
        }

        // Products are few and read-mostly; reuse the cached active list when possible.
        var active = await GetActiveProductsAsync(cancellationToken);
        var cached = active.FirstOrDefault(p => p.Id == id);
        if (cached is not null)
        {
            return cached;
        }

        var product = await _repository.GetByIdAsync(id, cancellationToken);
        return product is null || !product.IsActive ? null : Map(product);
    }

    private static ProductDto Map(Product product) => new(
        product.Id,
        product.Name,
        product.Description,
        product.Category,
        DeserializeList(product.KeyFeatures),
        DeserializeList(product.TargetLocations),
        product.ImageUrl,
        product.DisplayOrder);

    private static IReadOnlyList<string> DeserializeList(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json, JsonOptions) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
