using Zeroprime.Domain.Common;

namespace Zeroprime.Domain.Entities;

public class CompanyProfile : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Mission { get; set; } = string.Empty;
    public string Vision { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public int FoundingYear { get; set; }

    // Required by EF Core
    public CompanyProfile() { }

    public static CompanyProfile Create(
        string name, string description, string mission, string vision,
        string industry, string location, string contactEmail, int foundingYear)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Company name required.", nameof(name));
        if (string.IsNullOrWhiteSpace(contactEmail)) throw new ArgumentException("Contact email required.", nameof(contactEmail));
        if (foundingYear < 1900 || foundingYear > DateTime.UtcNow.Year)
            throw new ArgumentException("Invalid founding year.", nameof(foundingYear));

        return new CompanyProfile
        {
            Name = name,
            Description = description,
            Mission = mission,
            Vision = vision,
            Industry = industry,
            Location = location,
            ContactEmail = contactEmail,
            FoundingYear = foundingYear
        };
    }

    public void Update(string description, string mission, string vision,
        string industry, string location, string contactEmail)
    {
        Description = description; Mission = mission; Vision = vision;
        Industry = industry; Location = location; ContactEmail = contactEmail;
        SetUpdatedAt();
    }
}
