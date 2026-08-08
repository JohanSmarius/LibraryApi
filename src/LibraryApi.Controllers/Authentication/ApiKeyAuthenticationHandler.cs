using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace LibraryApi.Controllers.Authentication;

public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationDefaults.HeaderName, out var providedKeys))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (providedKeys.Count != 1 || !IsValidApiKey(providedKeys[0]))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, ApiKeyAuthenticationDefaults.AuthenticationScheme),
            new Claim(ClaimTypes.Name, ApiKeyAuthenticationDefaults.AuthenticationScheme)
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private bool IsValidApiKey(string? providedKey)
    {
        if (string.IsNullOrEmpty(providedKey))
        {
            return false;
        }

        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(Options.ApiKey));
        var providedHash = SHA256.HashData(Encoding.UTF8.GetBytes(providedKey));

        return CryptographicOperations.FixedTimeEquals(expectedHash, providedHash);
    }
}
