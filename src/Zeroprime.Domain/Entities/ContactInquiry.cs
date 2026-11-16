using Zeroprime.Domain.Common;
using Zeroprime.Domain.Enums;

namespace Zeroprime.Domain.Entities;

public class ContactInquiry : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Phone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    public DateTime SubmittedAt { get; set; }

    public ContactInquiry() { }

    public static ContactInquiry Create(
        string name, string email, string subject, string message,
        string? company = null, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email required.", nameof(email));
        if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Subject required.", nameof(subject));
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message required.", nameof(message));

        return new ContactInquiry
        {
            Name = name,
            Email = email,
            Company = company,
            Phone = phone,
            Subject = subject,
            Message = message,
            Status = InquiryStatus.New,
            SubmittedAt = DateTime.UtcNow
        };
    }

    public void UpdateStatus(InquiryStatus status) { Status = status; SetUpdatedAt(); }
    public void MarkAsInProgress() => UpdateStatus(InquiryStatus.InProgress);
    public void MarkAsResolved() => UpdateStatus(InquiryStatus.Resolved);
    public void MarkAsClosed() => UpdateStatus(InquiryStatus.Closed);
}
