namespace LibraryApi.Blazor.Models;

public record BookDto(int Id, string Title, int PublicationYear, string Isbn, int AuthorId, string AuthorFullName);

public class CreateBookDto
{
    public string Title { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public int AuthorId { get; set; }
}
