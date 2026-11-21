using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.Dtos;

namespace LibraryApi.Controllers.Queries.Books;

public record GetBooksQuery();

public class GetBooksQueryHandler(IBookRepository books)
{
    public Task<List<BookDto>> HandleAsync(GetBooksQuery query, CancellationToken cancellationToken)
    {
        return books.GetAllAsync(cancellationToken);
    }
}
