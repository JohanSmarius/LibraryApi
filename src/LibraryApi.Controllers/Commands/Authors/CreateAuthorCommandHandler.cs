using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Commands.Authors;

public record CreateAuthorCommand(CreateAuthorDto Request);

public class CreateAuthorCommandHandler(LibraryDbContext db)
{
    public async Task<AuthorDto> HandleAsync(CreateAuthorCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

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
