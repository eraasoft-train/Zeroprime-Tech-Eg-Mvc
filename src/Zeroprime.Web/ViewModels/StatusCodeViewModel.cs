namespace Zeroprime.Web.ViewModels;

public sealed record StatusCodeViewModel(int StatusCode)
{
    public string Title => StatusCode switch
    {
        404 => "Page not found",
        403 => "Access denied",
        429 => "Too many requests",
        _ => "Something went wrong"
    };

    public string Message => StatusCode switch
    {
        404 => "The page you are looking for does not exist or may have been moved.",
        403 => "You do not have permission to access this resource.",
        429 => "You have submitted too many requests in a short period. Please wait a few minutes and try again.",
        _ => "An unexpected error occurred on our side. Please try again later."
    };
}
