using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.UseCases.Books;

public class CreateBookUseCase(LibraryDbContext db)
{
    public async Task<BookDto?> ExecuteAsync(
        SaveBookDto request,
        CancellationToken cancellationToken)
    {
        var author = await db.Authors.FindAsync([request.AuthorId], cancellationToken);
        if (author is null)
        {
            return null;
        }

        var book = new Book
        {
            Title = request.Title,
            PublicationYear = request.PublicationYear,
            Isbn = request.Isbn,
            AuthorId = request.AuthorId,
            Author = author
        };

        db.Books.Add(book);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(book);
    }

    private static BookDto ToDto(Book book)
    {
        return new BookDto(
            book.Id,
            book.Title,
            book.PublicationYear,
            book.Isbn,
            book.AuthorId,
            $"{book.Author!.FirstName} {book.Author.LastName}");
    }
}
