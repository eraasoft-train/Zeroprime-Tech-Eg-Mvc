using Zeroprime.Domain.Common;

namespace Zeroprime.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string KeyFeatures { get; set; } = "[]";       // JSON array stored as text
    public string TargetLocations { get; set; } = "[]";   // JSON array stored as text
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    public Product() { }

    public static Product Create(
        string name, string description, string category,
        string keyFeatures = "[]", string targetLocations = "[]",
        string imageUrl = "", int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name required.", nameof(name));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description required.", nameof(description));

        return new Product
        {
            Name = name,
            Description = description,
            Category = category,
            KeyFeatures = keyFeatures,
            TargetLocations = targetLocations,
            ImageUrl = imageUrl,
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    public void Update(string description, string category, string keyFeatures,
        string targetLocations, string imageUrl, int displayOrder)
    {
        Description = description; Category = category; KeyFeatures = keyFeatures;
        TargetLocations = targetLocations; ImageUrl = imageUrl; DisplayOrder = displayOrder;
        SetUpdatedAt();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
