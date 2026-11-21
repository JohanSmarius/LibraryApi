using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Queries.Books;

public record GetBookQuery(int Id);

public class GetBookQueryHandler(LibraryDbContext db)
{
    public Task<BookDto?> HandleAsync(GetBookQuery query, CancellationToken cancellationToken)
    {
        var id = query.Id;

        return db.Books
            .AsNoTracking()
            .Where(book => book.Id == id)
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
