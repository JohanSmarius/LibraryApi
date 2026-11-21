using LibraryApi.Controllers.Commands.Books;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Entities;

namespace LibraryApi.Tests;

public class BookCommandTests
{
    private static SaveBookDto Request() => new() { Title = "Title", PublicationYear = 2001, Isbn = "ISBN", AuthorId = 12 };
    private static Author Author() => new() { Id = 12, FirstName = "Octavia", LastName = "Butler" };

    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public async Task Create_flat_and_nested_check_author_and_map_book(bool nested, bool exists)
    {
        var author = Author();
        var authors = new AuthorRepositoryFake { Found = exists ? author : null };
        var books = new BookRepositoryFake();
        using var cts = new CancellationTokenSource();
        var request = Request();
        var result = nested
            ? await new CreateBookForAuthorCommandHandler(authors, books).HandleAsync(new(12, request), cts.Token)
            : await new CreateBookCommandHandler(authors, books).HandleAsync(new(request), cts.Token);

        Assert.Equal(exists ? new BookDto(84, "Title", 2001, "ISBN", 12, "Octavia Butler") : null, result);
        Assert.Equal(new[] { "Find" }, authors.Calls);
        Assert.Equal(12, authors.Id);
        Assert.Equal(cts.Token, authors.Token);
        Assert.Equal(exists ? new[] { "Add", "Save" } : Array.Empty<string>(), books.Calls);
        if (exists)
        {
            Assert.NotNull(books.Added);
            Assert.Equal(("Title", 2001, "ISBN", 12), (books.Added.Title, books.Added.PublicationYear, books.Added.Isbn, books.Added.AuthorId));
            Assert.Same(author, books.Added.Author);
            Assert.Equal(cts.Token, books.Token);
        }
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task Update_requires_book_and_author_before_mutating(bool bookExists, bool authorExists)
    {
        var author = Author();
        var book = new Book { Id = 9, Title = "Old", PublicationYear = 1990, Isbn = "Old ISBN", AuthorId = 3 };
        var books = new BookRepositoryFake { Found = bookExists ? book : null };
        var authors = new AuthorRepositoryFake { Found = authorExists ? author : null };
        using var cts = new CancellationTokenSource();
        var result = await new UpdateBookCommandHandler(authors, books).HandleAsync(new(9, Request()), cts.Token);

        Assert.Equal(bookExists && authorExists ? new BookDto(9, "Title", 2001, "ISBN", 12, "Octavia Butler") : null, result);
        Assert.Equal(bookExists && authorExists ? new[] { "Find", "Save" } : new[] { "Find" }, books.Calls);
        Assert.Equal(bookExists ? new[] { "Find" } : Array.Empty<string>(), authors.Calls);
        Assert.Equal(9, books.Id);
        Assert.Equal(cts.Token, books.Token);
        if (bookExists) { Assert.Equal(12, authors.Id); Assert.Equal(cts.Token, authors.Token); }
        if (bookExists && authorExists)
        {
            Assert.Equal(("Title", 2001, "ISBN", 12), (book.Title, book.PublicationYear, book.Isbn, book.AuthorId));
            Assert.Same(author, book.Author);
        }
        else Assert.Equal(("Old", 1990, "Old ISBN", 3), (book.Title, book.PublicationYear, book.Isbn, book.AuthorId));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_removes_only_existing_book(bool exists)
    {
        var book = new Book { Id = 9 };
        var repo = new BookRepositoryFake { Found = exists ? book : null };
        using var cts = new CancellationTokenSource();
        var result = await new DeleteBookCommandHandler(repo).HandleAsync(new(9), cts.Token);
        Assert.Equal(exists, result);
        Assert.Same(exists ? book : null, repo.Removed);
        Assert.Equal(exists ? new[] { "Find", "Remove", "Save" } : new[] { "Find" }, repo.Calls);
        Assert.Equal(9, repo.Id);
        Assert.Equal(cts.Token, repo.Token);
    }
}