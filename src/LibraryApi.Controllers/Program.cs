using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Endpoints;
using LibraryApi.Controllers.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

        return Task.CompletedTask;
    });
});
builder.Services.AddValidation();

var connectionString = builder.Configuration.GetConnectionString("LibraryDb")
    ?? "Data Source=library.db";

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();

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

app.MapAuthorsEndpoints();
app.MapBooksEndpoints();

app.Run();
