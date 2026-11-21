using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.Dtos;

namespace LibraryApi.Controllers.Queries.Books;

public record GetBooksForAuthorQuery(int AuthorId);

public class GetBooksForAuthorQueryHandler(IAuthorRepository authors, IBookRepository books)
{
    public async Task<List<BookDto>?> HandleAsync(GetBooksForAuthorQuery query, CancellationToken cancellationToken)
    {
        var authorId = query.AuthorId;

        var authorExists = await authors.ExistsAsync(authorId, cancellationToken);

        if (!authorExists)
        {
            return null;
        }

        return await books.GetForAuthorAsync(authorId, cancellationToken);
    }
}
