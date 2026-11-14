using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.UseCases.Books;

public class DeleteBookUseCase(IBookRepository bookRepository)
{
    public async Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(id, cancellationToken);
        if (book is null)
        {
            return false;
        }

        await bookRepository.RemoveAsync(id, cancellationToken);
        await bookRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
