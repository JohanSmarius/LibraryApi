using LibraryApi.Controllers.Data;

namespace LibraryApi.Controllers.UseCases.Books;

public class DeleteBookUseCase(LibraryDbContext db)
{
    public async Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
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
