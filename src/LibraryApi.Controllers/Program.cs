using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Endpoints;
using LibraryApi.Controllers.Middleware;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Authors;
using LibraryApi.Controllers.UseCases.Books;
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

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddScoped<GetAuthorsUseCase>();
builder.Services.AddScoped<GetAuthorUseCase>();
builder.Services.AddScoped<CreateAuthorUseCase>();
builder.Services.AddScoped<UpdateAuthorUseCase>();
builder.Services.AddScoped<DeleteAuthorUseCase>();

builder.Services.AddScoped<GetBooksUseCase>();
builder.Services.AddScoped<GetBookUseCase>();
builder.Services.AddScoped<CreateBookUseCase>();
builder.Services.AddScoped<UpdateBookUseCase>();
builder.Services.AddScoped<DeleteBookUseCase>();
builder.Services.AddScoped<GetBooksForAuthorUseCase>();
builder.Services.AddScoped<GetBookForAuthorUseCase>();
builder.Services.AddScoped<CreateBookForAuthorUseCase>();

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

// Protect all API endpoints with API key authentication
app.UseMiddleware<ApiKeyMiddleware>();

app.MapAuthorsEndpoints();
app.MapBooksEndpoints();

app.Run();
