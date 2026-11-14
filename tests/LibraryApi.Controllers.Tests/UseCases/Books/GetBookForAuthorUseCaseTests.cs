using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Books;
using Moq;
using Xunit;

namespace LibraryApi.Controllers.Tests.UseCases.Books;

public class GetBookForAuthorUseCaseTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly GetBookForAuthorUseCase _useCase;

    public GetBookForAuthorUseCaseTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _useCase = new GetBookForAuthorUseCase(_bookRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBookExists_ReturnsDto()
    {
        // Arrange
        var author = new Author { Id = 1, FirstName = "John", LastName = "Doe", Country = "USA" };
        var book = new Book
        {
            Id = 1,
            Title = "The Great Book",
            PublicationYear = 2023,
            Isbn = "123-456",
            AuthorId = 1,
            Author = author
        };
        _bookRepository.Setup(r => r.GetByAuthorIdAndBookIdAsync(1, 1, CancellationToken.None))
            .ReturnsAsync(book);

        // Act
        var result = await _useCase.ExecuteAsync(1, 1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("The Great Book", result.Title);
        Assert.Equal(2023, result.PublicationYear);
        Assert.Equal("123-456", result.Isbn);
        Assert.Equal(1, result.AuthorId);
        Assert.Equal("John Doe", result.AuthorFullName);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBookNotFound_ReturnsNull()
    {
        // Arrange
        _bookRepository.Setup(r => r.GetByAuthorIdAndBookIdAsync(1, 999, CancellationToken.None))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _useCase.ExecuteAsync(1, 999, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        var author = new Author { Id = 1, FirstName = "Jane", LastName = "Smith", Country = "UK" };
        var book = new Book { Id = 1, Title = "Book", AuthorId = 1, Author = author };
        _bookRepository.Setup(r => r.GetByAuthorIdAndBookIdAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        var cancellationToken = new CancellationToken(false);

        // Act
        await _useCase.ExecuteAsync(1, 1, cancellationToken);

        // Assert
        _bookRepository.Verify(r => r.GetByAuthorIdAndBookIdAsync(1, 1, cancellationToken), Times.Once);
    }
}
