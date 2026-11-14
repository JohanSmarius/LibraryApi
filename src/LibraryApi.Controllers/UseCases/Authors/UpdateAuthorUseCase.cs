using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.UseCases.Authors;

public class UpdateAuthorUseCase(IAuthorRepository authorRepository)
{
    public async Task<AuthorDto?> ExecuteAsync(
        int id,
        UpdateAuthorDto request,
        CancellationToken cancellationToken)
    {
        var author = await authorRepository.GetByIdWithBooksAsync(id, cancellationToken);

        if (author is null)
        {
            return null;
        }

        author.FirstName = request.FirstName;
        author.LastName = request.LastName;
        author.Country = request.Country;

        authorRepository.Update(author);
        await authorRepository.SaveChangesAsync(cancellationToken);

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
