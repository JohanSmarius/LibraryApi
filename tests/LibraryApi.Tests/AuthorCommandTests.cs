using LibraryApi.Controllers.Commands.Authors;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Tests;

public class AuthorCommandTests
{
    [Fact]
    public async Task Create_maps_adds_and_saves_author()
    {
        var repo = new AuthorRepositoryFake();
        using var cts = new CancellationTokenSource();
        var result = await new CreateAuthorCommandHandler(repo).HandleAsync(
            new(new CreateAuthorDto { FirstName = "Ada", LastName = "Lovelace", Country = "UK" }), cts.Token);

        Assert.Equal(new AuthorDto(42, "Ada", "Lovelace", "UK", 0), result);
        Assert.NotNull(repo.Added);
        Assert.Equal(("Ada", "Lovelace", "UK"), (repo.Added.FirstName, repo.Added.LastName, repo.Added.Country));
        Assert.Equal(new[] { "Add", "Save" }, repo.Calls);
        Assert.Equal(cts.Token, repo.Token);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Update_saves_only_when_author_exists(bool exists)
    {
        var author = new Author { Id = 7, Books = [new Book(), new Book()] };
        var repo = new AuthorRepositoryFake { Found = exists ? author : null };
        using var cts = new CancellationTokenSource();
        var result = await new UpdateAuthorCommandHandler(repo).HandleAsync(
            new(7, new UpdateAuthorDto { FirstName = "Jane", LastName = "Austen", Country = "UK" }), cts.Token);

        Assert.Equal(exists ? new AuthorDto(7, "Jane", "Austen", "UK", 2) : null, result);
        Assert.Equal(exists ? new[] { "FindWithBooks", "Save" } : new[] { "FindWithBooks" }, repo.Calls);
        Assert.Equal(7, repo.Id);
        Assert.Equal(cts.Token, repo.Token);
        if (exists) Assert.Equal(("Jane", "Austen", "UK"), (author.FirstName, author.LastName, author.Country));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_removes_only_existing_author(bool exists)
    {
        var author = new Author { Id = 7 };
        var repo = new AuthorRepositoryFake { Found = exists ? author : null };
        using var cts = new CancellationTokenSource();
        var result = await new DeleteAuthorCommandHandler(repo).HandleAsync(new(7), cts.Token);

        Assert.Equal(exists, result);
        Assert.Same(exists ? author : null, repo.Removed);
        Assert.Equal(exists ? new[] { "Find", "Remove", "Save" } : new[] { "Find" }, repo.Calls);
        Assert.Equal(7, repo.Id);
        Assert.Equal(cts.Token, repo.Token);
    }
}