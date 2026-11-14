using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers.UseCases.Authors;

public class UpdateAuthorUseCase(LibraryDbContext db)
{
    public async Task<AuthorDto?> ExecuteAsync(
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
