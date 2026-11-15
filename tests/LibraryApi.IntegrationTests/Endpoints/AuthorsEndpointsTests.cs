using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;

namespace LibraryApi.IntegrationTests.Endpoints;

public class AuthorsEndpointsTests : IClassFixture<ApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ApplicationFactory _factory;

    public AuthorsEndpointsTests(ApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        ClearDatabase();
    }

    private void ClearDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        db.Books.ExecuteDelete();
        db.Authors.ExecuteDelete();
    }

    [Fact]
    public async Task GetAuthors_ReturnsEmptyList_WhenNoAuthors()
    {
        // Act
        var response = await _client.GetAsync("/api/authors");

        // Assert
        response.EnsureSuccessStatusCode();
        var authors = await response.Content.ReadFromJsonAsync<List<AuthorDto>>();
        Assert.NotNull(authors);
        Assert.Empty(authors);
    }

    [Fact]
    public async Task CreateAuthor_ReturnsCreated_WithAuthorDto()
    {
        // Arrange
        var request = new CreateAuthorDto
        {
            FirstName = "John",
            LastName = "Doe",
            Country = "USA"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/authors", request);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var author = await response.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);
        Assert.Equal("John", author.FirstName);
        Assert.Equal("Doe", author.LastName);
        Assert.Equal("USA", author.Country);
        Assert.Equal(0, author.BookCount);
        Assert.True(author.Id > 0);
    }

    [Fact]
    public async Task GetAuthor_ReturnsAuthor_WhenExists()
    {
        // Arrange
        var createRequest = new CreateAuthorDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Country = "UK"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/authors", createRequest);
        var createdAuthor = await createResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(createdAuthor);

        // Act
        var response = await _client.GetAsync($"/api/authors/{createdAuthor.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var author = await response.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);
        Assert.Equal(createdAuthor.Id, author.Id);
        Assert.Equal("Jane", author.FirstName);
        Assert.Equal("Smith", author.LastName);
        Assert.Equal("UK", author.Country);
    }

    [Fact]
    public async Task GetAuthor_ReturnsNotFound_WhenNotExists()
    {
        // Act
        var response = await _client.GetAsync("/api/authors/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuthor_ReturnsUpdatedAuthor_WhenExists()
    {
        // Arrange
        var createRequest = new CreateAuthorDto
        {
            FirstName = "Old",
            LastName = "Name",
            Country = "DE"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/authors", createRequest);
        var createdAuthor = await createResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(createdAuthor);

        var updateRequest = new UpdateAuthorDto
        {
            FirstName = "New",
            LastName = "Name",
            Country = "FR"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/authors/{createdAuthor.Id}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var author = await response.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);
        Assert.Equal(createdAuthor.Id, author.Id);
        Assert.Equal("New", author.FirstName);
        Assert.Equal("Name", author.LastName);
        Assert.Equal("FR", author.Country);
    }

    [Fact]
    public async Task UpdateAuthor_ReturnsNotFound_WhenNotExists()
    {
        // Arrange
        var updateRequest = new UpdateAuthorDto
        {
            FirstName = "New",
            LastName = "Name",
            Country = "FR"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/authors/999", updateRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthor_ReturnsNoContent_WhenExists()
    {
        // Arrange
        var createRequest = new CreateAuthorDto
        {
            FirstName = "Delete",
            LastName = "Me",
            Country = "JP"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/authors", createRequest);
        var createdAuthor = await createResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(createdAuthor);

        // Act
        var response = await _client.DeleteAsync($"/api/authors/{createdAuthor.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthor_ReturnsNotFound_WhenNotExists()
    {
        // Act
        var response = await _client.DeleteAsync("/api/authors/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthors_ReturnsAllAuthors_AfterCreation()
    {
        // Arrange
        var author1 = new CreateAuthorDto { FirstName = "Author1", LastName = "Last1", Country = "C1" };
        var author2 = new CreateAuthorDto { FirstName = "Author2", LastName = "Last2", Country = "C2" };
        await _client.PostAsJsonAsync("/api/authors", author1);
        await _client.PostAsJsonAsync("/api/authors", author2);

        // Act
        var response = await _client.GetAsync("/api/authors");

        // Assert
        response.EnsureSuccessStatusCode();
        var authors = await response.Content.ReadFromJsonAsync<List<AuthorDto>>();
        Assert.NotNull(authors);
        Assert.Equal(2, authors.Count);
    }
}
