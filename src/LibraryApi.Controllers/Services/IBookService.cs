using LibraryApi.Controllers.Dtos;

namespace LibraryApi.Controllers.Services;

public interface IBookService
{
    Task<List<BookDto>> GetBooksAsync(CancellationToken cancellationToken);

    Task<BookDto?> GetBookAsync(int id, CancellationToken cancellationToken);

    Task<BookDto?> CreateBookAsync(SaveBookDto request, CancellationToken cancellationToken);

    Task<BookDto?> UpdateBookAsync(int id, SaveBookDto request, CancellationToken cancellationToken);

    Task<bool> DeleteBookAsync(int id, CancellationToken cancellationToken);

    Task<List<BookDto>?> GetBooksForAuthorAsync(int authorId, CancellationToken cancellationToken);

    Task<BookDto?> GetBookForAuthorAsync(int authorId, int bookId, CancellationToken cancellationToken);

    Task<BookDto?> CreateBookForAuthorAsync(
        int authorId,
        CreateBookDto request,
        CancellationToken cancellationToken);
}
