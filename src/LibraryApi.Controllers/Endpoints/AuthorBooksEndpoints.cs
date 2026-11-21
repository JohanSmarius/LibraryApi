using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Commands.Books;
using LibraryApi.Controllers.Queries.Books;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryApi.Controllers.Endpoints;

public static class AuthorBooksEndpoints
{
    extension (RouteGroupBuilder authorsGroup)
    {
        public RouteGroupBuilder MapAuthorBooksEndpoints()
        {
            var group = authorsGroup.MapGroup("/{authorId:int}/books")
                .WithTags("Author Books");

            group.MapGet("", GetBooksForAuthor)
                .WithName(nameof(GetBooksForAuthor))
                .WithSummary("Gets all books for a given author.");

            group.MapGet("/{bookId:int}", GetBookForAuthor)
                .WithName(nameof(GetBookForAuthor))
                .WithSummary("Gets a single book that belongs to a given author.");

            group.MapPost("", CreateBookForAuthor)
                .WithName(nameof(CreateBookForAuthor))
                .WithSummary("Creates a new book under the given author.")
                .ProducesValidationProblem();

            return group;
        }
    }

    private static async Task<Results<Ok<List<BookDto>>, NotFound>> GetBooksForAuthor(
        int authorId,
        GetBooksForAuthorQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var books = await handler.HandleAsync(new GetBooksForAuthorQuery(authorId), cancellationToken);

        return books is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(books);
    }

    private static async Task<Results<Ok<BookDto>, NotFound>> GetBookForAuthor(
        int authorId,
        int bookId,
        GetBookForAuthorQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var book = await handler.HandleAsync(new GetBookForAuthorQuery(authorId, bookId), cancellationToken);

        return book is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(book);
    }

    private static async Task<Results<CreatedAtRoute<BookDto>, NotFound>> CreateBookForAuthor(
        int authorId,
        CreateBookDto request,
        CreateBookForAuthorCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var book = await handler.HandleAsync(
            new CreateBookForAuthorCommand(authorId, request), cancellationToken);

        if (book is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.CreatedAtRoute(
            book,
            nameof(GetBookForAuthor),
            new { authorId, bookId = book.Id });
    }
}
