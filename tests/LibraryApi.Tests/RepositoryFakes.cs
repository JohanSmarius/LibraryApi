using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;
using LibraryApi.Controllers.Repositories;

namespace LibraryApi.Tests;

internal class AuthorRepositoryFake : IAuthorRepository
{
    public List<string> Calls { get; } = [];
    public Author? Found { get; set; }
    public Author? Added { get; private set; }
    public Author? Removed { get; private set; }
    public AuthorDto? Dto { get; set; }
    public List<AuthorDto> Dtos { get; set; } = [];
    public bool Exists { get; set; }
    public int Id { get; private set; }
    private CancellationToken? token;
    public CancellationToken Token
    {
        get => token ?? default;
        private set { if (token.HasValue) Assert.Equal(token.Value, value); token = value; }
    }

    public Task<List<AuthorDto>> GetAllAsync(CancellationToken token) { Calls.Add("GetAll"); Token = token; return Task.FromResult(Dtos); }
    public Task<AuthorDto?> GetAsync(int id, CancellationToken token) { Calls.Add("Get"); Id = id; Token = token; return Task.FromResult(Dto); }
    public Task<bool> ExistsAsync(int id, CancellationToken token) { Calls.Add("Exists"); Id = id; Token = token; return Task.FromResult(Exists); }
    public Task<Author?> FindAsync(int id, CancellationToken token) { Calls.Add("Find"); Id = id; Token = token; return Task.FromResult(Found); }
    public Task<Author?> FindWithBooksAsync(int id, CancellationToken token) { Calls.Add("FindWithBooks"); Id = id; Token = token; return Task.FromResult(Found); }
    public void Add(Author author) { Calls.Add("Add"); Added = author; author.Id = 42; }
    public void Remove(Author author) { Calls.Add("Remove"); Removed = author; }
    public Task SaveChangesAsync(CancellationToken token) { Calls.Add("Save"); Token = token; return Task.CompletedTask; }
}

internal class BookRepositoryFake : IBookRepository
{
    public List<string> Calls { get; } = [];
    public Book? Found { get; set; }
    public Book? Added { get; private set; }
    public Book? Removed { get; private set; }
    public BookDto? Dto { get; set; }
    public List<BookDto> Dtos { get; set; } = [];
    public int Id { get; private set; }
    public int AuthorId { get; private set; }
    private CancellationToken? token;
    public CancellationToken Token
    {
        get => token ?? default;
        private set { if (token.HasValue) Assert.Equal(token.Value, value); token = value; }
    }

    public Task<List<BookDto>> GetAllAsync(CancellationToken token) { Calls.Add("GetAll"); Token = token; return Task.FromResult(Dtos); }
    public Task<BookDto?> GetAsync(int id, CancellationToken token) { Calls.Add("Get"); Id = id; Token = token; return Task.FromResult(Dto); }
    public Task<List<BookDto>> GetForAuthorAsync(int authorId, CancellationToken token) { Calls.Add("GetForAuthor"); AuthorId = authorId; Token = token; return Task.FromResult(Dtos); }
    public Task<BookDto?> GetForAuthorAsync(int authorId, int bookId, CancellationToken token) { Calls.Add("GetOneForAuthor"); AuthorId = authorId; Id = bookId; Token = token; return Task.FromResult(Dto); }
    public Task<Book?> FindAsync(int id, CancellationToken token) { Calls.Add("Find"); Id = id; Token = token; return Task.FromResult(Found); }
    public void Add(Book book) { Calls.Add("Add"); Added = book; book.Id = 84; }
    public void Remove(Book book) { Calls.Add("Remove"); Removed = book; }
    public Task SaveChangesAsync(CancellationToken token) { Calls.Add("Save"); Token = token; return Task.CompletedTask; }
}