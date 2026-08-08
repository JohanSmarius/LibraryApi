using Microsoft.AspNetCore.Authentication;

namespace LibraryApi.Controllers.Authentication;

public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = string.Empty;
}
