using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.UseCases.Authors;

public class GetAuthorUseCase(IAuthorRepository authorRepository)
{
    public async Task<AuthorDto?> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        var author = await authorRepository.GetByIdWithBooksAsync(id, cancellationToken);
        if (author is null)
        {
            return null;
        }

        return ToDto(author);
    }

    private static AuthorDto ToDto(LibraryApi.Controllers.Entities.Author author)
    {
        return new AuthorDto(
            author.Id,
            author.FirstName,
            author.LastName,
            author.Country,
            author.Books.Count);
    }
}
