using LibraryApi.MinimalApi.Data;
using LibraryApi.MinimalApi.Dtos;
using LibraryApi.MinimalApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.MinimalApi.Services;

public interface IBookService
{
    Task<List<BookDto>?> GetBooksForAuthorAsync(int authorId);
    Task<BookDto?> GetBookForAuthorAsync(int authorId, int bookId);
    Task<BookDto?> CreateBookForAuthorAsync(int authorId, CreateBookDto request);
}
 
public class BookService : IBookService
{
    private readonly LibraryDbContext _db;
    public BookService(LibraryDbContext db) => _db = db;
 
    public async Task<List<BookDto>?> GetBooksForAuthorAsync(int authorId)
    {
        var author = await _db.Authors.FindAsync(authorId);
        if (author is null) return null;
 
        return await _db.Books.Include(b => b.Author)
            .Where(b => b.AuthorId == authorId)
            .Select(b => new BookDto(b.Id, b.Title, b.PublicationYear, b.Isbn, b.AuthorId,
                b.Author!.FirstName + " " + b.Author.LastName))
            .ToListAsync();
    }

    public async Task<BookDto?> GetBookForAuthorAsync(int authorId, int bookId)
    {
        var book = await _db.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == bookId && b.AuthorId == authorId);

        if (book is null) return null;

        return new BookDto(book.Id, book.Title, book.PublicationYear, book.Isbn, book.AuthorId,
            book.Author!.FirstName + " " + book.Author.LastName);
    }

    public async Task<BookDto?> CreateBookForAuthorAsync(int authorId, CreateBookDto request)
    {
        var author = await _db.Authors.FindAsync(authorId);
        if (author is null) return null;

        var book = new Book
        {
            Title = request.Title,
            PublicationYear = request.PublicationYear,
            Isbn = request.Isbn,
            AuthorId = authorId
        };

        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        return new BookDto(book.Id, book.Title, book.PublicationYear, book.Isbn, book.AuthorId,
            author.FirstName + " " + author.LastName);
    }
}
