namespace LibraryApi.Controllers.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string? _apiKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, IWebHostEnvironment environment)
    {
        _next = next;
        // In development, skip authentication if no key is configured
        _apiKey = environment.IsDevelopment() ? null : configuration["ApiSettings:ApiKey"];
        if (!environment.IsDevelopment() && string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("API key not configured. Set ApiSettings:ApiKey in your secrets.");
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip authentication for OpenAPI, Scalar, and health-check paths
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/scalar", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // If no API key is configured (development mode), skip authentication
        if (string.IsNullOrEmpty(_apiKey))
        {
            await _next(context);
            return;
        }

        var providedKey = context.Request.Headers["X-API-Key"].ToString();

        if (string.IsNullOrEmpty(providedKey) || providedKey != _apiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\": \"Unauthorized: Invalid or missing API key.\"}");
            return;
        }

        await _next(context);
    }
}
