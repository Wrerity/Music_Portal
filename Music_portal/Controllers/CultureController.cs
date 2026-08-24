using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Music_portal.Controllers;

public class CultureController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string culture, string returnUrl = "/")
    {
        // Разрешённые культуры: ru (основной), uk / uk-UA
        var allowed = new[] { "ru", "uk", "uk-UA" };
        if (!allowed.Contains(culture))
            culture = "ru";

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                Path = "/"
            });

        if (Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult SetLanguageGet(string culture, string returnUrl = "/")
    {
        var allowed = new[] { "ru", "uk", "uk-UA" };
        if (!allowed.Contains(culture))
            culture = "ru";

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                Path = "/"
            });

        if (Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }
}
