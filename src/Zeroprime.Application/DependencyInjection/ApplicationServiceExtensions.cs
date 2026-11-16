using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Zeroprime.Application.Features.Company;
using Zeroprime.Application.Features.Contact;
using Zeroprime.Application.Features.Contact.Validators;
using Zeroprime.Application.Features.Products;
using Zeroprime.Application.Features.Settings;

namespace Zeroprime.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICompanyProfileService, CompanyProfileService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IContactInquiryService, ContactInquiryService>();
        services.AddScoped<ISocialMediaLinkService, SocialMediaLinkService>();
        services.AddScoped<ISiteSettingsService, SiteSettingsService>();

        services.AddScoped<IValidator<CreateContactInquiryRequest>, CreateContactInquiryRequestValidator>();

        return services;
    }
}
