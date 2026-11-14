using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;
using LibraryApi.Controllers.UseCases.Authors;
using Moq;
using Xunit;

namespace LibraryApi.Controllers.Tests.UseCases.Authors;

public class CreateAuthorUseCaseTests
{
    private readonly Mock<IAuthorRepository> _authorRepository;
    private readonly CreateAuthorUseCase _useCase;

    public CreateAuthorUseCaseTests()
    {
        _authorRepository = new Mock<IAuthorRepository>();
        _useCase = new CreateAuthorUseCase(_authorRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesAuthorAndReturnsDto()
    {
        // Arrange
        var request = new CreateAuthorDto
        {
            FirstName = "John",
            LastName = "Doe",
            Country = "USA"
        };

        // Act
        var result = await _useCase.ExecuteAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("USA", result.Country);
        Assert.Equal(0, result.BookCount);

        _authorRepository.Verify(r => r.AddAsync(It.Is<Author>(a =>
            a.FirstName == "John" && a.LastName == "Doe" && a.Country == "USA"),
            CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_PassesCancellationToken()
    {
        // Arrange
        var request = new CreateAuthorDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Country = "UK"
        };
        var cancellationToken = new CancellationToken(false);

        // Act
        await _useCase.ExecuteAsync(request, cancellationToken);

        // Assert
        _authorRepository.Verify(r => r.AddAsync(It.IsAny<Author>(), cancellationToken), Times.Once);
    }
}
