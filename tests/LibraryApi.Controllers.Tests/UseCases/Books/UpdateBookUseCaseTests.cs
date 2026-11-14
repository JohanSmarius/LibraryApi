using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Books;
using Moq;
using Xunit;

namespace LibraryApi.Controllers.Tests.UseCases.Books;

public class UpdateBookUseCaseTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly Mock<IAuthorRepository> _authorRepository;
    private readonly UpdateBookUseCase _useCase;

    public UpdateBookUseCaseTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _authorRepository = new Mock<IAuthorRepository>();
        _useCase = new UpdateBookUseCase(_bookRepository.Object, _authorRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBookAndAuthorExist_ReturnsUpdatedDto()
    {
        // Arrange
        var existingAuthor = new Author { Id = 1, FirstName = "John", LastName = "Doe", Country = "USA" };
        var newAuthor = new Author { Id = 2, FirstName = "Jane", LastName = "Smith", Country = "UK" };

        var book = new Book
        {
            Id = 1,
            Title = "Old Title",
            PublicationYear = 2020,
            Isbn = "111",
            AuthorId = 1,
            Author = existingAuthor
        };

        _bookRepository.Setup(r => r.GetByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(book);
        _authorRepository.Setup(r => r.GetByIdAsync(2, CancellationToken.None))
            .ReturnsAsync(newAuthor);

        var request = new SaveBookDto
        {
            Title = "New Title",
            PublicationYear = 2023,
            Isbn = "222",
            AuthorId = 2
        };

        // Act
        var result = await _useCase.ExecuteAsync(1, request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Title", result.Title);
        Assert.Equal(2023, result.PublicationYear);
        Assert.Equal("222", result.Isbn);
        Assert.Equal(2, result.AuthorId);
        Assert.Equal("Jane Smith", result.AuthorFullName);

        _bookRepository.Verify(r => r.Update(It.Is<Book>(b =>
            b.Title == "New Title" &&
            b.PublicationYear == 2023 &&
            b.Isbn == "222" &&
            b.AuthorId == 2)), Times.Once);
        _bookRepository.Verify(r => r.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBookNotFound_ReturnsNull()
    {
        // Arrange
        _bookRepository.Setup(r => r.GetByIdAsync(999, CancellationToken.None))
            .ReturnsAsync((Book?)null);

        var request = new SaveBookDto
        {
            Title = "New Title",
            PublicationYear = 2023,
            Isbn = "222",
            AuthorId = 1
        };

        // Act
        var result = await _useCase.ExecuteAsync(999, request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _authorRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _bookRepository.Verify(r => r.Update(It.IsAny<Book>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAuthorNotFound_ReturnsNull()
    {
        // Arrange
        var book = new Book
        {
            Id = 1,
            Title = "Old Title",
            PublicationYear = 2020,
            Isbn = "111",
            AuthorId = 1,
            Author = new Author { Id = 1, FirstName = "John", LastName = "Doe" }
        };
        _bookRepository.Setup(r => r.GetByIdAsync(1, CancellationToken.None))
            .ReturnsAsync(book);
        _authorRepository.Setup(r => r.GetByIdAsync(999, CancellationToken.None))
            .ReturnsAsync((Author?)null);

        var request = new SaveBookDto
        {
            Title = "New Title",
            PublicationYear = 2023,
            Isbn = "222",
            AuthorId = 999
        };

        // Act
        var result = await _useCase.ExecuteAsync(1, request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _bookRepository.Verify(r => r.Update(It.IsAny<Book>()), Times.Never);
        _bookRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        var book = new Book
        {
            Id = 1,
            Title = "Old",
            AuthorId = 1,
            Author = new Author { Id = 1, FirstName = "Old", LastName = "Author" }
        };
        var newAuthor = new Author { Id = 2, FirstName = "New", LastName = "Author" };
        _bookRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
        _authorRepository.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newAuthor);
        var cancellationToken = new CancellationToken(false);
        var request = new SaveBookDto
        {
            Title = "New",
            PublicationYear = 2023,
            Isbn = "999",
            AuthorId = 2
        };

        // Act
        await _useCase.ExecuteAsync(1, request, cancellationToken);

        // Assert
        _bookRepository.Verify(r => r.GetByIdAsync(1, cancellationToken), Times.Once);
        _authorRepository.Verify(r => r.GetByIdAsync(2, cancellationToken), Times.Once);
        _bookRepository.Verify(r => r.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
