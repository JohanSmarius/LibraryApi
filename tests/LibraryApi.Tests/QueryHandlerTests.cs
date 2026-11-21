using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Queries.Authors;
using LibraryApi.Controllers.Queries.Books;

namespace LibraryApi.Tests;

public class QueryHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Get_author_forwards_id_token_and_result(bool found)
    {
        var dto = found ? new AuthorDto(7, "Jane", "Austen", "UK", 3) : null;
        var repo = new AuthorRepositoryFake { Dto = dto };
        using var cts = new CancellationTokenSource();
        Assert.Same(dto, await new GetAuthorQueryHandler(repo).HandleAsync(new(7), cts.Token));
        Assert.Equal(new[] { "Get" }, repo.Calls);
        Assert.Equal(7, repo.Id);
        Assert.Equal(cts.Token, repo.Token);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Get_authors_returns_repository_list_including_empty(bool hasItems)
    {
        var list = hasItems ? new List<AuthorDto> { new(7, "Jane", "Austen", "UK", 3) } : [];
        var repo = new AuthorRepositoryFake { Dtos = list };
        using var cts = new CancellationTokenSource();
        Assert.Same(list, await new GetAuthorsQueryHandler(repo).HandleAsync(new(), cts.Token));
        Assert.Equal(new[] { "GetAll" }, repo.Calls);
        Assert.Equal(cts.Token, repo.Token);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task Get_book_flat_and_nested_forward_ids_token_and_result(bool nested, bool found)
    {
        var dto = found ? new BookDto(9, "Title", 2001, "ISBN", 12, "Octavia Butler") : null;
        var repo = new BookRepositoryFake { Dto = dto };
        using var cts = new CancellationTokenSource();
        var result = nested
            ? await new GetBookForAuthorQueryHandler(repo).HandleAsync(new(12, 9), cts.Token)
            : await new GetBookQueryHandler(repo).HandleAsync(new(9), cts.Token);
        Assert.Same(dto, result);
        Assert.Equal(new[] { nested ? "GetOneForAuthor" : "Get" }, repo.Calls);
        Assert.Equal(9, repo.Id);
        if (nested) Assert.Equal(12, repo.AuthorId);
        Assert.Equal(cts.Token, repo.Token);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Get_books_returns_repository_list_including_empty(bool hasItems)
    {
        var list = hasItems ? new List<BookDto> { new(9, "Title", 2001, "ISBN", 12, "Octavia Butler") } : [];
        var repo = new BookRepositoryFake { Dtos = list };
        using var cts = new CancellationTokenSource();
        Assert.Same(list, await new GetBooksQueryHandler(repo).HandleAsync(new(), cts.Token));
        Assert.Equal(new[] { "GetAll" }, repo.Calls);
        Assert.Equal(cts.Token, repo.Token);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task Get_books_for_author_checks_existence_before_querying(bool exists, bool hasItems)
    {
        var list = hasItems ? new List<BookDto> { new(9, "Title", 2001, "ISBN", 12, "Octavia Butler") } : [];
        var authors = new AuthorRepositoryFake { Exists = exists };
        var books = new BookRepositoryFake { Dtos = list };
        using var cts = new CancellationTokenSource();
        var result = await new GetBooksForAuthorQueryHandler(authors, books).HandleAsync(new(12), cts.Token);
        Assert.Same(exists ? list : null, result);
        Assert.Equal(new[] { "Exists" }, authors.Calls);
        Assert.Equal(12, authors.Id);
        Assert.Equal(cts.Token, authors.Token);
        Assert.Equal(exists ? new[] { "GetForAuthor" } : Array.Empty<string>(), books.Calls);
        if (exists) { Assert.Equal(12, books.AuthorId); Assert.Equal(cts.Token, books.Token); }
    }
}