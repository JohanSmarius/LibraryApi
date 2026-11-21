using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Commands.Books;

public record CreateBookForAuthorCommand(int AuthorId, CreateBookDto Request);

public class CreateBookForAuthorCommandHandler(LibraryDbContext db)
{
    public async Task<BookDto?> HandleAsync(CreateBookForAuthorCommand command, CancellationToken cancellationToken)
    {
        var authorId = command.AuthorId;
        var request = command.Request;

        var author = await db.Authors.FindAsync([authorId], cancellationToken);
        if (author is null)
        {
            return null;
        }

        var book = new Book
        {
            Title = request.Title,
            PublicationYear = request.PublicationYear,
            Isbn = request.Isbn,
            AuthorId = authorId,
            Author = author
        };

        db.Books.Add(book);
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
