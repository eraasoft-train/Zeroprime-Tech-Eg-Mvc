namespace Zeroprime.Application.Features.Products;

public sealed record ProductDto(
    int Id,
    string Name,
    string Description,
    string Category,
    IReadOnlyList<string> KeyFeatures,
    IReadOnlyList<string> TargetLocations,
    string ImageUrl,
    int DisplayOrder);
