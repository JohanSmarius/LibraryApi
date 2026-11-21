using LibraryApi.Controllers.Data;

namespace LibraryApi.Controllers.Commands.Books;

public record DeleteBookCommand(int Id);

public class DeleteBookCommandHandler(LibraryDbContext db)
{
    public async Task<bool> HandleAsync(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        var id = command.Id;

        var book = await db.Books.FindAsync([id], cancellationToken);
        if (book is null)
        {
            return false;
        }

        db.Books.Remove(book);
        await db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
