using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

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
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var authorExists = await db.Authors
            .AsNoTracking()
            .AnyAsync(author => author.Id == authorId, cancellationToken);

        if (!authorExists)
        {
            return TypedResults.NotFound();
        }

        var books = await db.Books
            .AsNoTracking()
            .Where(book => book.AuthorId == authorId)
            .Select(book => book.ToDto())
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(books);
    }

    private static async Task<Results<Ok<BookDto>, NotFound>> GetBookForAuthor(
        int authorId,
        int bookId,
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var book = await db.Books
            .AsNoTracking()
            .Where(book => book.Id == bookId && book.AuthorId == authorId)
            .Select(book => book.ToDto())
            .FirstOrDefaultAsync(cancellationToken);

        return book is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(book);
    }

    private static async Task<Results<CreatedAtRoute<BookDto>, NotFound>> CreateBookForAuthor(
        int authorId,
        CreateBookDto request,
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var author = await db.Authors.FindAsync([authorId], cancellationToken);
        if (author is null)
        {
            return TypedResults.NotFound();
        }

        var book = new Book
        {
            Title = request.Title,
            PublicationYear = request.PublicationYear,
            Isbn = request.Isbn,
            AuthorId = authorId,
            Author = author
        };

        db.Books.Add(book);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.CreatedAtRoute(
            book.ToDto(),
            nameof(GetBookForAuthor),
            new { authorId, bookId = book.Id });
    }
}
