namespace LibraryApi.Blazor.Models;

public record AuthorDto(int Id, string FirstName, string LastName, string Country, int BookCount);

public class CreateAuthorDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
