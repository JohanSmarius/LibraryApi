using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.Dtos;

namespace LibraryApi.Controllers.Queries.Authors;

public record GetAuthorQuery(int Id);

public class GetAuthorQueryHandler(IAuthorRepository authors)
{
    public Task<AuthorDto?> HandleAsync(GetAuthorQuery query, CancellationToken cancellationToken)
    {
        return authors.GetAsync(query.Id, cancellationToken);
    }
}
