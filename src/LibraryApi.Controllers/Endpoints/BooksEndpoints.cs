using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Services;
using Microsoft.AspNetCore.Http.HttpResults;

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
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        var books = await bookService.GetBooksAsync(cancellationToken);

        return TypedResults.Ok(books);
    }

    private static async Task<Results<Ok<BookDto>, NotFound>> GetBook(
        int id,
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        var book = await bookService.GetBookAsync(id, cancellationToken);

        return book is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(book);
    }

    private static async Task<Results<CreatedAtRoute<BookDto>, NotFound>> CreateBook(
        SaveBookDto request,
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        var book = await bookService.CreateBookAsync(request, cancellationToken);
        if (book is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.CreatedAtRoute(book, nameof(GetBook), new { id = book.Id });
    }

    private static async Task<Results<Ok<BookDto>, NotFound>> UpdateBook(
        int id,
        SaveBookDto request,
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        var book = await bookService.UpdateBookAsync(id, request, cancellationToken);

        return book is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(book);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteBook(
        int id,
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        var deleted = await bookService.DeleteBookAsync(id, cancellationToken);

        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}
