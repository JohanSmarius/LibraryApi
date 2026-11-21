using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Queries.Authors;

public record GetAuthorsQuery();

public class GetAuthorsQueryHandler(LibraryDbContext db)
{
    public Task<List<AuthorDto>> HandleAsync(GetAuthorsQuery query, CancellationToken cancellationToken)
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
}
