using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Queries.Books;

public record GetBooksForAuthorQuery(int AuthorId);

public class GetBooksForAuthorQueryHandler(LibraryDbContext db)
{
    public async Task<List<BookDto>?> HandleAsync(GetBooksForAuthorQuery query, CancellationToken cancellationToken)
    {
        var authorId = query.AuthorId;

        var authorExists = await db.Authors
            .AsNoTracking()
            .AnyAsync(author => author.Id == authorId, cancellationToken);

        if (!authorExists)
        {
            return null;
        }

        return await db.Books
            .AsNoTracking()
            .Where(book => book.AuthorId == authorId)
            .Select(book => new BookDto(
                book.Id,
                book.Title,
                book.PublicationYear,
                book.Isbn,
                book.AuthorId,
                $"{book.Author!.FirstName} {book.Author.LastName}"))
            .ToListAsync(cancellationToken);
    }
}
