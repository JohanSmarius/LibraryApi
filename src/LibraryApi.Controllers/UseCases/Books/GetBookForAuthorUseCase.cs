using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.UseCases.Books;

public class GetBookForAuthorUseCase(LibraryDbContext db)
{
    public Task<BookDto?> ExecuteAsync(
        int authorId,
        int bookId,
        CancellationToken cancellationToken)
    {
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
