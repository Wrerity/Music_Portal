using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Music.bisLog.Dtos;
using Music.bisLog.Exceptions;
using Music.bisLog.Services;
using Music_portal.Resources;
using Music_portal.Utils;
using Music_portal.ViewModels;

namespace Music_portal.Controllers;

public class SongController : Controller
{
    private readonly ISongService _songService;
    private readonly IGenreService _genreService;
    private readonly IAuthorService _authorService;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public SongController(ISongService songService, IGenreService genreService, IAuthorService authorService, IStringLocalizer<SharedResource> localizer)
    {
        _songService = songService;
        _genreService = genreService;
        _authorService = authorService;
        _localizer = localizer;
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _songService.GetDetailAsync(id);
        if (dto == null) return NotFound();
        var model = new SongDetailViewModel
        {
            Id = dto.Id, UserId = dto.UserId, Title = dto.Title, Authors = dto.AuthorNames, Genres = dto.GenreNames,
            Duration = dto.Duration, DurationFormatted = dto.DurationFormatted, Lyrics = dto.Lyrics, UploadDate = dto.UploadDate, PlayCount = dto.PlayCount,
        };
        return View(model);
    }

    [HttpGet][Authorize]
    public async Task<IActionResult> Create()
    {
        return View(new SongCreateViewModel { AllAuthors = await _authorService.GetAllLightAsync(), AllGenres = await _genreService.GetAllLightAsync() });
    }

    [HttpPost][ValidateAntiForgeryToken][Authorize]
    public async Task<IActionResult> Create(SongCreateViewModel model)
    {
        model.AllAuthors = await _authorService.GetAllLightAsync();
        model.AllGenres = await _genreService.GetAllLightAsync();
        if (!ModelState.IsValid) return View(model);
        try
        {
            await _songService.CreateAsync(new CreateSongDto
            {
                Title = model.Title, Duration = model.Duration, Lyrics = model.Lyrics, UserId = User.GetUserId(),
                AuthorIds = model.AuthorIds?.ToArray() ?? Array.Empty<int>(), GenreIds = model.GenreIds?.ToArray() ?? Array.Empty<int>(),
                NewAuthorName = model.NewAuthorName, AudioStream = model.AudioFile!.OpenReadStream(), AudioFileName = model.AudioFile.FileName
            });
            return RedirectToAction("Index", "Home");
        }
        catch (BusinessException ex) { ModelState.AddModelError(string.Empty, ex.Message); return View(model); }
    }

    [HttpGet][Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = User.GetUserId();
        var data = await _songService.GetEditDataAsync(id, userId);
        if (data == null) return Forbid();
        var model = new SongEditViewModel { Id = data.Id, Title = data.Title, AuthorIds = data.AuthorIds, GenreIds = data.GenreIds, Duration = data.Duration, Lyrics = data.Lyrics, AllAuthors = data.AllAuthors, AllGenres = data.AllGenres };
        return View(model);
    }

    [HttpPost][ValidateAntiForgeryToken][Authorize]
    public async Task<IActionResult> Edit(SongEditViewModel model)
    {
        model.AllAuthors = await _authorService.GetAllLightAsync();
        model.AllGenres = await _genreService.GetAllLightAsync();
        if (!ModelState.IsValid) return View(model);
        try
        {
            await _songService.UpdateAsync(new UpdateSongDto
            {
                Id = model.Id, Title = model.Title, Duration = model.Duration, Lyrics = model.Lyrics, UserId = User.GetUserId(),
                AuthorIds = model.AuthorIds?.ToArray() ?? Array.Empty<int>(), GenreIds = model.GenreIds?.ToArray() ?? Array.Empty<int>(),
                NewAuthorName = model.NewAuthorName, AudioStream = model.AudioFile?.OpenReadStream(), AudioFileName = model.AudioFile?.FileName
            });
            return RedirectToAction("Details", new { id = model.Id });
        }
        catch (BusinessException ex) { ModelState.AddModelError(string.Empty, ex.Message); return View(model); }
    }

    [HttpPost][ValidateAntiForgeryToken][Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try { await _songService.DeleteAsync(new DeleteSongDto { SongId = id, UserId = User.GetUserId() }); }
        catch (BusinessException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction("MySongs");
    }

    [HttpGet][Authorize]
    public async Task<IActionResult> MySongs()
    {
        var userId = User.GetUserId();
        var songs = await _songService.GetUserSongsAsync(userId);
        var models = songs.Select(s => new SongDisplayViewModel { Id = s.Id, Title = s.Title, Authors = s.Authors, Genres = s.Genres, UploadDate = s.UploadDate, PlayCount = s.PlayCount, UploadedBy = s.UploadedBy }).ToList();
        return View(models);
    }

    [HttpGet][Authorize]
    public async Task<IActionResult> Download(int id)
    {
        var result = await _songService.DownloadAsync(id);
        if (!result.Success || result.FilePath == null) return NotFound();
        var fileName = Path.GetFileName(result.FilePath);
        return PhysicalFile(result.FilePath, "application/octet-stream", fileName);
    }

    [HttpGet]
    public async Task<IActionResult> Stream(int id)
    {
        var result = await _songService.DownloadAsync(id);
        if (!result.Success || result.FilePath == null) return NotFound();
        var ext = Path.GetExtension(result.FilePath).ToLowerInvariant();
        var mime = ext switch { ".mp3" => "audio/mpeg", ".wav" => "audio/wav", _ => "application/octet-stream" };
        return PhysicalFile(result.FilePath, mime, enableRangeProcessing: true);
    }
}
