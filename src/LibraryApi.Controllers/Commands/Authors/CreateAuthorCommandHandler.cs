using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Commands.Authors;

public record CreateAuthorCommand(CreateAuthorDto Request);

public class CreateAuthorCommandHandler(IAuthorRepository authors)
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

        authors.Add(author);
        await authors.SaveChangesAsync(cancellationToken);

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
