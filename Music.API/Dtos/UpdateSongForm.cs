namespace Music.API.Dtos;

public class UpdateSongForm
{
    public string Title { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public int Duration { get; set; }
    public string? Lyrics { get; set; }
    public List<int> AuthorIds { get; set; } = new();
    public List<int> GenreIds { get; set; } = new();
    public string? NewAuthorName { get; set; }
    public IFormFile? AudioFile { get; set; }
}