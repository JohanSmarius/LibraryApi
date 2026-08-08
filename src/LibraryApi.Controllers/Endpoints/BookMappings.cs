using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Endpoints;

internal static class BookMappings
{
    extension (Book book)
    {
        public BookDto ToDto()
        {
            return new BookDto(
                book.Id,
                book.Title,
                book.PublicationYear,
                book.Isbn,
                book.AuthorId,
                $"{book.Author!.FirstName} {book.Author.LastName}");
        }
    }
}
