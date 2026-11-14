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
            AuthorId = authorId,
            Author = author
        };

        await bookRepository.AddAsync(book, cancellationToken);

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
