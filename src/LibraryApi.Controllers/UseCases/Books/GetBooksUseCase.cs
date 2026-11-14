using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.UseCases.Books;

public class GetBooksUseCase(IBookRepository bookRepository)
{
    public async Task<List<BookDto>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var books = await bookRepository.GetAllAsync(cancellationToken);
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
