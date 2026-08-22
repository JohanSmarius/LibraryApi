using LibraryApi.MinimalApi.Data;
using LibraryApi.MinimalApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.MinimalApi.Handlers;

public class AuthorHandler(LibraryDbContext Db) : IAuthorHandler
{
    public async Task<IResult> GetAuthorsAsync()
    {
        var result = await Db.Authors
            .Include(a => a.Books)
            .Select(a => new AuthorDto(a.Id, a.FirstName, a.LastName, a.Country, a.Books.Count))
            .ToListAsync();
        
        return  Results.Ok(result);
    }
}