using LibraryApi.MinimalApi.Data;
using LibraryApi.MinimalApi.Dtos;
using LibraryApi.MinimalApi.Entities;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var group = app.MapGroup("/api/authors");

group.MapGet("", GetAuthorsAsync);

group.MapGet("{id}", GetAuthor);

group.MapPost("", AddAuthorAsync);


app.Run();

static async Task<Ok<List<AuthorDto>>> GetAuthorsAsync(LibraryDbContext db)
{
    var result = await db.Authors
        .Include(a => a.Books)
        .Select(a => new AuthorDto(a.Id, a.FirstName, a.LastName, a.Country, a.Books.Count))
        .ToListAsync();
    return TypedResults.Ok(result);
}

static async Task<Results<Ok<AuthorDto>, NotFound>> GetAuthor(int id, LibraryDbContext db)
{
    var author = await db.Authors
        .Include(a => a.Books)
        .FirstOrDefaultAsync(a => a.Id == id);

    if (author is null)
    {
        return TypedResults.NotFound();
    }

    return TypedResults.Ok(new AuthorDto(author.Id, author.FirstName, author.LastName, author.Country, author.Books.Count));
}

static async Task<Created<AuthorDto>> AddAuthorAsync([FromBody] CreateAuthorDto authorToAdd, [FromServices] LibraryDbContext db)
{
    var author = new Author
    {
        FirstName = authorToAdd.FirstName,
        LastName = authorToAdd.LastName,
        Country = authorToAdd.Country
    };
 
    db.Authors.Add(author);
    await db.SaveChangesAsync();
 
    var dto = new AuthorDto(author.Id, author.FirstName, author.LastName, author.Country, 0);
    return TypedResults.Created($"/api/authors/{author.Id}", dto);
}



