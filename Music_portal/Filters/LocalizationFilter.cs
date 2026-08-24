using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Music_portal.Filters;

/// <summary>
/// Фильтр локализации — устанавливает культуру для каждого запроса.
/// Поддерживаемые культуры: ru (основной, по умолчанию), uk / uk-UA.
/// Источники: ?culture= query, cookie .AspNetCore.Culture, Accept-Language (через middleware),
/// fallback — ru. Одно представление используется для всех культур, строки берутся из Resource.resx.
/// </summary>
public class LocalizationFilter : IAsyncActionFilter
{
    private static readonly HashSet<string> Supported = new(StringComparer.OrdinalIgnoreCase)
    {
        "ru", "uk", "uk-UA"
    };

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var http = context.HttpContext;

        // 1. ?culture=uk / ?culture=ru — явное переключение (приоритет выше cookie)
        string? rawCulture = http.Request.Query["culture"].FirstOrDefault();

        // 2. cookie .AspNetCore.Culture вида c=uk|uic=uk
        if (string.IsNullOrWhiteSpace(rawCulture) && http.Request.Cookies.TryGetValue(CookieRequestCultureProvider.DefaultCookieName, out var cookieVal))
        {
            var parsed = TryParseCookie(cookieVal);
            if (parsed != null) rawCulture = parsed;
        }

        // 3. Валидация
        var cultureName = NormalizeCulture(rawCulture);

        try
        {
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
        catch (CultureNotFoundException)
        {
            var fallback = new CultureInfo("ru");
            CultureInfo.CurrentCulture = fallback;
            CultureInfo.CurrentUICulture = fallback;
        }

        // Для отладки можно посмотреть текущую культуру в View: CultureInfo.CurrentUICulture.Name
        await next();
    }

    private static string NormalizeCulture(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "ru";
        raw = raw.Trim();
        // cookie может содержать uk-UA, query может быть uk
        if (Supported.Contains(raw)) return raw.Equals("uk-UA", StringComparison.OrdinalIgnoreCase) ? "uk-UA" : raw.ToLowerInvariant();
        // попытка сократить uk-UA -> uk
        if (raw.StartsWith("uk", StringComparison.OrdinalIgnoreCase)) return "uk";
        if (raw.StartsWith("ru", StringComparison.OrdinalIgnoreCase)) return "ru";
        return "ru";
    }

    private static string? TryParseCookie(string cookieValue)
    {
        // Формат: c=uk|uic=uk  или c=ru|uic=ru
        // CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("uk")) => c=uk|uic=uk
        try
        {
            var parts = cookieValue.Split('|', StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts)
            {
                var kv = p.Split('=', 2);
                if (kv.Length == 2 && kv[0].Trim().Equals("c", StringComparison.OrdinalIgnoreCase))
                    return kv[1].Trim();
            }
            // fallback: если просто "uk"
            if (Supported.Contains(cookieValue.Trim())) return cookieValue.Trim();
        }
        catch { }
        return null;
    }
}
