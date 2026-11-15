using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.UseCases.Books;

public class CreateBookForAuthorUseCase(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository)
{
    public async Task<BookDto?> ExecuteAsync(
        int authorId,
        CreateBookDto request,
        CancellationToken cancellationToken)
    {
        var author = await authorRepository.GetByIdAsync(authorId, cancellationToken);
        if (author is null)
        {
            return null;
        }

        var book = new Book
        {
            Title = request.Title,
            PublicationYear = request.PublicationYear,
            Isbn = request.Isbn,
            AuthorId = authorId
        };

        await bookRepository.AddAsync(book, cancellationToken);

        return ToDto(book, author);
    }

    private static BookDto ToDto(Book book, Author author)
    {
        return new BookDto(
            book.Id,
            book.Title,
            book.PublicationYear,
            book.Isbn,
            book.AuthorId,
            $"{author.FirstName} {author.LastName}");
    }
}
