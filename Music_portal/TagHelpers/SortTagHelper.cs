using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Music_portal.TagHelpers;

/// <summary>
/// TagHelper сортировки с сохранением фильтрации и пагинации
/// Использование: <sort-link sort-by="title" current-sort-by="@Model.SortBy" search="@Model.SearchTerm" genre-ids="@Model.SelectedGenreIds" author-ids="@Model.SelectedAuthorIds">По названию</sort-link>
/// Генерирует <a href="?search=&genreIds=&authorIds=&sortBy=&page=1"> с active классом
/// </summary>
[HtmlTargetElement("sort-link")]
public class SortTagHelper : TagHelper
{
    [HtmlAttributeName("sort-by")]
    public string SortBy { get; set; } = "date";

    [HtmlAttributeName("current-sort-by")]
    public string CurrentSortBy { get; set; } = "date";

    [HtmlAttributeName("search")]
    public string? Search { get; set; }

    [HtmlAttributeName("genre-ids")]
    public List<int>? GenreIds { get; set; }

    [HtmlAttributeName("author-ids")]
    public List<int>? AuthorIds { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "a";
        output.TagMode = TagMode.StartTagAndEndTag;

        var query = BuildQuery(Search, GenreIds, AuthorIds, SortBy, 1);
        output.Attributes.SetAttribute("href", "?" + query);

        var isActive = string.Equals(SortBy, CurrentSortBy, StringComparison.OrdinalIgnoreCase);
        var existingClass = output.Attributes["class"]?.Value?.ToString() ?? "";
        output.Attributes.SetAttribute("class", (existingClass + (isActive ? " active fw-bold text-decoration-underline" : "")).Trim());

        if (isActive)
            output.Attributes.SetAttribute("aria-current", "page");
    }

    private static string BuildQuery(string? search, List<int>? genreIds, List<int>? authorIds, string sortBy, int page)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(search))
            parts.Add($"search={Uri.EscapeDataString(search)}");
        if (genreIds != null)
            foreach (var id in genreIds)
                parts.Add($"genreIds={id}");
        if (authorIds != null)
            foreach (var id in authorIds)
                parts.Add($"authorIds={id}");
        if (!string.IsNullOrWhiteSpace(sortBy))
            parts.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
        parts.Add($"page={page}");
        return string.Join("&", parts);
    }
}
