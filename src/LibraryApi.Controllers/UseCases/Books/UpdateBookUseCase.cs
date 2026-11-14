using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.UseCases.Books;

public class UpdateBookUseCase(LibraryDbContext db)
{
    public async Task<BookDto?> ExecuteAsync(
        int id,
        SaveBookDto request,
        CancellationToken cancellationToken)
    {
        var book = await db.Books.FindAsync([id], cancellationToken);
        if (book is null)
        {
            return null;
        }

        var author = await db.Authors.FindAsync([request.AuthorId], cancellationToken);
        if (author is null)
        {
            return null;
        }

        book.Title = request.Title;
        book.PublicationYear = request.PublicationYear;
        book.Isbn = request.Isbn;
        book.AuthorId = request.AuthorId;
        book.Author = author;

        await db.SaveChangesAsync(cancellationToken);

        return new BookDto(
            book.Id,
            book.Title,
            book.PublicationYear,
            book.Isbn,
            book.AuthorId,
            $"{book.Author!.FirstName} {book.Author.LastName}");
    }
}
