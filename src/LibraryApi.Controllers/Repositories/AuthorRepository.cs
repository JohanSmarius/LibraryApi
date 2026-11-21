using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Repositories;

public class AuthorRepository(LibraryDbContext db) : IAuthorRepository
{
    public Task<List<AuthorDto>> GetAllAsync(CancellationToken cancellationToken) => db.Authors
        .AsNoTracking()
        .Select(author => new AuthorDto(author.Id, author.FirstName, author.LastName, author.Country, author.Books.Count))
        .ToListAsync(cancellationToken);

    public Task<AuthorDto?> GetAsync(int id, CancellationToken cancellationToken) => db.Authors
        .AsNoTracking()
        .Where(author => author.Id == id)
        .Select(author => new AuthorDto(author.Id, author.FirstName, author.LastName, author.Country, author.Books.Count))
        .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) => db.Authors
        .AsNoTracking().AnyAsync(author => author.Id == id, cancellationToken);

    public async Task<Author?> FindAsync(int id, CancellationToken cancellationToken) =>
        await db.Authors.FindAsync([id], cancellationToken);

    public Task<Author?> FindWithBooksAsync(int id, CancellationToken cancellationToken) => db.Authors
        .Include(author => author.Books)
        .FirstOrDefaultAsync(author => author.Id == id, cancellationToken);

    public void Add(Author author) => db.Authors.Add(author);
    public void Remove(Author author) => db.Authors.Remove(author);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await db.SaveChangesAsync(cancellationToken);
}