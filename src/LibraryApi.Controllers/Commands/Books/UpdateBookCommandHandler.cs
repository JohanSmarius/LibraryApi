using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.Dtos;

namespace LibraryApi.Controllers.Commands.Books;

public record UpdateBookCommand(int Id, SaveBookDto Request);

public class UpdateBookCommandHandler(IAuthorRepository authors, IBookRepository books)
{
    public async Task<BookDto?> HandleAsync(UpdateBookCommand command, CancellationToken cancellationToken)
    {
        var id = command.Id;
        var request = command.Request;

        var book = await books.FindAsync(id, cancellationToken);
        if (book is null)
        {
            return null;
        }

        var author = await authors.FindAsync(request.AuthorId, cancellationToken);
        if (author is null)
        {
            return null;
        }

        book.Title = request.Title;
        book.PublicationYear = request.PublicationYear;
        book.Isbn = request.Isbn;
        book.AuthorId = request.AuthorId;
        book.Author = author;

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
