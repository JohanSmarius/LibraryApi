using LibraryApi.MinimalApi.Data;
using LibraryApi.MinimalApi.Dtos;
using LibraryApi.MinimalApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.MinimalApi.Services;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAuthorsAsync();
    Task<AuthorDto?> GetAuthorAsync(int id);
    Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto request);
}
 
public class AuthorService : IAuthorService
{
    private readonly LibraryDbContext _db;
 
    public AuthorService(LibraryDbContext db) => _db = db;
 
    public async Task<List<AuthorDto>> GetAuthorsAsync() =>
        await _db.Authors.Include(a => a.Books)
            .Select(a => new AuthorDto(a.Id, a.FirstName, a.LastName, a.Country, a.Books.Count))
            .ToListAsync();
 
    public async Task<AuthorDto?> GetAuthorAsync(int id)
    {
        var author = await _db.Authors.Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);
 
        return author is null
            ? null
            : new AuthorDto(author.Id, author.FirstName, author.LastName, author.Country, author.Books.Count);
    }
 
    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto request)
    {
        var author = new Author
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Country = request.Country
        };
 
        _db.Authors.Add(author);
        await _db.SaveChangesAsync();
 
        return new AuthorDto(author.Id, author.FirstName, author.LastName, author.Country, 0);
    }
}
