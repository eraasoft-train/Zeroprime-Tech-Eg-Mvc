namespace Zeroprime.Application.Features.Company;

/// <summary>
/// Public company profile projection used by the website.
/// </summary>
public sealed record CompanyProfileDto(
    string Name,
    string Description,
    string Mission,
    string Vision,
    string Industry,
    string Location,
    string ContactEmail,
    int FoundingYear);
