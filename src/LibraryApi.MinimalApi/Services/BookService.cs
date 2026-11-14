using LibraryApi.MinimalApi.Data;
using LibraryApi.MinimalApi.Dtos;
using LibraryApi.MinimalApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.MinimalApi.Services;


public interface IBookService
{

    Task<List<BookDto>> GetBooksAsync(CancellationToken cancellationToken);
    Task<BookDto?> GetBookAsync(int id, CancellationToken cancellationToken);

    Task<BookDto?> CreateBookAsync(
        SaveBookDto request,
        CancellationToken cancellationToken);

    Task<BookDto?> UpdateBookAsync(
        int id,
        SaveBookDto request,
        CancellationToken cancellationToken);

    Task<bool> DeleteBookAsync(int id, CancellationToken cancellationToken);

    Task<List<BookDto>?> GetBooksForAuthorAsync(int authorId);
    Task<BookDto?> GetBookForAuthorAsync(int authorId, int bookId);
    Task<BookDto?> CreateBookForAuthorAsync(int authorId, CreateBookDto request);
}

public class BookService : IBookService
{
    private readonly LibraryDbContext _db;
    public BookService(LibraryDbContext db) => _db = db;

    public Task<List<BookDto>> GetBooksAsync(CancellationToken cancellationToken)
    {
        return _db.Books
            .AsNoTracking()
            .Select(book => new BookDto(
                book.Id,
                book.Title,
                book.PublicationYear,
                book.Isbn,
                book.AuthorId,
                $"{book.Author!.FirstName} {book.Author.LastName}"))
            .ToListAsync(cancellationToken);
    }

    public Task<BookDto?> GetBookAsync(int id, CancellationToken cancellationToken)
    {
        return _db.Books
            .AsNoTracking()
            .Where(book => book.Id == id)
            .Select(book => new BookDto(
                book.Id,
                book.Title,
                book.PublicationYear,
                book.Isbn,
                book.AuthorId,
                $"{book.Author!.FirstName} {book.Author.LastName}"))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<BookDto?> CreateBookAsync(
        SaveBookDto request,
        CancellationToken cancellationToken)
    {
        var author = await _db.Authors.FindAsync([request.AuthorId], cancellationToken);
        if (author is null)
        {
            return null;
        }

        var book = CreateBook(request, request.AuthorId);

        _db.Books.Add(book);
        await _db.SaveChangesAsync(cancellationToken);

        return ToDto(book);
    }



    public async Task<BookDto?> UpdateBookAsync(
        int id,
        SaveBookDto request,
        CancellationToken cancellationToken)
    {
        var book = await _db.Books.FindAsync([id], cancellationToken);
        if (book is null)
        {
            return null;
        }

        var author = await _db.Authors.FindAsync([request.AuthorId], cancellationToken);
        if (author is null)
        {
            return null;
        }

        book.Title = request.Title;
        book.PublicationYear = request.PublicationYear;
        book.Isbn = request.Isbn;
        book.AuthorId = request.AuthorId;

        await _db.SaveChangesAsync(cancellationToken);

        return ToDto(book);
    }


    public async Task<bool> DeleteBookAsync(int id, CancellationToken cancellationToken)
    {
        var book = await _db.Books.FindAsync([id], cancellationToken);
        if (book is null)
        {
            return false;
        }

        _db.Books.Remove(book);
        await _db.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static Book CreateBook(CreateBookDto request, int authorId)
    {
        return new Book
        {
            Title = request.Title,
            PublicationYear = request.PublicationYear,
            Isbn = request.Isbn,
            AuthorId = authorId
        };
    }

    private static BookDto ToDto(Book book)
    {
        return new BookDto(
            book.Id,
            book.Title,
            book.PublicationYear,
            book.Isbn,
            book.AuthorId,
            $"{book.Author!.FirstName} {book.Author.LastName}");
    }

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