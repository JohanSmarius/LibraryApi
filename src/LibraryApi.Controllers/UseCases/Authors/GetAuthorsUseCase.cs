using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.UseCases.Authors;

public class GetAuthorsUseCase(IAuthorRepository authorRepository)
{
    public async Task<List<AuthorDto>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var authors = await authorRepository.GetAllAsync(cancellationToken);
        return authors.Select(ToDto).ToList();
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
