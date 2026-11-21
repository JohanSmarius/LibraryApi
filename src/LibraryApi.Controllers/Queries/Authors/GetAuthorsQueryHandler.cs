using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.Dtos;

namespace LibraryApi.Controllers.Queries.Authors;

public record GetAuthorsQuery();

public class GetAuthorsQueryHandler(IAuthorRepository authors)
{
    public Task<List<AuthorDto>> HandleAsync(GetAuthorsQuery query, CancellationToken cancellationToken)
    {
        return authors.GetAllAsync(cancellationToken);
    }
}
