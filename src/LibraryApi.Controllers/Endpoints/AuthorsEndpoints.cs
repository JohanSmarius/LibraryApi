using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

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
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var authors = await db.Authors
            .AsNoTracking()
            .Select(author => author.ToDto())
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(authors);
    }

    private static async Task<Results<Ok<AuthorDto>, NotFound>> GetAuthor(
        int id,
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var author = await db.Authors
            .AsNoTracking()
            .Where(author => author.Id == id)
            .Select(author => author.ToDto())
            .FirstOrDefaultAsync(cancellationToken);

        return author is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(author);
    }

    private static async Task<CreatedAtRoute<AuthorDto>> CreateAuthor(
        CreateAuthorDto request,
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var author = new Author
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Country = request.Country
        };

        db.Authors.Add(author);
        await db.SaveChangesAsync(cancellationToken);

        var dto = author.ToDto();
        return TypedResults.CreatedAtRoute(dto, nameof(GetAuthor), new { id = author.Id });
    }

    private static async Task<Results<Ok<AuthorDto>, NotFound>> UpdateAuthor(
        int id,
        UpdateAuthorDto request,
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var author = await db.Authors
            .Include(author => author.Books)
            .FirstOrDefaultAsync(author => author.Id == id, cancellationToken);

        if (author is null)
        {
            return TypedResults.NotFound();
        }

        author.FirstName = request.FirstName;
        author.LastName = request.LastName;
        author.Country = request.Country;

        await db.SaveChangesAsync(cancellationToken);

        var dto = author.ToDto();
        return TypedResults.Ok(dto);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteAuthor(
        int id,
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var author = await db.Authors.FindAsync([id], cancellationToken);
        if (author is null)
        {
            return TypedResults.NotFound();
        }

        db.Authors.Remove(author);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
