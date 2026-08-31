namespace Music.bisLog.Dtos;

public class SongDetailDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    // 7. Детальный GET возвращает объекты жанров/авторов с id+name, а не только строки
    public List<AuthorDto> Authors { get; set; } = new();
    public List<GenreDto> Genres { get; set; } = new();
    // Для обратной совместимости оставляем строковые представления
    public List<string> AuthorNames => Authors.Select(a => a.Name).ToList();
    public List<string> GenreNames => Genres.Select(g => g.Name).ToList();
    public int Duration { get; set; }
    public string DurationFormatted { get; set; } = string.Empty;
    public string? Lyrics { get; set; }
    public DateTime UploadDate { get; set; }
    public int PlayCount { get; set; }
}
