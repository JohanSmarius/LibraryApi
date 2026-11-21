using LibraryApi.MinimalApi.Dtos;
using LibraryApi.MinimalApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryApi.MinimalApi.Registrations;

public static class AuthorsEndpoints
{
    extension(IEndpointRouteBuilder routes)
    {
        public RouteGroupBuilder MapAuthorsEndpoints()
        {
            var group = routes.MapGroup("/api/authors");

            group.MapGet("", async (IAuthorService authorService) =>
            {
                var authors = await authorService.GetAuthorsAsync();
                return TypedResults.Ok(authors);
            });

            group.MapGet("/{id:int}", async Task<Results<Ok<AuthorDto>, NotFound>> (
                int id,
                IAuthorService authorService) =>
            {
                var author = await authorService.GetAuthorAsync(id);
                return author is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(author);
            });

            group.MapPost("", async Task<CreatedAtRoute<AuthorDto>> (
                CreateAuthorDto request,
                IAuthorService authorService) =>
            {
                var author = await authorService.CreateAuthorAsync(request);
                return TypedResults.CreatedAtRoute(author, "GetAuthor", new { id = author.Id });
            });

            group.MapAuthorBooksEndpoints();
            
            return group;
        }
    }
}