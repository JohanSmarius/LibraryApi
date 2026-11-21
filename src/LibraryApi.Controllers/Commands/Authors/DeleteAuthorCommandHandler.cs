using LibraryApi.Controllers.Data;

namespace LibraryApi.Controllers.Commands.Authors;

public record DeleteAuthorCommand(int Id);

public class DeleteAuthorCommandHandler(LibraryDbContext db)
{
    public async Task<bool> HandleAsync(DeleteAuthorCommand command, CancellationToken cancellationToken)
    {
        var id = command.Id;

        var author = await db.Authors.FindAsync([id], cancellationToken);
        if (author is null)
        {
            return false;
        }

        db.Authors.Remove(author);
        await db.SaveChangesAsync(cancellationToken);

        return true;
    }
}
