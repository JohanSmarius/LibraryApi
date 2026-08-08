namespace LibraryApi.Controllers.Entities;

public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int PublicationYear { get; set; }

    public string Isbn { get; set; } = string.Empty;

    public int AuthorId { get; set; }

    public Author? Author { get; set; }
}
