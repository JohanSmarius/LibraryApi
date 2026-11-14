using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Controllers.UseCases.Authors;

public class DeleteAuthorUseCase(IAuthorRepository authorRepository)
{
    public async Task<bool> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        var author = await authorRepository.GetByIdAsync(id, cancellationToken);
        if (author is null)
        {
            return false;
        }

        await authorRepository.RemoveAsync(id, cancellationToken);
        await authorRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
