using Zeroprime.Application.Features.Products;

namespace Zeroprime.Web.ViewModels.Products;

public sealed record ProductListViewModel(IReadOnlyList<ProductDto> Products);
