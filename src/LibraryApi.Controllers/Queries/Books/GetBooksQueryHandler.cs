using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Queries.Books;

public record GetBooksQuery();

public class GetBooksQueryHandler(LibraryDbContext db)
{
    public Task<List<BookDto>> HandleAsync(GetBooksQuery query, CancellationToken cancellationToken)
    {
        return db.Books
            .AsNoTracking()
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
