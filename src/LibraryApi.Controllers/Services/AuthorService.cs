using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Services;

public class AuthorService(LibraryDbContext db) : IAuthorService
{
    public Task<List<AuthorDto>> GetAuthorsAsync(CancellationToken cancellationToken)
    {
        return db.Authors
            .AsNoTracking()
            .Select(author => new AuthorDto(
                author.Id,
                author.FirstName,
                author.LastName,
                author.Country,
                author.Books.Count))
            .ToListAsync(cancellationToken);
    }

    public Task<AuthorDto?> GetAuthorAsync(int id, CancellationToken cancellationToken)
    {
        return db.Authors
            .AsNoTracking()
            .Where(author => author.Id == id)
            .Select(author => new AuthorDto(
                author.Id,
                author.FirstName,
                author.LastName,
                author.Country,
                author.Books.Count))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AuthorDto> CreateAuthorAsync(
        CreateAuthorDto request,
        CancellationToken cancellationToken)
    {
        var author = new Author
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Country = request.Country
        };

        db.Authors.Add(author);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(author);
    }

    public async Task<AuthorDto?> UpdateAuthorAsync(
        int id,
        UpdateAuthorDto request,
        CancellationToken cancellationToken)
    {
        var author = await db.Authors
            .Include(author => author.Books)
            .FirstOrDefaultAsync(author => author.Id == id, cancellationToken);

        if (author is null)
        {
            return null;
        }

        author.FirstName = request.FirstName;
        author.LastName = request.LastName;
        author.Country = request.Country;

        await db.SaveChangesAsync(cancellationToken);

        return ToDto(author);
    }

    public async Task<bool> DeleteAuthorAsync(int id, CancellationToken cancellationToken)
    {
        var author = await db.Authors.FindAsync([id], cancellationToken);
        if (author is null)
        {
            return false;
        }

        db.Authors.Remove(author);
        await db.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static AuthorDto ToDto(Author author)
    {
        return new AuthorDto(
            author.Id,
            author.FirstName,
            author.LastName,
            author.Country,
            author.Books.Count);
    }
}
