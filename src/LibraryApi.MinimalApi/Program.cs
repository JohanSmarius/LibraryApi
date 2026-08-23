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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var group = app.MapGroup("/api/authors");

group.MapGet("", async (IAuthorService service) =>
    TypedResults.Ok(await service.GetAuthorsAsync()));
 
group.MapGet("/{id:int}", async Task<Results<Ok<AuthorDto>, NotFound>> (int id, IAuthorService service) =>
{
    var author = await service.GetAuthorAsync(id);
    return author is null ? TypedResults.NotFound() : TypedResults.Ok(author);
});
 
group.MapPost("", async (CreateAuthorDto request, IAuthorService service) =>
{
    var dto = await service.CreateAuthorAsync(request);
    return TypedResults.Created($"/api/authors/{dto.Id}", dto);
});

app.Run();
