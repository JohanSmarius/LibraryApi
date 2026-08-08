using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Controllers;

// Flat book routes are kept separate from AuthorBooksController's nested
// routes so both routing styles remain available in the starting solution.
[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly LibraryDbContext _context;

    public BooksController(LibraryDbContext context)
    {
        _context = context;
    }

    /// <summary>Gets all books.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
    {
        var books = await _context.Books
            .Include(b => b.Author)
            .Select(b => new BookDto(
                b.Id,
                b.Title,
                b.PublicationYear,
                b.Isbn,
                b.AuthorId,
                b.Author!.FirstName + " " + b.Author.LastName))
            .ToListAsync();

        return Ok(books);
    }

    /// <summary>Gets a single book by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book is null)
        {
            return NotFound();
        }

        return Ok(ToDto(book));
    }

    /// <summary>Creates a book.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> CreateBook(SaveBookDto request)
    {
        var author = await _context.Authors.FindAsync(request.AuthorId);
        if (author is null)
        {
            return NotFound();
        }

        var book = new Book
        {
            Title = request.Title,
            PublicationYear = request.PublicationYear,
            Isbn = request.Isbn,
            AuthorId = request.AuthorId,
            Author = author
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var dto = ToDto(book);
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, dto);
    }

    /// <summary>Updates a book.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> UpdateBook(int id, SaveBookDto request)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        var author = await _context.Authors.FindAsync(request.AuthorId);
        if (author is null)
        {
            return NotFound();
        }

        book.Title = request.Title;
        book.PublicationYear = request.PublicationYear;
        book.Isbn = request.Isbn;
        book.AuthorId = request.AuthorId;
        book.Author = author;

        await _context.SaveChangesAsync();

        return Ok(ToDto(book));
    }

    /// <summary>Deletes a book by id, regardless of author.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
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
