namespace Music.bisLog.Dtos;

public class SongDetailDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = new();
    public List<string> Genres { get; set; } = new();
    public int Duration { get; set; }
    public string DurationFormatted { get; set; } = string.Empty;
    public string? Lyrics { get; set; }
    public DateTime UploadDate { get; set; }
    public int PlayCount { get; set; }
}
