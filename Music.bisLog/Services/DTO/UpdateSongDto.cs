namespace Music.bisLog.Dtos;

public class UpdateSongDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int Duration { get; set; }
    public string? Lyrics { get; set; }
    public int[] AuthorIds { get; set; } = Array.Empty<int>();
    public int[] GenreIds { get; set; } = Array.Empty<int>();
    public string? NewAuthorName { get; set; }
    public Stream? AudioStream { get; set; }
    public string? AudioFileName { get; set; }
}