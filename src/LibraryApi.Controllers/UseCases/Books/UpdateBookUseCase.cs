using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.UseCases.Books;

public class UpdateBookUseCase(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository)
{
    public async Task<BookDto?> ExecuteAsync(
        int id,
        SaveBookDto request,
        CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
        {
            return null;
        }

        var author = await authorRepository.GetByIdAsync(request.AuthorId, cancellationToken);
        if (author is null)
        {
            return null;
        }

        book.Title = request.Title;
        book.PublicationYear = request.PublicationYear;
        book.Isbn = request.Isbn;
        book.AuthorId = request.AuthorId;
        book.Author = author;

        bookRepository.Update(book);
        await bookRepository.SaveChangesAsync(cancellationToken);

        return ToDto(book);
    }

    private static BookDto ToDto(LibraryApi.Controllers.Entities.Book book)
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
