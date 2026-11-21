using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.Commands.Authors;

public record DeleteAuthorCommand(int Id);

public class DeleteAuthorCommandHandler(IAuthorRepository authors)
{
    public async Task<bool> HandleAsync(DeleteAuthorCommand command, CancellationToken cancellationToken)
    {
        var id = command.Id;

        var author = await authors.FindAsync(id, cancellationToken);
        if (author is null)
        {
            return false;
        }

        authors.Remove(author);
        await authors.SaveChangesAsync(cancellationToken);

        return true;
    }
}
