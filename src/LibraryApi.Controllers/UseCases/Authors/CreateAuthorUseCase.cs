using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.UseCases.Authors;

public class CreateAuthorUseCase(LibraryDbContext db)
{
    public async Task<AuthorDto> ExecuteAsync(
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
