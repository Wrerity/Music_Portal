using Music.bisLog.Dtos;

namespace Music_portal.ViewModels;

public class CatalogViewModel
{
    public string SearchTerm { get; set; } = "";
    public List<int> SelectedGenreIds { get; set; } = new();
    public List<int> SelectedAuthorIds { get; set; } = new();
    public string SortBy { get; set; } = "date";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public List<SongDisplayViewModel> Songs { get; set; } = new();
    public List<GenreDto> AllGenres { get; set; } = new();
    public List<AuthorDto> AllAuthors { get; set; } = new();
}
