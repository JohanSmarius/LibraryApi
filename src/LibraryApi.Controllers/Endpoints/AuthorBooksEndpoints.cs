using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Services;
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
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        var books = await bookService.GetBooksForAuthorAsync(authorId, cancellationToken);

        return books is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(books);
    }

    private static async Task<Results<Ok<BookDto>, NotFound>> GetBookForAuthor(
        int authorId,
        int bookId,
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        var book = await bookService.GetBookForAuthorAsync(authorId, bookId, cancellationToken);

        return book is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(book);
    }

    private static async Task<Results<CreatedAtRoute<BookDto>, NotFound>> CreateBookForAuthor(
        int authorId,
        CreateBookDto request,
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        var book = await bookService.CreateBookForAuthorAsync(
            authorId,
            request,
            cancellationToken);

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
