using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Books;
using Moq;
using Xunit;

namespace LibraryApi.Controllers.Tests.UseCases.Books;

public class CreateBookUseCaseTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly Mock<IAuthorRepository> _authorRepository;
    private readonly CreateBookUseCase _useCase;

    public CreateBookUseCaseTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _authorRepository = new Mock<IAuthorRepository>();
        _useCase = new CreateBookUseCase(_bookRepository.Object, _authorRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorExists_CreatesBookAndReturnsDto()
    {
        // Arrange
        var author = new Author { Id = 1, FirstName = "John", LastName = "Doe", Country = "USA" };
        _authorRepository.Setup(r => r.GetByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(author);

        var request = new SaveBookDto
        {
            Title = "The Great Book",
            PublicationYear = 2023,
            Isbn = "123-456",
            AuthorId = 1
        };

        // Act
        var result = await _useCase.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("The Great Book", result.Title);
        Assert.Equal(2023, result.PublicationYear);
        Assert.Equal("123-456", result.Isbn);
        Assert.Equal(1, result.AuthorId);
        Assert.Equal("John Doe", result.AuthorFullName);

        _bookRepository.Verify(r => r.AddAsync(It.Is<Book>(b =>
            b.Title == "The Great Book" &&
            b.PublicationYear == 2023 &&
            b.Isbn == "123-456" &&
            b.AuthorId == 1),
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorNotFound_ReturnsNull()
    {
        // Arrange
        _authorRepository.Setup(r => r.GetByIdAsync(999, CancellationToken.None))
            .ReturnsAsync((Author?)null);

        var request = new SaveBookDto
        {
            Title = "The Great Book",
            PublicationYear = 2023,
            Isbn = "123-456",
            AuthorId = 999
        };

        // Act
        var result = await _useCase.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _bookRepository.Verify(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        var author = new Author { Id = 1, FirstName = "Jane", LastName = "Smith", Country = "UK" };
        _authorRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);
        var cancellationToken = new CancellationToken(false);
        var request = new SaveBookDto
        {
            Title = "Book",
            PublicationYear = 2020,
            Isbn = "999",
            AuthorId = 1
        };

        // Act
        await _useCase.ExecuteAsync(request, cancellationToken);

        // Assert
        _authorRepository.Verify(r => r.GetByIdAsync(1, cancellationToken), Times.Once);
        _bookRepository.Verify(r => r.AddAsync(It.IsAny<Book>(), cancellationToken), Times.Once);
    }
}
