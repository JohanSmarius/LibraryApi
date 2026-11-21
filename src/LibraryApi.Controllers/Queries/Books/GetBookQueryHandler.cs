using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.Dtos;

namespace LibraryApi.Controllers.Queries.Books;

public record GetBookQuery(int Id);

public class GetBookQueryHandler(IBookRepository books)
{
    public Task<BookDto?> HandleAsync(GetBookQuery query, CancellationToken cancellationToken)
    {
        return books.GetAsync(query.Id, cancellationToken);
    }
}
