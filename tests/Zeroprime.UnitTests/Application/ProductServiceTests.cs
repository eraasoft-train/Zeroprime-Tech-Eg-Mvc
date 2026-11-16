using Microsoft.Extensions.Caching.Memory;
using Zeroprime.Application.Abstractions;
using Zeroprime.Application.Features.Products;
using Zeroprime.Domain.Entities;

namespace Zeroprime.UnitTests.Application;

sealed class StubProductRepository : IProductRepository
{
    public int GetAllActiveCallCount { get; private set; }
    private readonly IReadOnlyList<Product> _products;

    public StubProductRepository(IReadOnlyList<Product> products) => _products = products;

    public Task<IReadOnlyList<Product>> GetAllActiveAsync(CancellationToken ct = default)
    {
        GetAllActiveCallCount++;
        return Task.FromResult(_products);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
}

public class ProductServiceTests
{
    private static Product CanBank() => Product.Create(
        "CanBank",
        "Reverse vending machine",
        "RVM",
        "[\"Instant mobile credit rewards\",\"Charity donation option\"]",
        "[\"Universities\",\"Supermarkets\"]",
        "/images/canbank.svg",
        1);

    [Fact]
    public async Task GetActiveProductsAsync_MapsJsonLists()
    {
        var product = CanBank();
        var repository = new StubProductRepository([product]);
        var service = new ProductService(repository, new MemoryCache(new MemoryCacheOptions()));

        var products = await service.GetActiveProductsAsync();

        var dto = Assert.Single(products);
        Assert.Equal("CanBank", dto.Name);
        Assert.Equal(["Instant mobile credit rewards", "Charity donation option"], dto.KeyFeatures.ToArray());
        Assert.Equal(["Universities", "Supermarkets"], dto.TargetLocations.ToArray());
    }

    [Fact]
    public async Task GetActiveProductsAsync_CachesResultAndHitsRepositoryOnce()
    {
        var repository = new StubProductRepository([CanBank()]);
        var service = new ProductService(repository, new MemoryCache(new MemoryCacheOptions()));

        await service.GetActiveProductsAsync();
        await service.GetActiveProductsAsync();

        Assert.Equal(1, repository.GetAllActiveCallCount);
    }

    [Fact]
    public async Task GetActiveProductsAsync_WithMalformedJson_ReturnsEmptyLists()
    {
        var broken = Product.Create("CanBank", "desc", "RVM");
        broken.Update("desc", "RVM", "{not json", "[unclosed", "", 0);
        var repository = new StubProductRepository([broken]);
        var service = new ProductService(repository, new MemoryCache(new MemoryCacheOptions()));

        var products = await service.GetActiveProductsAsync();

        var dto = Assert.Single(products);
        Assert.Empty(dto.KeyFeatures);
        Assert.Empty(dto.TargetLocations);
    }
}
