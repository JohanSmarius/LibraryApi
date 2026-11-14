using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.UseCases.Books;

public class GetBooksUseCase(LibraryDbContext db)
{
    public Task<List<BookDto>> ExecuteAsync(CancellationToken cancellationToken)
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
