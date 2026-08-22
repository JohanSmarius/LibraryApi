namespace LibraryApi.MinimalApi.Entities;

public class Author
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    // Navigation property - deliberately left as a simple collection.
    // No repository/service layer wraps this; controllers query the
    // DbContext directly, which is exactly the "realistic but unlayered"
    // starting point the talk is about.
    public List<Book> Books { get; set; } = new();
}
