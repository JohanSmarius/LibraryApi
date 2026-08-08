using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Services;

public class BookService(LibraryDbContext db) : IBookService
{
    public Task<List<BookDto>> GetBooksAsync(CancellationToken cancellationToken)
    {
        return db.Books
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
        return db.Books
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
        var author = await db.Authors.FindAsync([request.AuthorId], cancellationToken);
        if (author is null)
        {
            return null;
        }

        var book = CreateBook(request, request.AuthorId, author);

        db.Books.Add(book);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(book);
    }

    public async Task<BookDto?> UpdateBookAsync(
        int id,
        SaveBookDto request,
        CancellationToken cancellationToken)
    {
        var book = await db.Books.FindAsync([id], cancellationToken);
        if (book is null)
        {
            return null;
        }

        var author = await db.Authors.FindAsync([request.AuthorId], cancellationToken);
        if (author is null)
        {
            return null;
        }

        book.Title = request.Title;
        book.PublicationYear = request.PublicationYear;
        book.Isbn = request.Isbn;
        book.AuthorId = request.AuthorId;
        book.Author = author;

        await db.SaveChangesAsync(cancellationToken);

        return ToDto(book);
    }

    public async Task<bool> DeleteBookAsync(int id, CancellationToken cancellationToken)
    {
        var book = await db.Books.FindAsync([id], cancellationToken);
        if (book is null)
        {
            return false;
        }

        db.Books.Remove(book);
        await db.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<List<BookDto>?> GetBooksForAuthorAsync(
        int authorId,
        CancellationToken cancellationToken)
    {
        var authorExists = await db.Authors
            .AsNoTracking()
            .AnyAsync(author => author.Id == authorId, cancellationToken);

        if (!authorExists)
        {
            return null;
        }

        return await db.Books
            .AsNoTracking()
            .Where(book => book.AuthorId == authorId)
            .Select(book => new BookDto(
                book.Id,
                book.Title,
                book.PublicationYear,
                book.Isbn,
                book.AuthorId,
                $"{book.Author!.FirstName} {book.Author.LastName}"))
            .ToListAsync(cancellationToken);
    }

    public Task<BookDto?> GetBookForAuthorAsync(
        int authorId,
        int bookId,
        CancellationToken cancellationToken)
    {
        return db.Books
            .AsNoTracking()
            .Where(book => book.Id == bookId && book.AuthorId == authorId)
            .Select(book => new BookDto(
                book.Id,
                book.Title,
                book.PublicationYear,
                book.Isbn,
                book.AuthorId,
                $"{book.Author!.FirstName} {book.Author.LastName}"))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<BookDto?> CreateBookForAuthorAsync(
        int authorId,
        CreateBookDto request,
        CancellationToken cancellationToken)
    {
        var author = await db.Authors.FindAsync([authorId], cancellationToken);
        if (author is null)
        {
            return null;
        }

        var book = CreateBook(request, authorId, author);

        db.Books.Add(book);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(book);
    }

    private static Book CreateBook(CreateBookDto request, int authorId, Author author)
    {
        return new Book
        {
            Title = request.Title,
            PublicationYear = request.PublicationYear,
            Isbn = request.Isbn,
            AuthorId = authorId,
            Author = author
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
}
