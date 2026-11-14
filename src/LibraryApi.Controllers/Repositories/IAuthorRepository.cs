using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Repositories;

public interface IAuthorRepository
{
    Task<List<Author>> GetAllAsync(CancellationToken cancellationToken);
    Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Author?> GetByIdWithBooksAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(Author author, CancellationToken cancellationToken);
    void Update(Author author);
    Task RemoveAsync(int id, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
