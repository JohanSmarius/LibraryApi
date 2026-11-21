using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.Dtos;

namespace LibraryApi.Controllers.Queries.Books;

public record GetBookForAuthorQuery(int AuthorId, int BookId);

public class GetBookForAuthorQueryHandler(IBookRepository books)
{
    public Task<BookDto?> HandleAsync(GetBookForAuthorQuery query, CancellationToken cancellationToken)
    {
        return books.GetForAuthorAsync(query.AuthorId, query.BookId, cancellationToken);
    }
}
