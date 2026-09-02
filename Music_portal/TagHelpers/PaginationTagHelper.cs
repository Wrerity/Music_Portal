using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Music_portal.TagHelpers;

/// <summary>
/// TagHelper пагинации с сортировкой и фильтрацией
/// Использование: <pagination total-pages="@Model.TotalPages" current-page="@Model.Page" search="@Model.SearchTerm" sort-by="@Model.SortBy" genre-ids="@Model.SelectedGenreIds" author-ids="@Model.SelectedAuthorIds" />
/// Генерирует <nav><ul class="pagination">...</ul></nav> с сохранением всех фильтров
/// </summary>
[HtmlTargetElement("pagination")]
public class PaginationTagHelper : TagHelper
{
    [HtmlAttributeName("total-pages")]
    public int TotalPages { get; set; }

    [HtmlAttributeName("current-page")]
    public int CurrentPage { get; set; }

    [HtmlAttributeName("search")]
    public string? Search { get; set; }

    [HtmlAttributeName("sort-by")]
    public string? SortBy { get; set; }

    [HtmlAttributeName("genre-ids")]
    public List<int>? GenreIds { get; set; }

    [HtmlAttributeName("author-ids")]
    public List<int>? AuthorIds { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "nav";
        output.Attributes.SetAttribute("class", "mt-4");
        output.Attributes.SetAttribute("aria-label", "Pagination");

        if (TotalPages <= 1)
        {
            output.SuppressOutput();
            return;
        }

        var ul = "<ul class=\"pagination justify-content-center\">";

        // Предыдущая
        ul += BuildItem(CurrentPage > 1 ? CurrentPage - 1 : -1, "«", CurrentPage == 1);

        // Логика с ... для больших страниц
        var pages = GetPages();
        int prev = -1;
        foreach (var p in pages)
        {
            if (prev != -1 && p - prev > 1)
                ul += "<li class=\"page-item disabled\"><span class=\"page-link\">…</span></li>";
            ul += BuildItem(p, p.ToString(), p == CurrentPage, p == CurrentPage);
            prev = p;
        }

        // Следующая
        ul += BuildItem(CurrentPage < TotalPages ? CurrentPage + 1 : -1, "»", CurrentPage == TotalPages);

        ul += "</ul>";
        output.Content.SetHtmlContent(ul);
    }

    private List<int> GetPages()
    {
        var list = new List<int>();
        if (TotalPages <= 7)
        {
            for (int i = 1; i <= TotalPages; i++) list.Add(i);
            return list;
        }
        list.Add(1);
        if (CurrentPage > 3) list.Add(CurrentPage - 1);
        if (CurrentPage > 1 && CurrentPage < TotalPages) list.Add(CurrentPage);
        if (CurrentPage < TotalPages - 2) list.Add(CurrentPage + 1);
        list.Add(TotalPages);
        // Добавляем соседей для первой/последней
        if (!list.Contains(2) && CurrentPage <= 3) list.Add(2);
        if (!list.Contains(TotalPages - 1) && CurrentPage >= TotalPages - 2) list.Add(TotalPages - 1);
        list = list.Distinct().OrderBy(x => x).ToList();
        return list;
    }

    private string BuildItem(int page, string text, bool disabled, bool active = false)
    {
        if (disabled || page == -1)
            return $"<li class=\"page-item disabled\"><span class=\"page-link\">{text}</span></li>";
        var href = "?" + BuildQuery(Search, GenreIds, AuthorIds, SortBy, page);
        var activeClass = active ? " active" : "";
        return $"<li class=\"page-item{activeClass}\"><a class=\"page-link\" href=\"{href}\">{text}</a></li>";
    }

    private static string BuildQuery(string? search, List<int>? genreIds, List<int>? authorIds, string? sortBy, int page)
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
