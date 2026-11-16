using Zeroprime.Application.Features.Company;
using Zeroprime.Application.Features.Products;
using Zeroprime.Application.Features.Settings;

namespace Zeroprime.Web.ViewModels.Home;

public sealed record HomeViewModel(
    CompanyProfileDto? Company,
    IReadOnlyList<ProductDto> Products,
    SiteSettingsDto Settings);
