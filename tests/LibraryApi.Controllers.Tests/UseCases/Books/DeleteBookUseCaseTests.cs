using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Books;
using Moq;
using Xunit;

namespace LibraryApi.Controllers.Tests.UseCases.Books;

public class DeleteBookUseCaseTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly DeleteBookUseCase _useCase;

    public DeleteBookUseCaseTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _useCase = new DeleteBookUseCase(_bookRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBookExists_ReturnsTrue()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Book 1", AuthorId = 1 };
        _bookRepository.Setup(r => r.GetByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(book);

        // Act
        var result = await _useCase.ExecuteAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result);
        _bookRepository.Verify(r => r.RemoveAsync(1, CancellationToken.None), Times.Once);
        _bookRepository.Verify(r => r.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBookNotFound_ReturnsFalse()
    {
        // Arrange
        _bookRepository.Setup(r => r.GetByIdAsync(999, CancellationToken.None))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _useCase.ExecuteAsync(999, CancellationToken.None);

        // Assert
        Assert.False(result);
        _bookRepository.Verify(r => r.RemoveAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _bookRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        var book = new Book { Id = 2, Title = "Book 2", AuthorId = 1 };
        _bookRepository.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        var cancellationToken = new CancellationToken(false);

        // Act
        await _useCase.ExecuteAsync(2, cancellationToken);

        // Assert
        _bookRepository.Verify(r => r.GetByIdAsync(2, cancellationToken), Times.Once);
        _bookRepository.Verify(r => r.RemoveAsync(2, cancellationToken), Times.Once);
        _bookRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
