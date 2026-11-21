using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Commands.Authors;

public record UpdateAuthorCommand(int Id, UpdateAuthorDto Request);

public class UpdateAuthorCommandHandler(IAuthorRepository authors)
{
    public async Task<AuthorDto?> HandleAsync(UpdateAuthorCommand command, CancellationToken cancellationToken)
    {
        var id = command.Id;
        var request = command.Request;

        var author = await authors.FindWithBooksAsync(id, cancellationToken);

        if (author is null)
        {
            return null;
        }

        author.FirstName = request.FirstName;
        author.LastName = request.LastName;
        author.Country = request.Country;

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
