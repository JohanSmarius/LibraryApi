using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.UseCases.Authors;

public class GetAuthorsUseCase(LibraryDbContext db)
{
    public Task<List<AuthorDto>> ExecuteAsync(CancellationToken cancellationToken)
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
