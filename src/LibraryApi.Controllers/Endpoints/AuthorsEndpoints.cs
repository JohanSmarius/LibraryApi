using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryApi.Controllers.Endpoints;

public static class AuthorsEndpoints
{
    extension (IEndpointRouteBuilder routes)
    {
        public RouteGroupBuilder MapAuthorsEndpoints()
        {
            var group = routes.MapGroup("/api/authors")
                .WithTags("Authors");

            group.MapGet("", GetAuthors)
                .WithName(nameof(GetAuthors))
                .WithSummary("Gets all authors.");

            group.MapGet("/{id:int}", GetAuthor)
                .WithName(nameof(GetAuthor))
                .WithSummary("Gets a single author by id.");

            group.MapPost("", CreateAuthor)
                .WithName(nameof(CreateAuthor))
                .WithSummary("Creates a new author.")
                .ProducesValidationProblem();

            group.MapPut("/{id:int}", UpdateAuthor)
                .WithName(nameof(UpdateAuthor))
                .WithSummary("Updates an author.")
                .ProducesValidationProblem();

            group.MapDelete("/{id:int}", DeleteAuthor)
                .WithName(nameof(DeleteAuthor))
                .WithSummary("Deletes an author and all of their books.");

            group.MapAuthorBooksEndpoints();

            return group;
        }
    }

    private static async Task<Ok<List<AuthorDto>>> GetAuthors(
        IAuthorService authorService,
        CancellationToken cancellationToken)
    {
        var authors = await authorService.GetAuthorsAsync(cancellationToken);

        return TypedResults.Ok(authors);
    }

    private static async Task<Results<Ok<AuthorDto>, NotFound>> GetAuthor(
        int id,
        IAuthorService authorService,
        CancellationToken cancellationToken)
    {
        var author = await authorService.GetAuthorAsync(id, cancellationToken);

        return author is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(author);
    }

    private static async Task<CreatedAtRoute<AuthorDto>> CreateAuthor(
        CreateAuthorDto request,
        IAuthorService authorService,
        CancellationToken cancellationToken)
    {
        var author = await authorService.CreateAuthorAsync(request, cancellationToken);
        return TypedResults.CreatedAtRoute(author, nameof(GetAuthor), new { id = author.Id });
    }

    private static async Task<Results<Ok<AuthorDto>, NotFound>> UpdateAuthor(
        int id,
        UpdateAuthorDto request,
        IAuthorService authorService,
        CancellationToken cancellationToken)
    {
        var author = await authorService.UpdateAuthorAsync(id, request, cancellationToken);

        if (author is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(author);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteAuthor(
        int id,
        IAuthorService authorService,
        CancellationToken cancellationToken)
    {
        var deleted = await authorService.DeleteAuthorAsync(id, cancellationToken);
        if (!deleted)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }
}
