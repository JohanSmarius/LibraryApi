using LibraryApi.Controllers.Commands.Authors;
using LibraryApi.Controllers.Commands.Books;
using LibraryApi.Controllers.Data;
using LibraryApi.Controllers.Dtos;
using LibraryApi.Controllers.Queries.Authors;
using LibraryApi.Controllers.Queries.Books;
using LibraryApi.Controllers.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();
var options = new DbContextOptionsBuilder<LibraryDbContext>().UseSqlite(connection).Options;
await using (var setup = new LibraryDbContext(options))
{
    await setup.Database.EnsureCreatedAsync();
}

const int missing = 99999;
var ct = CancellationToken.None;
int authorId;
int otherAuthorId;
int bookId;
int nestedBookId;

await using (var db = new LibraryDbContext(options))
{
    Check((await new GetAuthorsQueryHandler(new AuthorRepository(db)).HandleAsync(new GetAuthorsQuery(), ct)).Count == 0, "initial authors empty");
    Check((await new GetBooksQueryHandler(new BookRepository(db)).HandleAsync(new GetBooksQuery(), ct)).Count == 0, "initial books empty");
    Check(await new GetAuthorQueryHandler(new AuthorRepository(db)).HandleAsync(new GetAuthorQuery(missing), ct) is null, "missing author query");
    Check(await new GetBookQueryHandler(new BookRepository(db)).HandleAsync(new GetBookQuery(missing), ct) is null, "missing book query");
    Check(await new GetBooksForAuthorQueryHandler(new AuthorRepository(db), new BookRepository(db)).HandleAsync(new GetBooksForAuthorQuery(missing), ct) is null, "missing author's books");
    Check(await new GetBookForAuthorQueryHandler(new BookRepository(db)).HandleAsync(new GetBookForAuthorQuery(missing, missing), ct) is null, "missing nested book");
    Check(await new UpdateAuthorCommandHandler(new AuthorRepository(db)).HandleAsync(new UpdateAuthorCommand(missing, new UpdateAuthorDto()), ct) is null, "update missing author");
    Check(!await new DeleteAuthorCommandHandler(new AuthorRepository(db)).HandleAsync(new DeleteAuthorCommand(missing), ct), "delete missing author");
    Check(await new CreateBookCommandHandler(new AuthorRepository(db), new BookRepository(db)).HandleAsync(new CreateBookCommand(Book("missing", "missing-1", missing)), ct) is null, "create book with missing author");
    Check(await new CreateBookForAuthorCommandHandler(new AuthorRepository(db), new BookRepository(db)).HandleAsync(new CreateBookForAuthorCommand(missing, NestedBook("missing", "missing-2")), ct) is null, "create nested book with missing author");
    Check(await new UpdateBookCommandHandler(new AuthorRepository(db), new BookRepository(db)).HandleAsync(new UpdateBookCommand(missing, Book("missing", "missing-3", missing)), ct) is null, "update missing book");
    Check(!await new DeleteBookCommandHandler(new BookRepository(db)).HandleAsync(new DeleteBookCommand(missing), ct), "delete missing book");

    var author = await new CreateAuthorCommandHandler(new AuthorRepository(db)).HandleAsync(new CreateAuthorCommand(new CreateAuthorDto { FirstName = "Ada", LastName = "Lovelace", Country = "UK" }), ct);
    var other = await new CreateAuthorCommandHandler(new AuthorRepository(db)).HandleAsync(new CreateAuthorCommand(new CreateAuthorDto { FirstName = "Grace", LastName = "Hopper", Country = "US" }), ct);
    authorId = author.Id;
    otherAuthorId = other.Id;
    Check(authorId > 0 && otherAuthorId != authorId && author.BookCount == 0, "created authors");
    var empty = await new GetBooksForAuthorQueryHandler(new AuthorRepository(db), new BookRepository(db)).HandleAsync(new GetBooksForAuthorQuery(authorId), ct);
    Check(empty is { Count: 0 }, "existing author has empty book list");

    var book = await new CreateBookCommandHandler(new AuthorRepository(db), new BookRepository(db)).HandleAsync(new CreateBookCommand(Book("First", "isbn-1", authorId)), ct);
    var nested = await new CreateBookForAuthorCommandHandler(new AuthorRepository(db), new BookRepository(db)).HandleAsync(new CreateBookForAuthorCommand(authorId, NestedBook("Second", "isbn-2")), ct);
    Check(book is not null && book.AuthorId == authorId && book.AuthorFullName == "Ada Lovelace", "create flat book");
    Check(nested is not null && nested.AuthorId == authorId && nested.AuthorFullName == "Ada Lovelace", "create nested book");
    bookId = book!.Id;
    nestedBookId = nested!.Id;
    Check(bookId > 0 && nestedBookId != bookId, "created book identifiers");
}

