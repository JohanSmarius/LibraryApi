using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Endpoints;

public static class BooksEndpoints
{
    extension (IEndpointRouteBuilder routes)
    {
        public RouteGroupBuilder MapBooksEndpoints()
        {
            var group = routes.MapGroup("/api/books")
                .WithTags("Books");

            group.MapGet("", GetBooks)
                .WithName(nameof(GetBooks))
                .WithSummary("Gets all books.");

            group.MapGet("/{id:int}", GetBook)
                .WithName(nameof(GetBook))
                .WithSummary("Gets a single book by id.");

            group.MapPost("", CreateBook)
                .WithName(nameof(CreateBook))
                .WithSummary("Creates a book.")
                .ProducesValidationProblem();

            group.MapPut("/{id:int}", UpdateBook)
                .WithName(nameof(UpdateBook))
                .WithSummary("Updates a book.")
                .ProducesValidationProblem();

            group.MapDelete("/{id:int}", DeleteBook)
                .WithName(nameof(DeleteBook))
                .WithSummary("Deletes a book by id.");

            return group;
        }
    }

    private static async Task<Ok<List<BookDto>>> GetBooks(
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var books = await db.Books
            .AsNoTracking()
            .Select(book => book.ToDto())
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(books);
    }

    private static async Task<Results<Ok<BookDto>, NotFound>> GetBook(
        int id,
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var book = await db.Books
            .AsNoTracking()
            .Where(book => book.Id == id)
            .Select(book => book.ToDto())
            .FirstOrDefaultAsync(cancellationToken);

        return book is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(book);
    }

    private static async Task<Results<CreatedAtRoute<BookDto>, NotFound>> CreateBook(
        SaveBookDto request,
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var author = await db.Authors.FindAsync([request.AuthorId], cancellationToken);
        if (author is null)
        {
            return TypedResults.NotFound();
        }

        var book = new Book
        {
            Title = request.Title,
            PublicationYear = request.PublicationYear,
            Isbn = request.Isbn,
            AuthorId = request.AuthorId,
            Author = author
        };

        db.Books.Add(book);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.CreatedAtRoute(book.ToDto(), nameof(GetBook), new { id = book.Id });
    }

    private static async Task<Results<Ok<BookDto>, NotFound>> UpdateBook(
        int id,
        SaveBookDto request,
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var book = await db.Books.FindAsync([id], cancellationToken);
        if (book is null)
        {
            return TypedResults.NotFound();
        }

        var author = await db.Authors.FindAsync([request.AuthorId], cancellationToken);
        if (author is null)
        {
            return TypedResults.NotFound();
        }

        book.Title = request.Title;
        book.PublicationYear = request.PublicationYear;
        book.Isbn = request.Isbn;
        book.AuthorId = request.AuthorId;
        book.Author = author;

        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(book.ToDto());
    }

    private static async Task<Results<NoContent, NotFound>> DeleteBook(
        int id,
        LibraryDbContext db,
        CancellationToken cancellationToken)
    {
        var book = await db.Books.FindAsync([id], cancellationToken);
        if (book is null)
        {
            return TypedResults.NotFound();
        }

        db.Books.Remove(book);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
