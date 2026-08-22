using System.ComponentModel.DataAnnotations;

namespace LibraryApi.MinimalApi.Dtos;

/// <summary>Response shape for a book.</summary>
public record BookDto(int Id, string Title, int PublicationYear, string Isbn, int AuthorId, string AuthorFullName);

/// <summary>Request shape for creating a book under a specific author (nested route).</summary>
public class CreateBookDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1450, 2100)]
    public int PublicationYear { get; set; }

    [Required, MaxLength(20)]
    public string Isbn { get; set; } = string.Empty;
}

/// <summary>Request shape for creating or updating a book through the flat route.</summary>
public class SaveBookDto : CreateBookDto
{
    [Range(1, int.MaxValue)]
    public int AuthorId { get; set; }
}
