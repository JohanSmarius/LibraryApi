using LibraryApi.MinimalApi.Dtos;
using LibraryApi.MinimalApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryApi.MinimalApi.Endpoints;

public static class AuthorBooksEndpoints
{
    extension(RouteGroupBuilder authorsGroup)
    {
        public RouteGroupBuilder MapAuthorBooksEndpoints()
        {
            var booksAuthor = authorsGroup.MapGroup("/{authorId:int}/books").WithTags("Books");

            booksAuthor.MapGet("",
                async Task<Results<Ok<List<BookDto>>, NotFound>> (int authorId, IBookService service) =>
                {
                    var result = await service.GetBooksForAuthorAsync(authorId);
                    return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
                });

            booksAuthor.MapGet("/{bookId:int}",
                async Task<Results<Ok<BookDto>, NotFound>> (int authorId, int bookId, IBookService service) =>
                {
                    var result = await service.GetBookForAuthorAsync(authorId, bookId);
                    return result is null ? TypedResults.NotFound() : TypedResults.Ok(result);
                });

            booksAuthor.MapPost("",
                async Task<Results<Created<BookDto>, NotFound>> (int authorId, CreateBookDto request,
                    IBookService service) =>
                {
                    var result = await service.CreateBookForAuthorAsync(authorId, request);
                    return result is null
                        ? TypedResults.NotFound()
                        : TypedResults.Created($"/api/authors/{authorId}/books/{result.Id}", result);
                });

            return booksAuthor;
        }
    }

}