using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.Queries.Authors;

public record GetAuthorQuery(int Id);

public class GetAuthorQueryHandler(LibraryDbContext db)
{
    public Task<AuthorDto?> HandleAsync(GetAuthorQuery query, CancellationToken cancellationToken)
    {
        var id = query.Id;

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
}
