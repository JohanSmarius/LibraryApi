using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.UseCases.Books;

public class GetBookForAuthorUseCase(IBookRepository bookRepository)
{
    public async Task<BookDto?> ExecuteAsync(
        int authorId,
        int bookId,
        CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByAuthorIdAndBookIdAsync(authorId, bookId, cancellationToken);
        if (book is null)
        {
            return null;
        }

        return ToDto(book);
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
