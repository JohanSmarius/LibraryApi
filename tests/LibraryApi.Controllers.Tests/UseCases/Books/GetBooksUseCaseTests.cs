using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Books;
using Moq;
using Xunit;

namespace LibraryApi.Controllers.Tests.UseCases.Books;

public class GetBooksUseCaseTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly GetBooksUseCase _useCase;

    public GetBooksUseCaseTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _useCase = new GetBooksUseCase(_bookRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsListOfBookDtos()
    {
        // Arrange
        var author = new Author { Id = 1, FirstName = "John", LastName = "Doe", Country = "USA" };
        var books = new List<Book>
        {
            new Book { Id = 1, Title = "Book 1", PublicationYear = 2020, Isbn = "111", AuthorId = 1, Author = author },
            new Book { Id = 2, Title = "Book 2", PublicationYear = 2021, Isbn = "222", AuthorId = 1, Author = author }
        };
        _bookRepository.Setup(r => r.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(books);

        // Act
        var result = await _useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("Book 1", result[0].Title);
        Assert.Equal(2020, result[0].PublicationYear);
        Assert.Equal("111", result[0].Isbn);
        Assert.Equal(1, result[0].AuthorId);
        Assert.Equal("John Doe", result[0].AuthorFullName);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoBooks_ReturnsEmptyList()
    {
        // Arrange
        _bookRepository.Setup(r => r.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(new List<Book>());

        // Act
        var result = await _useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        _bookRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book>());
        var cancellationToken = new CancellationToken(false);

        // Act
        await _useCase.ExecuteAsync(cancellationToken);

        // Assert
        _bookRepository.Verify(r => r.GetAllAsync(cancellationToken), Times.Once);
    }
}