await using (var db = new LibraryDbContext(options))
{
    var authors = await new GetAuthorsQueryHandler(new AuthorRepository(db)).HandleAsync(new GetAuthorsQuery(), ct);
    Check(authors.Count == 2 && authors.Single(a => a.Id == authorId).BookCount == 2, "persisted authors and book count");
    var author = await new GetAuthorQueryHandler(new AuthorRepository(db)).HandleAsync(new GetAuthorQuery(authorId), ct);
    Check(author is { FirstName: "Ada", LastName: "Lovelace", Country: "UK", BookCount: 2 }, "persisted author query");
    var books = await new GetBooksQueryHandler(new BookRepository(db)).HandleAsync(new GetBooksQuery(), ct);
    Check(books.Count == 2 && books.Any(b => b.Id == bookId) && books.Any(b => b.Id == nestedBookId), "persisted books query");
    var book = await new GetBookQueryHandler(new BookRepository(db)).HandleAsync(new GetBookQuery(bookId), ct);
    Check(book is { Title: "First", PublicationYear: 2000, Isbn: "isbn-1", AuthorFullName: "Ada Lovelace" }, "persisted book query");
    var nestedBooks = await new GetBooksForAuthorQueryHandler(new AuthorRepository(db), new BookRepository(db)).HandleAsync(new GetBooksForAuthorQuery(authorId), ct);
    Check(nestedBooks is { Count: 2 } && nestedBooks.Any(b => b.Id == nestedBookId), "nested book list");
    Check((await new GetBookForAuthorQueryHandler(new BookRepository(db)).HandleAsync(new GetBookForAuthorQuery(authorId, nestedBookId), ct))?.Id == nestedBookId, "nested book query");
    Check(await new GetBookForAuthorQueryHandler(new BookRepository(db)).HandleAsync(new GetBookForAuthorQuery(otherAuthorId, nestedBookId), ct) is null, "nested author mismatch");
    Check(await new GetBookForAuthorQueryHandler(new BookRepository(db)).HandleAsync(new GetBookForAuthorQuery(authorId, missing), ct) is null, "missing book under existing author");

    var updatedAuthor = await new UpdateAuthorCommandHandler(new AuthorRepository(db)).HandleAsync(new UpdateAuthorCommand(authorId, new UpdateAuthorDto { FirstName = "Augusta", LastName = "King", Country = "England" }), ct);
    Check(updatedAuthor is { FirstName: "Augusta", LastName: "King", Country: "England", BookCount: 2 }, "update author");
    Check(await new UpdateBookCommandHandler(new AuthorRepository(db), new BookRepository(db)).HandleAsync(new UpdateBookCommand(bookId, Book("Invalid", "isbn-invalid", missing)), ct) is null, "update book with missing author");
    var updatedBook = await new UpdateBookCommandHandler(new AuthorRepository(db), new BookRepository(db)).HandleAsync(new UpdateBookCommand(bookId, Book("Updated", "isbn-updated", otherAuthorId)), ct);
    Check(updatedBook is { Title: "Updated", Isbn: "isbn-updated", AuthorFullName: "Grace Hopper" } && updatedBook.AuthorId == otherAuthorId, "update and reassign book");
}

