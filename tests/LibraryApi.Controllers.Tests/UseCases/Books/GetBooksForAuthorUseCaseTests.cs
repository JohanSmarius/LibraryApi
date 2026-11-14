using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Books;
using Moq;
using Xunit;

namespace LibraryApi.Controllers.Tests.UseCases.Books;

public class GetBooksForAuthorUseCaseTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly Mock<IAuthorRepository> _authorRepository;
    private readonly GetBooksForAuthorUseCase _useCase;

    public GetBooksForAuthorUseCaseTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _authorRepository = new Mock<IAuthorRepository>();
        _useCase = new GetBooksForAuthorUseCase(_bookRepository.Object, _authorRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorExists_ReturnsListOfBooks()
    {
        // Arrange
        var author = new Author { Id = 1, FirstName = "John", LastName = "Doe", Country = "USA" };
        _authorRepository.Setup(r => r.ExistsAsync(1, CancellationToken.None))
            .ReturnsAsync(true);

        var books = new List<Book>
        {
            new Book { Id = 1, Title = "Book 1", PublicationYear = 2020, Isbn = "111", AuthorId = 1, Author = author },
            new Book { Id = 2, Title = "Book 2", PublicationYear = 2021, Isbn = "222", AuthorId = 1, Author = author }
        };
        _bookRepository.Setup(r => r.GetByAuthorIdAsync(1, CancellationToken.None))
            .ReturnsAsync(books);

        // Act
        var result = await _useCase.ExecuteAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Book 1", result[0].Title);
        Assert.Equal("Book 2", result[1].Title);
        Assert.Equal("John Doe", result[0].AuthorFullName);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorNotFound_ReturnsNull()
    {
        // Arrange
        _authorRepository.Setup(r => r.ExistsAsync(999, CancellationToken.None))
            .ReturnsAsync(false);

        // Act
        var result = await _useCase.ExecuteAsync(999, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _bookRepository.Verify(r => r.GetByAuthorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorHasNoBooks_ReturnsEmptyList()
    {
        // Arrange
        _authorRepository.Setup(r => r.ExistsAsync(1, CancellationToken.None))
            .ReturnsAsync(true);
        _bookRepository.Setup(r => r.GetByAuthorIdAsync(1, CancellationToken.None))
            .ReturnsAsync(new List<Book>());

        // Act
        var result = await _useCase.ExecuteAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        _authorRepository.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _bookRepository.Setup(r => r.GetByAuthorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book>());
        var cancellationToken = new CancellationToken(false);

        // Act
        await _useCase.ExecuteAsync(1, cancellationToken);

        // Assert
        _authorRepository.Verify(r => r.ExistsAsync(1, cancellationToken), Times.Once);
        _bookRepository.Verify(r => r.GetByAuthorIdAsync(1, cancellationToken), Times.Once);
    }
}
