namespace Music.bisLog.Dtos;

public class AdminSongListDto
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public List<SongDto> Songs { get; set; } = new();
}