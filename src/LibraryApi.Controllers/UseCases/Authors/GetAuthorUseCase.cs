using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.UseCases.Authors;

public class GetAuthorUseCase(LibraryDbContext db)
{
    public Task<AuthorDto?> ExecuteAsync(int id, CancellationToken cancellationToken)
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
}
