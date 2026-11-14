using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Authors;
using Moq;
using Xunit;

namespace LibraryApi.Controllers.Tests.UseCases.Authors;

public class GetAuthorUseCaseTests
{
    private readonly Mock<IAuthorRepository> _authorRepository;
    private readonly GetAuthorUseCase _useCase;

    public GetAuthorUseCaseTests()
    {
        _authorRepository = new Mock<IAuthorRepository>();
        _useCase = new GetAuthorUseCase(_authorRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorExists_ReturnsDto()
    {
        // Arrange
        var author = new Author
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Country = "USA"
        };
        author.Books = new List<Book>
        {
            new Book { Id = 1, Title = "Book 1", Author = author },
            new Book { Id = 2, Title = "Book 2", Author = author }
        };
        _authorRepository.Setup(r => r.GetByIdWithBooksAsync(1, CancellationToken.None))
            .ReturnsAsync(author);

        // Act
        var result = await _useCase.ExecuteAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("USA", result.Country);
        Assert.Equal(2, result.BookCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorNotFound_ReturnsNull()
    {
        // Arrange
        _authorRepository.Setup(r => r.GetByIdWithBooksAsync(999, CancellationToken.None))
            .ReturnsAsync((Author?)null);

        // Act
        var result = await _useCase.ExecuteAsync(999, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_AuthorWithNoBooks_ReturnsZeroBookCount()
    {
        // Arrange
        var author = new Author
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Country = "UK",
            Books = new List<Book>()
        };
        _authorRepository.Setup(r => r.GetByIdWithBooksAsync(1, CancellationToken.None))
            .ReturnsAsync(author);

        // Act
        var result = await _useCase.ExecuteAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.BookCount);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        var author = new Author { Id = 1, FirstName = "Test", LastName = "Author", Country = "DE" };
        _authorRepository.Setup(r => r.GetByIdWithBooksAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        var cancellationToken = new CancellationToken(false);

        // Act
        await _useCase.ExecuteAsync(1, cancellationToken);

        // Assert
        _authorRepository.Verify(r => r.GetByIdWithBooksAsync(1, cancellationToken), Times.Once);
    }
}
