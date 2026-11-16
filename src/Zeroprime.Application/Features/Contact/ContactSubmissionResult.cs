namespace Zeroprime.Application.Features.Contact;

/// <summary>
/// Outcome of a contact-form submission. Never throws for validation problems;
/// infrastructure failures still surface as exceptions to the global error handler.
/// </summary>
public sealed record ContactSubmissionResult(bool Succeeded, IReadOnlyDictionary<string, string[]> Errors)
{
    public static ContactSubmissionResult Success() => new(true, new Dictionary<string, string[]>());

    public static ContactSubmissionResult Failure(IReadOnlyDictionary<string, string[]> errors) => new(false, errors);
}
