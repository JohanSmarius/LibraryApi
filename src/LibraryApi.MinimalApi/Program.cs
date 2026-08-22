using LibraryApi.MinimalApi.Data;
using LibraryApi.MinimalApi.Dtos;
using LibraryApi.MinimalApi.Handlers;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LibraryDb")
                       ?? "Data Source=library.db";

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddTransient<IAuthorHandler, AuthorHandler>();


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

app.MapGet("/api/authors", async ([FromServices] IAuthorHandler handler) => await handler.GetAuthorsAsync())
    .Produces<List<AuthorDto>>();


app.Run();




