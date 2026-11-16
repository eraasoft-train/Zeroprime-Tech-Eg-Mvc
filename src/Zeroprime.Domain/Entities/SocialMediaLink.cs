using Zeroprime.Domain.Common;

namespace Zeroprime.Domain.Entities;

public class SocialMediaLink : BaseEntity
{
    public string Platform { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    public SocialMediaLink() { }

    public static SocialMediaLink Create(string platform, string url, string icon = "", int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(platform)) throw new ArgumentException("Platform required.", nameof(platform));
        if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("URL required.", nameof(url));

        return new SocialMediaLink { Platform = platform, Url = url, Icon = icon, DisplayOrder = displayOrder, IsActive = true };
    }

    public void Update(string url, string icon, int displayOrder)
    {
        Url = url; Icon = icon; DisplayOrder = displayOrder; SetUpdatedAt();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
