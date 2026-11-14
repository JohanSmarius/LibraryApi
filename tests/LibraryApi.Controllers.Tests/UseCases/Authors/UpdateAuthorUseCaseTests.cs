using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Authors;
using Moq;
using Xunit;

namespace LibraryApi.Controllers.Tests.UseCases.Authors;

public class UpdateAuthorUseCaseTests
{
    private readonly Mock<IAuthorRepository> _authorRepository;
    private readonly UpdateAuthorUseCase _useCase;

    public UpdateAuthorUseCaseTests()
    {
        _authorRepository = new Mock<IAuthorRepository>();
        _useCase = new UpdateAuthorUseCase(_authorRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorExists_ReturnsUpdatedDto()
    {
        // Arrange
        var author = new Author
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Country = "USA",
            Books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1" }
            }
        };
        _authorRepository.Setup(r => r.GetByIdWithBooksAsync(1, CancellationToken.None))
            .ReturnsAsync(author);

        var request = new UpdateAuthorDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Country = "UK"
        };

        // Act
        var result = await _useCase.ExecuteAsync(1, request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.Equal("UK", result.Country);
        Assert.Equal(1, result.BookCount);

        _authorRepository.Verify(r => r.Update(It.Is<Author>(a =>
            a.FirstName == "Jane" && a.LastName == "Smith" && a.Country == "UK")), Times.Once);
        _authorRepository.Verify(r => r.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorNotFound_ReturnsNull()
    {
        // Arrange
        _authorRepository.Setup(r => r.GetByIdWithBooksAsync(999, CancellationToken.None))
            .ReturnsAsync((Author?)null);

        var request = new UpdateAuthorDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Country = "UK"
        };

        // Act
        var result = await _useCase.ExecuteAsync(999, request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _authorRepository.Verify(r => r.Update(It.IsAny<Author>()), Times.Never);
        _authorRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        var author = new Author { Id = 1, FirstName = "Old", LastName = "Name", Country = "DE" };
        _authorRepository.Setup(r => r.GetByIdWithBooksAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        var cancellationToken = new CancellationToken(false);
        var request = new UpdateAuthorDto
        {
            FirstName = "New",
            LastName = "Name",
            Country = "FR"
        };

        // Act
        await _useCase.ExecuteAsync(1, request, cancellationToken);

        // Assert
        _authorRepository.Verify(r => r.GetByIdWithBooksAsync(1, cancellationToken), Times.Once);
        _authorRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
