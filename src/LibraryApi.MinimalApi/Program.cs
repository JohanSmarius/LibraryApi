using LibraryApi.MinimalApi.Data;
using LibraryApi.MinimalApi.Dtos;
using Microsoft.AspNetCore.Builder.Extensions;
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/authors", GetAuthorsAsync).Produces<List<AuthorDto>>();

app.Run();

static async Task<IResult> GetAuthorsAsync(LibraryDbContext db)
{
    var result = await db.Authors
        .Include(a => a.Books)
        .Select(a => new AuthorDto(a.Id, a.FirstName, a.LastName, a.Country, a.Books.Count))
        .ToListAsync();
    return Results.Ok(result);
}


