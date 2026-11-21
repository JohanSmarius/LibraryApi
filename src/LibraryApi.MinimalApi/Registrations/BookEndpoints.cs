using LibraryApi.MinimalApi.Dtos;
using LibraryApi.MinimalApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryApi.MinimalApi.Registrations;



public static class BooksEndpoints
{
    extension(IEndpointRouteBuilder routes)
    {
        public RouteGroupBuilder MapBooksEndpoints()
        {
            var group = routes.MapGroup("/api/books");

            group.MapGet("", async (IBookService bookService, CancellationToken cancellationToken) =>
            {
                var books = await bookService.GetBooksAsync(cancellationToken);
                return TypedResults.Ok(books);
            });

            group.MapGet("/{id:int}", async Task<Results<Ok<BookDto>, NotFound>> (
                int id,
                IBookService bookService,
                CancellationToken cancellationToken) =>
            {
                var book = await bookService.GetBookAsync(id, cancellationToken);
                return book is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(book);
            });

            group.MapPost("", async Task<Results<CreatedAtRoute<BookDto>, NotFound>> (
                SaveBookDto request,
                IBookService bookService,
                CancellationToken cancellationToken) =>
            {
                var book = await bookService.CreateBookAsync(request, cancellationToken);
                if (book is null)
                {
                    return TypedResults.NotFound();
                }

                return TypedResults.CreatedAtRoute(book, "GetBook", new { id = book.Id });
            });

            group.MapPut("/{id:int}", async Task<Results<Ok<BookDto>, NotFound>> (
                int id,
                SaveBookDto request,
                IBookService bookService,
                CancellationToken cancellationToken) =>
            {
                var book = await bookService.UpdateBookAsync(id, request, cancellationToken);
                return book is null
                    ? TypedResults.NotFound()
                    : TypedResults.Ok(book);
            });

            group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound>> (
                int id,
                IBookService bookService,
                CancellationToken cancellationToken) =>
            {
                var deleted = await bookService.DeleteBookAsync(id, cancellationToken);
                return deleted
                    ? TypedResults.NoContent()
                    : TypedResults.NotFound();
            });

            return group;
        }
    }
}
