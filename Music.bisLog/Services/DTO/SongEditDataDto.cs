namespace Music.bisLog.Dtos;

public class SongEditDataDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? Lyrics { get; set; }
    public List<int> AuthorIds { get; set; } = new();
    public List<int> GenreIds { get; set; } = new();
    public List<AuthorDto> AllAuthors { get; set; } = new();
    public List<GenreDto> AllGenres { get; set; } = new();
}