using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Authors;
using Moq;
using Xunit;

namespace LibraryApi.Controllers.Tests.UseCases.Authors;

public class GetAuthorsUseCaseTests
{
    private readonly Mock<IAuthorRepository> _authorRepository;
    private readonly GetAuthorsUseCase _useCase;

    public GetAuthorsUseCaseTests()
    {
        _authorRepository = new Mock<IAuthorRepository>();
        _useCase = new GetAuthorsUseCase(_authorRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsListOfAuthorDtos()
    {
        // Arrange
        var authors = new List<Author>
        {
            new Author { Id = 1, FirstName = "John", LastName = "Doe", Country = "USA", Books = new List<Book>() },
            new Author { Id = 2, FirstName = "Jane", LastName = "Smith", Country = "UK", Books = new List<Book>() }
        };
        _authorRepository.Setup(r => r.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(authors);

        // Act
        var result = await _useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("John", result[0].FirstName);
        Assert.Equal("Doe", result[0].LastName);
        Assert.Equal("USA", result[0].Country);
        Assert.Equal(0, result[0].BookCount);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("Jane", result[1].FirstName);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoAuthors_ReturnsEmptyList()
    {
        // Arrange
        _authorRepository.Setup(r => r.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(new List<Author>());

        // Act
        var result = await _useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_AuthorsWithBooks_ReturnsCorrectBookCount()
    {
        // Arrange
        var author = new Author
        {
            Id = 1,
            FirstName = "Test",
            LastName = "Author",
            Country = "DE",
            Books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1" },
                new Book { Id = 2, Title = "Book 2" },
                new Book { Id = 3, Title = "Book 3" }
            }
        };
        _authorRepository.Setup(r => r.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(new List<Author> { author });

        // Act
        var result = await _useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(3, result[0].BookCount);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        _authorRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Author>());
        var cancellationToken = new CancellationToken(false);

        // Act
        await _useCase.ExecuteAsync(cancellationToken);

        // Assert
        _authorRepository.Verify(r => r.GetAllAsync(cancellationToken), Times.Once);
    }
}
