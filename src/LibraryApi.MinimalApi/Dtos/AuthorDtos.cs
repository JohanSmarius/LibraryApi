using System.ComponentModel.DataAnnotations;

namespace LibraryApi.MinimalApi.Dtos;

/// <summary>Response shape for an author.</summary>
public record AuthorDto(int Id, string FirstName, string LastName, string Country, int BookCount);

/// <summary>Request shape for creating an author.</summary>
public class CreateAuthorDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(60)]
    public string Country { get; set; } = string.Empty;
}

/// <summary>Request shape for updating an author.</summary>
public class UpdateAuthorDto : CreateAuthorDto
{
}
