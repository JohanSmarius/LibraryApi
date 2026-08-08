using LibraryApi.Controllers.Authentication;
using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var apiKey = builder.Configuration["Authentication:ApiKey"]
    ?? throw new InvalidOperationException(
        "The API key is not configured. Store it with: dotnet user-secrets set \"Authentication:ApiKey\" \"<key>\"");

builder.Services
    .AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationDefaults.AuthenticationScheme,
        options => options.ApiKey = apiKey);
builder.Services.AddAuthorization();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new()
        {
            Title = "Library API",
            Version = "v1",
            Description = "A .NET 10 Minimal API for managing authors and books."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>(StringComparer.Ordinal);
        document.Components.SecuritySchemes[ApiKeyAuthenticationDefaults.AuthenticationScheme] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = ApiKeyAuthenticationDefaults.HeaderName,
                Description = $"API key supplied in the {ApiKeyAuthenticationDefaults.HeaderName} header."
            };

        foreach (var operation in document.Paths.Values
                     .SelectMany(path =>
                         path.Operations?.Values.AsEnumerable() ?? Enumerable.Empty<OpenApiOperation>()))
        {
            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(
                    ApiKeyAuthenticationDefaults.AuthenticationScheme,
                    document)] = []
            });
        }

        return Task.CompletedTask;
    });
});
builder.Services.AddValidation();

var connectionString = builder.Configuration.GetConnectionString("LibraryDb")
    ?? "Data Source=library.db";

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Apply any pending EF Core migrations and seed known data on startup.
// This is the "reset button" behaviour: delete library.db (or run
// scripts/reset-db.*) and the next run always comes back to the same
// known state - no manual re-seeding live on stage.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.Migrate();
    SeedData.EnsureSeeded(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .WithTitle("Library API")
        .DisableAgent());
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("/api")
    .RequireAuthorization();

api.MapAuthorsEndpoints();
api.MapBooksEndpoints();

app.Run();
