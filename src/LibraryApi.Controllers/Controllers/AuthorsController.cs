using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly LibraryDbContext _context;

    // DbContext injected straight into the controller - no repository or
    // service layer in between. This is the deliberately "realistic but
    // unlayered" starting point for the talk.
    public AuthorsController(LibraryDbContext context)
    {
        _context = context;
    }

    /// <summary>Gets all authors.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuthorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
    {
        var authors = await _context.Authors
            .Include(a => a.Books)
            .Select(a => new AuthorDto(a.Id, a.FirstName, a.LastName, a.Country, a.Books.Count))
            .ToListAsync();

        return Ok(authors);
    }

    /// <summary>Gets a single author by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author is null)
        {
            return NotFound();
        }

        return Ok(new AuthorDto(author.Id, author.FirstName, author.LastName, author.Country, author.Books.Count));
    }

    /// <summary>Creates a new author.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(CreateAuthorDto request)
    {
        // Model validation runs via [ApiController] + data annotations on
        // CreateAuthorDto - this is the validation story that later gets
        // reworked as an endpoint filter on the minimal API side.
        var author = new Author
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Country = request.Country
        };

        _context.Authors.Add(author);
        await _context.SaveChangesAsync();

        var dto = new AuthorDto(author.Id, author.FirstName, author.LastName, author.Country, 0);
        return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, dto);
    }

    /// <summary>Updates an author.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthorDto>> UpdateAuthor(int id, UpdateAuthorDto request)
    {
        var author = await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (author is null)
        {
            return NotFound();
        }

        author.FirstName = request.FirstName;
        author.LastName = request.LastName;
        author.Country = request.Country;

        await _context.SaveChangesAsync();

        return Ok(new AuthorDto(author.Id, author.FirstName, author.LastName, author.Country, author.Books.Count));
    }

    /// <summary>Deletes an author and all of their books.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        var author = await _context.Authors.FindAsync(id);
        if (author is null)
        {
            return NotFound();
        }

        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
