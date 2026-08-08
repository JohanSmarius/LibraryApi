using LibraryApi.Controllers.Dtos;

namespace LibraryApi.Controllers.Services;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAuthorsAsync(CancellationToken cancellationToken);

    Task<AuthorDto?> GetAuthorAsync(int id, CancellationToken cancellationToken);

    Task<AuthorDto> CreateAuthorAsync(
        CreateAuthorDto request,
        CancellationToken cancellationToken);

    Task<AuthorDto?> UpdateAuthorAsync(
        int id,
        UpdateAuthorDto request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAuthorAsync(int id, CancellationToken cancellationToken);
}
