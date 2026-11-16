namespace Zeroprime.Web.Middleware;

/// <summary>
/// Adds defensive HTTP security headers to every response.
/// The Content-Security-Policy only allows same-origin assets; all scripts
/// and styles are local (no inline scripts are used anywhere in the app).
/// </summary>
public sealed class SecurityHeadersMiddleware
{
    private const string ContentSecurityPolicy =
        "default-src 'self'; " +
        "script-src 'self'; " +
        "style-src 'self' 'unsafe-inline'; " +  // inline style attributes are not used, but 'unsafe-inline' covers legacy attributes safely
        "img-src 'self' data:; " +
        "font-src 'self'; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none'; " +
        "form-action 'self'; " +
        "base-uri 'self'; " +
        "object-src 'none'";

    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;
        if (!headers.ContainsKey("X-Content-Type-Options"))
        {
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";
            headers["Content-Security-Policy"] = ContentSecurityPolicy;
            headers["Cross-Origin-Opener-Policy"] = "same-origin";
        }

        await _next(context);
    }
}
