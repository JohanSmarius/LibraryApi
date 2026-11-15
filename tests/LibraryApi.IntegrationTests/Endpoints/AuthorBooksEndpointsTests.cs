using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;

namespace LibraryApi.IntegrationTests.Endpoints;

public class AuthorBooksEndpointsTests : IClassFixture<ApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ApplicationFactory _factory;

    public AuthorBooksEndpointsTests(ApplicationFactory factory)
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
    public async Task GetBooksForAuthor_ReturnsEmptyList_WhenAuthorHasNoBooks()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto
        {
            FirstName = "NoBooks",
            LastName = "Author",
            Country = "USA"
        };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);

        // Act
        var response = await _client.GetAsync($"/api/authors/{author.Id}/books");

        // Assert
        response.EnsureSuccessStatusCode();
        var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
        Assert.NotNull(books);
        Assert.Empty(books);
    }

    [Fact]
    public async Task GetBooksForAuthor_ReturnsNotFound_WhenAuthorNotExists()
    {
        // Act
        var response = await _client.GetAsync("/api/authors/999/books");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBooksForAuthor_ReturnsBooks_WhenAuthorHasBooks()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto { FirstName = "Book", LastName = "Author", Country = "UK" };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);

        var book1 = new CreateBookDto { Title = "Book 1", PublicationYear = 2020, Isbn = "AB1" };
        var book2 = new CreateBookDto { Title = "Book 2", PublicationYear = 2021, Isbn = "AB2" };
        await _client.PostAsJsonAsync($"/api/authors/{author.Id}/books", book1);
        await _client.PostAsJsonAsync($"/api/authors/{author.Id}/books", book2);

        // Act
        var response = await _client.GetAsync($"/api/authors/{author.Id}/books");

        // Assert
        response.EnsureSuccessStatusCode();
        var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
        Assert.NotNull(books);
        Assert.Equal(2, books.Count);
    }

    [Fact]
    public async Task GetBookForAuthor_ReturnsBook_WhenExists()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto { FirstName = "Single", LastName = "Book", Country = "DE" };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);

        var bookRequest = new CreateBookDto { Title = "Single Book", PublicationYear = 2022, Isbn = "SB1" };
        var bookResponse = await _client.PostAsJsonAsync($"/api/authors/{author.Id}/books", bookRequest);
        var createdBook = await bookResponse.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(createdBook);

        // Act
        var response = await _client.GetAsync($"/api/authors/{author.Id}/books/{createdBook.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var book = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(book);
        Assert.Equal(createdBook.Id, book.Id);
        Assert.Equal("Single Book", book.Title);
    }

    [Fact]
    public async Task GetBookForAuthor_ReturnsNotFound_WhenAuthorNotExists()
    {
        // Act
        var response = await _client.GetAsync("/api/authors/999/books/1");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetBookForAuthor_ReturnsNotFound_WhenBookNotExists()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto { FirstName = "Missing", LastName = "Book", Country = "FR" };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);

        // Act
        var response = await _client.GetAsync($"/api/authors/{author.Id}/books/999");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateBookForAuthor_ReturnsCreated_WithBookDto()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto { FirstName = "Create", LastName = "Nested", Country = "JP" };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);

        var bookRequest = new CreateBookDto
        {
            Title = "Nested Book",
            PublicationYear = 2023,
            Isbn = "NB1"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/authors/{author.Id}/books", bookRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        var book = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(book);
        Assert.Equal("Nested Book", book.Title);
        Assert.Equal(2023, book.PublicationYear);
        Assert.Equal("NB1", book.Isbn);
        Assert.Equal(author.Id, book.AuthorId);
        Assert.Equal("Create Nested", book.AuthorFullName);
    }

    [Fact]
    public async Task CreateBookForAuthor_ReturnsNotFound_WhenAuthorNotExists()
    {
        // Arrange
        var bookRequest = new CreateBookDto
        {
            Title = "Orphan Book",
            PublicationYear = 2023,
            Isbn = "OB1"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/authors/999/books", bookRequest);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAuthor_CascadesToDeleteBooks()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto { FirstName = "Cascade", LastName = "Delete", Country = "CA" };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);

        var bookRequest = new CreateBookDto { Title = "Cascade Book", PublicationYear = 2021, Isbn = "CB1" };
        await _client.PostAsJsonAsync($"/api/authors/{author.Id}/books", bookRequest);

        // Verify book exists
        var booksResponse = await _client.GetAsync($"/api/authors/{author.Id}/books");
        var books = await booksResponse.Content.ReadFromJsonAsync<List<BookDto>>();
        Assert.NotNull(books);
        Assert.Single(books);

        // Act - delete author
        var deleteResponse = await _client.DeleteAsync($"/api/authors/{author.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify author is gone
        var authorGetResponse = await _client.GetAsync($"/api/authors/{author.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, authorGetResponse.StatusCode);

        // Verify books are also gone
        var allBooksResponse = await _client.GetAsync("/api/books");
        var allBooks = await allBooksResponse.Content.ReadFromJsonAsync<List<BookDto>>();
        Assert.NotNull(allBooks);
        Assert.Empty(allBooks);
    }

    [Fact]
    public async Task AuthorBookCount_ReflectsCreatedBooks()
    {
        // Arrange
        var authorRequest = new CreateAuthorDto { FirstName = "Count", LastName = "Test", Country = "AU" };
        var authorResponse = await _client.PostAsJsonAsync("/api/authors", authorRequest);
        var author = await authorResponse.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(author);
        Assert.Equal(0, author.BookCount);

        // Create books
        await _client.PostAsJsonAsync($"/api/authors/{author.Id}/books",
            new CreateBookDto { Title = "Book 1", PublicationYear = 2020, Isbn = "CT1" });
        await _client.PostAsJsonAsync($"/api/authors/{author.Id}/books",
            new CreateBookDto { Title = "Book 2", PublicationYear = 2021, Isbn = "CT2" });

        // Act
        var response = await _client.GetAsync($"/api/authors/{author.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedAuthor = await response.Content.ReadFromJsonAsync<AuthorDto>();
        Assert.NotNull(updatedAuthor);
        Assert.Equal(2, updatedAuthor.BookCount);
    }
}
