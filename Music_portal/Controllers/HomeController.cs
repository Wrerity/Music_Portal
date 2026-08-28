using Microsoft.AspNetCore.Mvc;
using Music.bisLog.Dtos;
using Music.bisLog.Services;
using Music_portal.Models;
using Music_portal.ViewModels;
using System.Diagnostics;

namespace Music_portal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISongService _songService;

    public HomeController(ILogger<HomeController> logger, ISongService songService)
    {
        _logger = logger;
        _songService = songService;
    }

    public async Task<IActionResult> Index(string? search, List<int>? genreIds, List<int>? authorIds, string sortBy = "date", int page = 1)
    {
        // Модель: сортировка / фильтрация / пагинация через EF в SongService.GetCatalogAsync
        var catalog = await _songService.GetCatalogAsync(search, genreIds, authorIds, sortBy, page);

        var model = new CatalogViewModel
        {
            SearchTerm = catalog.SearchTerm,
            SelectedGenreIds = catalog.SelectedGenreIds,
            SelectedAuthorIds = catalog.SelectedAuthorIds,
            SortBy = catalog.SortBy,
            Page = catalog.Page,
            TotalCount = catalog.TotalCount,
            PageSize = catalog.PageSize,
            TotalPages = catalog.TotalPages,
            Songs = catalog.Songs.Select(s => new SongDisplayViewModel
            {
                Id = s.Id,
                Title = s.Title,
                Authors = s.Authors,
                Genres = s.Genres,
                UploadDate = s.UploadDate,
                PlayCount = s.PlayCount,
                UploadedBy = s.UploadedBy
            }).ToList(),
            AllGenres = catalog.AllGenres,
            AllAuthors = catalog.AllAuthors
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult NotFoundPage()
    {
        Response.StatusCode = 404;
        return View();
    }

    public IActionResult AccessDenied()
    {
        Response.StatusCode = 403;
        return View();
    }
}
