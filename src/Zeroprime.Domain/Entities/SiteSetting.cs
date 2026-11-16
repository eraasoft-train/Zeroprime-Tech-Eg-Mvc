namespace Zeroprime.Domain.Entities;

public class SiteSetting
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public SiteSetting() { }

    public static SiteSetting Create(string key, string value, string description = "")
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key required.", nameof(key));
        return new SiteSetting { Key = key, Value = value, Description = description, UpdatedAt = DateTime.UtcNow };
    }

    public void Update(string value, string description)
    {
        Value = value; Description = description; UpdatedAt = DateTime.UtcNow;
    }
}
