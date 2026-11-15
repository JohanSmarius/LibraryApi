using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;

namespace LibraryApi.IntegrationTests.Endpoints;

public class BooksEndpointsTests : IClassFixture<ApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ApplicationFactory _factory;

    public BooksEndpointsTests(ApplicationFactory factory)
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
    public async Task GetBooks_ReturnsEmptyList_WhenNoBooks()
    {
        // Act
        var response = await _client.GetAsync("/api/books");

        // Assert
        response.EnsureSuccessStatusCode();
        var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
        Assert.NotNull(books);
        Assert.Empty(books);
    }

    [Fact]
    public async Task CreateBook_ReturnsCreated_WithBookDto()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto
        {
            FirstName = "John",
            LastName = "Doe",
            Country = "USA"
        };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);

        var bookRequest = new SaveBookDto
        {
            Title = "The Great Book",
            PublicationYear = 2023,
            Isbn = "123-456",
            AuthorId = author.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", bookRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var book = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(book);
        Assert.Equal("The Great Book", book.Title);
        Assert.Equal(2023, book.PublicationYear);
        Assert.Equal("123-456", book.Isbn);
        Assert.Equal(author.Id, book.AuthorId);
        Assert.Equal("John Doe", book.AuthorFullName);
        Assert.True(book.Id > 0);
    }

    [Fact]
    public async Task CreateBook_ReturnsNotFound_WhenAuthorNotExists()
    {
        // Arrange
        var bookRequest = new SaveBookDto
        {
            Title = "The Great Book",
            PublicationYear = 2023,
            Isbn = "123-456",
            AuthorId = 999
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/books", bookRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBook_ReturnsBook_WhenExists()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto { FirstName = "Jane", LastName = "Smith", Country = "UK" };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);

        var bookRequest = new SaveBookDto
        {
            Title = "Another Book",
            PublicationYear = 2022,
            Isbn = "789-012",
            AuthorId = author.Id
        };
        var bookResponse = await _client.PostAsJsonAsync("/api/books", bookRequest);
        var createdBook = await bookResponse.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(createdBook);

        // Act
        var response = await _client.GetAsync($"/api/books/{createdBook.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var book = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(book);
        Assert.Equal(createdBook.Id, book.Id);
        Assert.Equal("Another Book", book.Title);
        Assert.Equal(2022, book.PublicationYear);
        Assert.Equal("789-012", book.Isbn);
    }

    [Fact]
    public async Task GetBook_ReturnsNotFound_WhenNotExists()
    {
        // Act
        var response = await _client.GetAsync("/api/books/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBook_ReturnsUpdatedBook_WhenExists()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto { FirstName = "Test", LastName = "Author", Country = "DE" };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);

        var bookRequest = new SaveBookDto
        {
            Title = "Original Title",
            PublicationYear = 2020,
            Isbn = "111-222",
            AuthorId = author.Id
        };
        var bookResponse = await _client.PostAsJsonAsync("/api/books", bookRequest);
        var createdBook = await bookResponse.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(createdBook);

        var updateRequest = new SaveBookDto
        {
            Title = "Updated Title",
            PublicationYear = 2024,
            Isbn = "333-444",
            AuthorId = author.Id
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/books/{createdBook.Id}", updateRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var book = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(book);
        Assert.Equal(createdBook.Id, book.Id);
        Assert.Equal("Updated Title", book.Title);
        Assert.Equal(2024, book.PublicationYear);
        Assert.Equal("333-444", book.Isbn);
    }

    [Fact]
    public async Task UpdateBook_ReturnsNotFound_WhenNotExists()
    {
        // Arrange
        var updateRequest = new SaveBookDto
        {
            Title = "Updated Title",
            PublicationYear = 2024,
            Isbn = "333-444",
            AuthorId = 1
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/books/999", updateRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_ReturnsNoContent_WhenExists()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto { FirstName = "Delete", LastName = "Test", Country = "JP" };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);

        var bookRequest = new SaveBookDto
        {
            Title = "Delete Me",
            PublicationYear = 2021,
            Isbn = "555-666",
            AuthorId = author.Id
        };
        var bookResponse = await _client.PostAsJsonAsync("/api/books", bookRequest);
        var createdBook = await bookResponse.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(createdBook);

        // Act
        var response = await _client.DeleteAsync($"/api/books/{createdBook.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_ReturnsNotFound_WhenNotExists()
    {
        // Act
        var response = await _client.DeleteAsync("/api/books/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBooks_ReturnsAllBooks_AfterCreation()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto { FirstName = "Multi", LastName = "Book", Country = "CA" };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);

        var book1 = new SaveBookDto { Title = "Book 1", PublicationYear = 2020, Isbn = "B1", AuthorId = author.Id };
        var book2 = new SaveBookDto { Title = "Book 2", PublicationYear = 2021, Isbn = "B2", AuthorId = author.Id };
        await _client.PostAsJsonAsync("/api/books", book1);
        await _client.PostAsJsonAsync("/api/books", book2);

        // Act
        var response = await _client.GetAsync("/api/books");

        // Assert
        response.EnsureSuccessStatusCode();
        var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
        Assert.NotNull(books);
        Assert.Equal(2, books.Count);
    }
}
