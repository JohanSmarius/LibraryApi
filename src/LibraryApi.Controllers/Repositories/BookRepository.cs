using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Repositories;

public class BookRepository(LibraryDbContext db) : IBookRepository
{
    private static IQueryable<BookDto> ReadBooks(IQueryable<Book> books) => books.AsNoTracking()
        .Select(book => new BookDto(book.Id, book.Title, book.PublicationYear, book.Isbn, book.AuthorId,
            $"{book.Author!.FirstName} {book.Author.LastName}"));

    public Task<List<BookDto>> GetAllAsync(CancellationToken cancellationToken) => ReadBooks(db.Books).ToListAsync(cancellationToken);
    public Task<BookDto?> GetAsync(int id, CancellationToken cancellationToken) =>
        ReadBooks(db.Books.Where(book => book.Id == id)).FirstOrDefaultAsync(cancellationToken);
    public Task<List<BookDto>> GetForAuthorAsync(int authorId, CancellationToken cancellationToken) =>
        ReadBooks(db.Books.Where(book => book.AuthorId == authorId)).ToListAsync(cancellationToken);
    public Task<BookDto?> GetForAuthorAsync(int authorId, int bookId, CancellationToken cancellationToken) =>
        ReadBooks(db.Books.Where(book => book.Id == bookId && book.AuthorId == authorId)).FirstOrDefaultAsync(cancellationToken);
    public async Task<Book?> FindAsync(int id, CancellationToken cancellationToken) =>
        await db.Books.FindAsync([id], cancellationToken);
    public void Add(Book book) => db.Books.Add(book);
    public void Remove(Book book) => db.Books.Remove(book);
    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await db.SaveChangesAsync(cancellationToken);
}