await using (var db = new LibraryDbContext(options))
{
    var author = await new GetAuthorQueryHandler(new AuthorRepository(db)).HandleAsync(new GetAuthorQuery(authorId), ct);
    Check(author is { FirstName: "Augusta", LastName: "King", Country: "England", BookCount: 1 }, "author update persisted");
    var book = await new GetBookQueryHandler(new BookRepository(db)).HandleAsync(new GetBookQuery(bookId), ct);
    Check(book is { Title: "Updated", PublicationYear: 2000, Isbn: "isbn-updated", AuthorFullName: "Grace Hopper" } && book.AuthorId == otherAuthorId, "book update persisted");
    Check(await new GetBookForAuthorQueryHandler(new BookRepository(db)).HandleAsync(new GetBookForAuthorQuery(authorId, bookId), ct) is null, "old author no longer owns book");
    Check(await new DeleteAuthorCommandHandler(new AuthorRepository(db)).HandleAsync(new DeleteAuthorCommand(authorId), ct), "delete author");
}

await using (var db = new LibraryDbContext(options))
{
    Check(await new GetAuthorQueryHandler(new AuthorRepository(db)).HandleAsync(new GetAuthorQuery(authorId), ct) is null, "author deletion persisted");
    Check(await new GetBookQueryHandler(new BookRepository(db)).HandleAsync(new GetBookQuery(nestedBookId), ct) is null, "cascade deletes author's book");
    Check((await new GetBookQueryHandler(new BookRepository(db)).HandleAsync(new GetBookQuery(bookId), ct)) is not null, "cascade preserves reassigned book");
    Check(await new DeleteBookCommandHandler(new BookRepository(db)).HandleAsync(new DeleteBookCommand(bookId), ct), "delete book");
    Check(!await new DeleteBookCommandHandler(new BookRepository(db)).HandleAsync(new DeleteBookCommand(bookId), ct), "delete book twice");
    Check(!await new DeleteAuthorCommandHandler(new AuthorRepository(db)).HandleAsync(new DeleteAuthorCommand(authorId), ct), "delete author twice");
}

await using (var db = new LibraryDbContext(options))
{
    Check(await new GetBookQueryHandler(new BookRepository(db)).HandleAsync(new GetBookQuery(bookId), ct) is null, "book deletion persisted");
    Check((await new GetBooksQueryHandler(new BookRepository(db)).HandleAsync(new GetBooksQuery(), ct)).Count == 0, "books empty after deletes");
    Check((await new GetBooksForAuthorQueryHandler(new AuthorRepository(db), new BookRepository(db)).HandleAsync(new GetBooksForAuthorQuery(otherAuthorId), ct)) is { Count: 0 }, "remaining author has no books");
}

await using (var db = new LibraryDbContext(options))
{
    using var cancelled = new CancellationTokenSource();
    cancelled.Cancel();
    try
    {
        await new GetBooksQueryHandler(new BookRepository(db)).HandleAsync(new GetBooksQuery(), cancelled.Token);
        throw new InvalidOperationException("Assertion failed: cancelled query");
    }
    catch (OperationCanceledException)
    {
    }
}

Console.WriteLine("CQRS smoke tests passed.");

static void Check(bool condition, string description)
{
    if (!condition) throw new InvalidOperationException($"Assertion failed: {description}");
}

static SaveBookDto Book(string title, string isbn, int authorId) => new()
{
    Title = title, PublicationYear = 2000, Isbn = isbn, AuthorId = authorId
};

static CreateBookDto NestedBook(string title, string isbn) => new()
{
    Title = title, PublicationYear = 2000, Isbn = isbn
};