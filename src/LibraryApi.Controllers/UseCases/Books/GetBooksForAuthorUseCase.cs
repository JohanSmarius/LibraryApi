using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.UseCases.Books;

public class GetBooksForAuthorUseCase(LibraryDbContext db)
{
    public async Task<List<BookDto>?> ExecuteAsync(
        int authorId,
        CancellationToken cancellationToken)
    {
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
