using LibraryApi.Controllers.Entities;

namespace LibraryApi.Controllers.Data;

/// <summary>
/// Applies deterministic seed data if the database is empty.
/// Combined with the reset script (reset-db.sh / reset-db.ps1), this means
/// a wiped SQLite file always comes back to the exact same known state -
/// no manually re-entering test data live on stage.
/// </summary>
public static class SeedData
{
    public static void EnsureSeeded(LibraryDbContext context)
    {
        if (context.Authors.Any())
        {
            return;
        }

        var tolkien = new Author { FirstName = "J.R.R.", LastName = "Tolkien", Country = "United Kingdom" };
        var christie = new Author { FirstName = "Agatha", LastName = "Christie", Country = "United Kingdom" };
        var king = new Author { FirstName = "Stephen", LastName = "King", Country = "United States" };

        tolkien.Books.Add(new Book { Title = "The Hobbit", PublicationYear = 1937, Isbn = "978-0547928227" });
        tolkien.Books.Add(new Book { Title = "The Fellowship of the Ring", PublicationYear = 1954, Isbn = "978-0547928210" });

        christie.Books.Add(new Book { Title = "Murder on the Orient Express", PublicationYear = 1934, Isbn = "978-0062693662" });

        king.Books.Add(new Book { Title = "The Shining", PublicationYear = 1977, Isbn = "978-0385121675" });
        king.Books.Add(new Book { Title = "Pet Sematary", PublicationYear = 1983, Isbn = "978-0385182447" });
        king.Books.Add(new Book { Title = "It", PublicationYear = 1986, Isbn = "978-0670813025" });
        king.Books.Add(new Book { Title = "Misery", PublicationYear = 1987, Isbn = "978-0670813643" });

        context.Authors.AddRange(tolkien, christie, king);
        context.SaveChanges();
    }
}
