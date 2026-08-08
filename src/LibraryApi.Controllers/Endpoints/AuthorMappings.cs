using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Endpoints;

internal static class AuthorMappings
{
    extension (Author author)
    {
        public AuthorDto ToDto()
        {
            return new AuthorDto(
                author.Id,
                author.FirstName,
                author.LastName,
                author.Country,
                author.Books.Count);
        }
    }
}
