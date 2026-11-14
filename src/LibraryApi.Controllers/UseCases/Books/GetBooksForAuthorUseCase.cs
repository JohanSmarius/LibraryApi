using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.UseCases.Books;

public class GetBooksForAuthorUseCase(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository)
{
    public async Task<List<BookDto>?> ExecuteAsync(
        int authorId,
        CancellationToken cancellationToken)
    {
        var authorExists = await authorRepository.ExistsAsync(authorId, cancellationToken);

        if (!authorExists)
        {
            return null;
        }

        var books = await bookRepository.GetByAuthorIdAsync(authorId, cancellationToken);
        return books.Select(ToDto).ToList();
    }

    private static BookDto ToDto(LibraryApi.Controllers.Entities.Book book)
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
