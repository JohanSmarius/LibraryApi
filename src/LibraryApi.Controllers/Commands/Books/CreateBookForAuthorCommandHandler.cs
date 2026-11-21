using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Commands.Books;

public record CreateBookForAuthorCommand(int AuthorId, CreateBookDto Request);

public class CreateBookForAuthorCommandHandler(IAuthorRepository authors, IBookRepository books)
{
    public async Task<BookDto?> HandleAsync(CreateBookForAuthorCommand command, CancellationToken cancellationToken)
    {
        var authorId = command.AuthorId;
        var request = command.Request;

        var author = await authors.FindAsync(authorId, cancellationToken);
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

        books.Add(book);
        await books.SaveChangesAsync(cancellationToken);

        return new BookDto(
            book.Id,
            book.Title,
            book.PublicationYear,
            book.Isbn,
            book.AuthorId,
            $"{book.Author!.FirstName} {book.Author.LastName}");
    }
}
