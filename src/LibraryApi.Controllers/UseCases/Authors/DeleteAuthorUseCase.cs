using LibraryApi.Controllers.Data;

namespace LibraryApi.Controllers.UseCases.Authors;

public class DeleteAuthorUseCase(LibraryDbContext db)
{
    public async Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
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
