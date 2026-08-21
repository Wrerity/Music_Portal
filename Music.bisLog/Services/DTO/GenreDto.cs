namespace Music.bisLog.Dtos;

public class GenreDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SongCount { get; set; }
}