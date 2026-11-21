using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.Commands.Books;

public record DeleteBookCommand(int Id);

public class DeleteBookCommandHandler(IBookRepository books)
{
    public async Task<bool> HandleAsync(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        var id = command.Id;

        var book = await books.FindAsync(id, cancellationToken);
        if (book is null)
        {
            return false;
        }

        books.Remove(book);
        await books.SaveChangesAsync(cancellationToken);

        return true;
    }
}
