using LibraryApi.MinimalApi.Data;
using LibraryApi.MinimalApi.Dtos;
using LibraryApi.MinimalApi.Entities;
using LibraryApi.MinimalApi.Services;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LibraryDb")
                       ?? "Data Source=library.db";

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(connectionString));


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddValidation();

builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var authors = app.MapGroup("/api/authors");

authors.MapGet("", async (IAuthorService service) =>
    TypedResults.Ok(await service.GetAuthorsAsync()));
 
authors.MapGet("/{id:int}", async Task<Results<Ok<AuthorDto>, NotFound>> (int id, IAuthorService service) =>
{
    var author = await service.GetAuthorAsync(id);
    return author is null ? TypedResults.NotFound() : TypedResults.Ok(author);
});
 
authors.MapPost("", async (CreateAuthorDto request, IAuthorService service) =>
{
    var dto = await service.CreateAuthorAsync(request);
    return TypedResults.Created($"/api/authors/{dto.Id}", dto);
});

var books = authors.MapGroup("/{authorId:int}/books").WithTags("Books");
 
books.MapGet("", async Task<Results<Ok<List<BookDto>>, NotFound>> (int authorId, IBookService service) =>
{
    var result = await service.GetBooksForAuthorAsync(authorId);
    return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
});
 
books.MapGet("/{bookId:int}", async Task<Results<Ok<BookDto>, NotFound>> (int authorId, int bookId, IBookService service) =>
{
    var result = await service.GetBookForAuthorAsync(authorId, bookId);
    return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
});
 
books.MapPost("", async Task<Results<Created<BookDto>, NotFound>> (int authorId, CreateBookDto request, IBookService service) =>
{
    var result = await service.CreateBookForAuthorAsync(authorId, request);
    return result is null
        ? TypedResults.NotFound()
        : TypedResults.Created($"/api/authors/{authorId}/books/{result.Id}", result);
});


app.Run();
