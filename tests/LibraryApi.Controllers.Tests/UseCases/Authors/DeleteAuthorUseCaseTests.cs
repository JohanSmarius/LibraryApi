using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Authors;
using Moq;
using Xunit;

namespace LibraryApi.Controllers.Tests.UseCases.Authors;

public class DeleteAuthorUseCaseTests
{
    private readonly Mock<IAuthorRepository> _authorRepository;
    private readonly DeleteAuthorUseCase _useCase;

    public DeleteAuthorUseCaseTests()
    {
        _authorRepository = new Mock<IAuthorRepository>();
        _useCase = new DeleteAuthorUseCase(_authorRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorExists_ReturnsTrue()
    {
        // Arrange
        var author = new Author { Id = 1, FirstName = "John", LastName = "Doe", Country = "USA" };
        _authorRepository.Setup(r => r.GetByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(author);

        // Act
        var result = await _useCase.ExecuteAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result);
        _authorRepository.Verify(r => r.RemoveAsync(1, CancellationToken.None), Times.Once);
        _authorRepository.Verify(r => r.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorNotFound_ReturnsFalse()
    {
        // Arrange
        _authorRepository.Setup(r => r.GetByIdAsync(999, CancellationToken.None))
            .ReturnsAsync((Author?)null);

        // Act
        var result = await _useCase.ExecuteAsync(999, CancellationToken.None);

        // Assert
        Assert.False(result);
        _authorRepository.Verify(r => r.RemoveAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _authorRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        var author = new Author { Id = 2, FirstName = "Jane", LastName = "Smith", Country = "UK" };
        _authorRepository.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        var cancellationToken = new CancellationToken(false);

        // Act
        await _useCase.ExecuteAsync(2, cancellationToken);

        // Assert
        _authorRepository.Verify(r => r.GetByIdAsync(2, cancellationToken), Times.Once);
        _authorRepository.Verify(r => r.RemoveAsync(2, cancellationToken), Times.Once);
        _authorRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
