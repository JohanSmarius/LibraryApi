using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Repositories;

public class BookRepository(LibraryDbContext dbContext) : IBookRepository
{
    public async Task<List<Book>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Books
            .AsNoTracking()
            .Include(book => book.Author)
            .ToListAsync(cancellationToken);
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Books
            .AsNoTracking()
            .Include(book => book.Author)
            .FirstOrDefaultAsync(book => book.Id == id, cancellationToken);
    }

    public async Task<List<Book>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken)
    {
        return await dbContext.Books
            .AsNoTracking()
            .Include(book => book.Author)
            .Where(book => book.AuthorId == authorId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Book?> GetByAuthorIdAndBookIdAsync(int authorId, int bookId, CancellationToken cancellationToken)
    {
        return await dbContext.Books
            .AsNoTracking()
            .Include(book => book.Author)
            .Where(book => book.Id == bookId && book.AuthorId == authorId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Book book, CancellationToken cancellationToken)
    {
        dbContext.Books.Add(book);
        await SaveChangesAsync(cancellationToken);
    }

    public void Update(Book book)
    {
        dbContext.Books.Update(book);
    }

    public async Task RemoveAsync(int id, CancellationToken cancellationToken)
    {
        var book = await dbContext.Books.FindAsync([id], cancellationToken);
        if (book is not null)
        {
            dbContext.Books.Remove(book);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
