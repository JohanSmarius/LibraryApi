using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Controllers;

// Nested under /api/authors/{authorId}/books - this is the controller-side
// version of the nested-routing story the talk demonstrates migrating to
// MapGroup in the minimal API version.
[ApiController]
[Route("api/authors/{authorId:int}/books")]
public class AuthorBooksController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public AuthorBooksController(LibraryDbContext context)
    {
        _context = context;
    }

    /// <summary>Gets all books for a given author.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksForAuthor(int authorId)
    {
        var author = await _context.Authors.FindAsync(authorId);
        if (author is null)
        {
            return NotFound();
        }

        var books = await _context.Books
            .Include(b => b.Author)
            .Where(b => b.AuthorId == authorId)
            .Select(b => new BookDto(b.Id, b.Title, b.PublicationYear, b.Isbn, b.AuthorId, b.Author!.FirstName + " " + b.Author.LastName))
            .ToListAsync();

        return Ok(books);
    }

    /// <summary>Gets a single book that belongs to a given author.</summary>
    [HttpGet("{bookId:int}")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetBookForAuthor(int authorId, int bookId)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == bookId && b.AuthorId == authorId);

        if (book is null)
        {
            return NotFound();
        }

        return Ok(new BookDto(book.Id, book.Title, book.PublicationYear, book.Isbn, book.AuthorId, book.Author!.FirstName + " " + book.Author.LastName));
    }

    /// <summary>Creates a new book under the given author.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> CreateBookForAuthor(int authorId, CreateBookDto request)
    {
        var author = await _context.Authors.FindAsync(authorId);
        if (author is null)
        {
            return NotFound();
        }

        var book = new Book
        {
            Title = request.Title,
            PublicationYear = request.PublicationYear,
            Isbn = request.Isbn,
            AuthorId = authorId
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var dto = new BookDto(book.Id, book.Title, book.PublicationYear, book.Isbn, book.AuthorId, $"{author.FirstName} {author.LastName}");
        return CreatedAtAction(nameof(GetBookForAuthor), new { authorId, bookId = book.Id }, dto);
    }
}
