namespace Music.bisLog.Dtos;

public class CatalogDto
{
    public string SearchTerm { get; set; } = "";
    public List<int> SelectedGenreIds { get; set; } = new();
    public string SortBy { get; set; } = "date";
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public List<SongDto> Songs { get; set; } = new();
    public List<GenreDto> AllGenres { get; set; } = new();
}
