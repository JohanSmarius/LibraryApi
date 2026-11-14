using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Repositories;

public class AuthorRepository(LibraryDbContext dbContext) : IAuthorRepository
{
    public async Task<List<Author>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Authors
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Authors
            .AsNoTracking()
            .FirstOrDefaultAsync(author => author.Id == id, cancellationToken);
    }

    public async Task<Author?> GetByIdWithBooksAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Authors
            .AsNoTracking()
            .Include(author => author.Books)
            .FirstOrDefaultAsync(author => author.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Authors
            .AsNoTracking()
            .AnyAsync(author => author.Id == id, cancellationToken);
    }

    public async Task AddAsync(Author author, CancellationToken cancellationToken)
    {
        dbContext.Authors.Add(author);
        await SaveChangesAsync(cancellationToken);
    }

    public void Update(Author author)
    {
        dbContext.Authors.Update(author);
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken)
    {
        var author = await dbContext.Authors.FindAsync([id], cancellationToken);
        if (author is not null)
        {
            dbContext.Authors.Remove(author);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
