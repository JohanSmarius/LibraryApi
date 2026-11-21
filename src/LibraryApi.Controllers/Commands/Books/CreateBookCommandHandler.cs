using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Commands.Books;

public record CreateBookCommand(SaveBookDto Request);

public class CreateBookCommandHandler(IAuthorRepository authors, IBookRepository books)
{
    public async Task<BookDto?> HandleAsync(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var author = await authors.FindAsync(request.AuthorId, cancellationToken);
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

        books.Add(book);
        await books.SaveChangesAsync(cancellationToken);

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
