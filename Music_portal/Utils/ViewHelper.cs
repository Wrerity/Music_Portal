namespace Music_portal.Utils;

public static class ViewHelper
{
    public static string TruncateGenres(string genres)
    {
        if (string.IsNullOrEmpty(genres)) return "";
        var parts = genres.Split(',');
        if (parts.Length <= 2) return genres;
        return string.Join(",", parts.Take(2)) + ",...";
    }

    public static string GenreQuery(List<int> ids)
    {
        if (ids.Count == 0) return "";
        return "&" + string.Join("&", ids.Select(id => $"genreIds={id}"));
    }

    public static string AuthorQuery(List<int> ids)
    {
        if (ids.Count == 0) return "";
        return "&" + string.Join("&", ids.Select(id => $"authorIds={id}"));
    }

    public static string FilterQuery(List<int> genreIds, List<int> authorIds)
    {
        return GenreQuery(genreIds) + AuthorQuery(authorIds);
    }
}