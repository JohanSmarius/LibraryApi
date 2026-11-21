using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Queries.Books;

public record GetBookForAuthorQuery(int AuthorId, int BookId);

public class GetBookForAuthorQueryHandler(LibraryDbContext db)
{
    public Task<BookDto?> HandleAsync(GetBookForAuthorQuery query, CancellationToken cancellationToken)
    {
        var authorId = query.AuthorId;
        var bookId = query.BookId;

        return db.Books
            .AsNoTracking()
            .Where(book => book.Id == bookId && book.AuthorId == authorId)
            .Select(book => new BookDto(
                book.Id,
                book.Title,
                book.PublicationYear,
                book.Isbn,
                book.AuthorId,
                $"{book.Author!.FirstName} {book.Author.LastName}"))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
