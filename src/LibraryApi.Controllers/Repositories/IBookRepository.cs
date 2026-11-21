using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Repositories;

public interface IBookRepository
{
    Task<List<BookDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<BookDto?> GetAsync(int id, CancellationToken cancellationToken);
    Task<List<BookDto>> GetForAuthorAsync(int authorId, CancellationToken cancellationToken);
    Task<BookDto?> GetForAuthorAsync(int authorId, int bookId, CancellationToken cancellationToken);
    Task<Book?> FindAsync(int id, CancellationToken cancellationToken);
    void Add(Book book);
    void Remove(Book book);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}