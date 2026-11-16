namespace Zeroprime.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }                          // EF Core needs set for migrations/seeding
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    protected void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
}
