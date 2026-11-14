using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Repositories;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync(CancellationToken cancellationToken);
    Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<Book>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken);
    Task<Book?> GetByAuthorIdAndBookIdAsync(int authorId, int bookId, CancellationToken cancellationToken);
    Task AddAsync(Book book, CancellationToken cancellationToken);
    void Update(Book book);
    Task RemoveAsync(int id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
