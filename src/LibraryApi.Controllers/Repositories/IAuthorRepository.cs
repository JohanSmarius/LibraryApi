using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Repositories;

public interface IAuthorRepository
{
    Task<List<AuthorDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<AuthorDto?> GetAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    Task<Author?> FindAsync(int id, CancellationToken cancellationToken);
    Task<Author?> FindWithBooksAsync(int id, CancellationToken cancellationToken);
    void Add(Author author);
    void Remove(Author author);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}