using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Music.bisLog.Dtos;
using Music.bisLog.Services;
using Music_portal.Resources;
using Music_portal.ViewModels;
using Music_portal.ViewModels.Admin;

namespace Music_portal.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUserService _userService;
    private readonly IGenreService _genreService;
    private readonly IAuthorService _authorService;
    private readonly ISongService _songService;
    private readonly ILogger<AdminController> _logger;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public AdminController(
        IUserService userService,
        IGenreService genreService,
        IAuthorService authorService,
        ISongService songService,
        ILogger<AdminController> logger,
        IStringLocalizer<SharedResource> localizer)
    {
        _userService = userService;
        _genreService = genreService;
        _authorService = authorService;
        _songService = songService;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<IActionResult> Index()
    {
        var stats = new DashboardStats
        {
            PendingCount = await _userService.CountPendingAsync(),
            TotalUsers = await _userService.CountAsync(),
            TotalSongs = await _songService.CountAsync(),
            TotalGenres = await _genreService.CountAsync(),
            TotalAuthors = await _authorService.CountAsync()
        };
        return View(stats);
    }

    // ============ Users ============

    public async Task<IActionResult> Users(string? search, int page = 1)
    {
        var result = await _userService.GetUsersAsync(search, page, 20);
        return View(new AdminUserListViewModel
        {
            Search = result.Search,
            Page = result.Page,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            Users = result.Users
        });
    }

    [HttpGet]
    public IActionResult UserCreate()
    {
        return View(new AdminUserCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserCreate(AdminUserCreateViewModel model)
    {
        var result = await _userService.CreateAsync(new CreateUserDto
        {
            Username = model.Username,
            Password = model.Password,
            Role = model.Role,
            IsApproved = model.IsApproved
        });

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error);
            return View(model);
        }

        TempData["Success"] = _localizer["Msg_UserCreated"];
        return RedirectToAction("Users");
    }

    [HttpGet]
    public async Task<IActionResult> UserEdit(int id)
    {
        var dto = await _userService.GetUserAsync(id);
        if (dto == null) return NotFound();

        return View(new AdminUserEditViewModel
        {
            Id = dto.Id,
            Username = dto.Username,
            Role = dto.Role,
            IsApproved = dto.IsApproved
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserEdit(AdminUserEditViewModel model)
    {
        var result = await _userService.UpdateAsync(new UpdateUserDto
        {
            Id = model.Id,
            Username = model.Username,
            Password = model.Password,
            Role = model.Role,
            IsApproved = model.IsApproved
        });

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error);
            return View(model);
        }

        TempData["Success"] = _localizer["Msg_UserUpdated"];
        return RedirectToAction("Users");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserDelete(int id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result.Success)
            TempData["Error"] = result.Error;
        else
            TempData["Success"] = _localizer["Msg_UserDeleted"];
        return RedirectToAction("Users");
    }

    // ============ Registration Requests ============

    public async Task<IActionResult> PendingRequests()
    {
        var users = await _userService.GetPendingAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        await _userService.ActivateUserAsync(new ActivateUserDto { UserId = id });
        return RedirectToAction("PendingRequests");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        await _userService.RejectUserAsync(id);
        return RedirectToAction("PendingRequests");
    }

    // ============ Genres ============

    public async Task<IActionResult> Genres()
    {
        var genres = await _genreService.GetAllAsync();
        var models = genres.Select(g => new AdminGenreViewModel
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            SongCount = g.SongCount
        }).ToList();
        return View(models);
    }

    [HttpGet]
    public IActionResult GenreCreate()
    {
        return View(new AdminGenreViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenreCreate(AdminGenreViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _genreService.CreateAsync(new GenreDto
        {
            Name = model.Name,
            Description = model.Description
        });

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error);
            return View(model);
        }

        TempData["Success"] = _localizer["Msg_GenreCreated"];
        return RedirectToAction("Genres");
    }

    [HttpGet]
    public async Task<IActionResult> GenreEdit(int id)
    {
        var dto = await _genreService.GetByIdAsync(id);
        if (dto == null) return NotFound();

        return View(new AdminGenreViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            SongCount = dto.SongCount
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenreEdit(AdminGenreViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _genreService.UpdateAsync(new GenreDto
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
        });

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error);
            return View(model);
        }

        TempData["Success"] = _localizer["Msg_GenreUpdated"];
        return RedirectToAction("Genres");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenreDelete(int id)
    {
        await _genreService.DeleteAsync(id);
        TempData["Success"] = _localizer["Msg_GenreDeleted"];
        return RedirectToAction("Genres");
    }

    // ============ Authors ============

    public async Task<IActionResult> Authors()
    {
        var authors = await _authorService.GetAllAsync();
        var models = authors.Select(a => new AdminAuthorViewModel
        {
            Id = a.Id,
            Name = a.Name,
            Country = a.Country,
            Description = a.Description,
            SongCount = a.SongCount
        }).ToList();
        return View(models);
    }

    [HttpGet]
    public IActionResult AuthorCreate()
    {
        return View(new AdminAuthorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AuthorCreate(AdminAuthorViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _authorService.CreateAsync(new AuthorDto
        {
            Name = model.Name,
            Country = model.Country,
            Description = model.Description
        });

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error);
            return View(model);
        }

        TempData["Success"] = _localizer["Msg_AuthorCreated"];
        return RedirectToAction("Authors");
    }

    [HttpGet]
    public async Task<IActionResult> AuthorEdit(int id)
    {
        var dto = await _authorService.GetByIdAsync(id);
        if (dto == null) return NotFound();

        return View(new AdminAuthorViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Country = dto.Country,
            Description = dto.Description,
            SongCount = dto.SongCount
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AuthorEdit(AdminAuthorViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _authorService.UpdateAsync(new AuthorDto
        {
            Id = model.Id,
            Name = model.Name,
            Country = model.Country,
            Description = model.Description
        });

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error);
            return View(model);
        }

        TempData["Success"] = _localizer["Msg_AuthorUpdated"];
        return RedirectToAction("Authors");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AuthorDelete(int id)
    {
        await _authorService.DeleteAsync(id);
        TempData["Success"] = _localizer["Msg_AuthorDeleted"];
        return RedirectToAction("Authors");
    }

    // ============ Songs ============

    public async Task<IActionResult> Songs(string? search, int page = 1)
    {
        var result = await _songService.GetAdminSongsAsync(search, page, 20);
        var viewModels = result.Songs.Select(s => new ViewModels.SongDisplayViewModel
        {
            Id = s.Id,
            Title = s.Title,
            Authors = s.Authors,
            Genres = s.Genres,
            UploadDate = s.UploadDate,
            PlayCount = s.PlayCount,
            UploadedBy = s.UploadedBy
        }).ToList();

        return View(new AdminSongListViewModel
        {
            Search = result.Search,
            Page = result.Page,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            Songs = viewModels
        });
    }

    [HttpGet]
    public async Task<IActionResult> SongEdit(int id)
    {
        var data = await _songService.GetAdminEditDataAsync(id);
        if (data == null) return NotFound();

        return View(new AdminSongEditViewModel
        {
            Id = data.Id,
            Title = data.Title,
            UserId = data.UserId,
            AuthorIds = data.AuthorIds,
            GenreIds = data.GenreIds,
            Duration = data.Duration,
            Lyrics = data.Lyrics,
            AllAuthors = data.AllAuthors,
            AllGenres = data.AllGenres,
            AllUsers = data.AllUsers
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SongEdit(AdminSongEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var reloaded = await _songService.GetAdminEditDataAsync(model.Id);
            if (reloaded != null)
            {
                model.AllAuthors = reloaded.AllAuthors;
                model.AllGenres = reloaded.AllGenres;
                model.AllUsers = reloaded.AllUsers;
            }
            return View(model);
        }

        var result = await _songService.AdminUpdateAsync(new AdminUpdateSongDto
        {
            Id = model.Id,
            UserId = model.UserId,
            Title = model.Title,
            Duration = model.Duration,
            Lyrics = model.Lyrics,
            AuthorIds = model.AuthorIds?.ToArray() ?? Array.Empty<int>(),
            GenreIds = model.GenreIds?.ToArray() ?? Array.Empty<int>(),
            NewAuthorName = model.NewAuthorName,
            AudioStream = model.AudioFile?.OpenReadStream(),
            AudioFileName = model.AudioFile?.FileName
        });

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Error);
            var reloaded = await _songService.GetAdminEditDataAsync(model.Id);
            if (reloaded != null)
            {
                model.AllAuthors = reloaded.AllAuthors;
                model.AllGenres = reloaded.AllGenres;
                model.AllUsers = reloaded.AllUsers;
            }
            return View(model);
        }

        TempData["Success"] = _localizer["Msg_SongUpdated"];
        return RedirectToAction("Songs");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SongDelete(int id)
    {
        var result = await _songService.AdminDeleteAsync(id);
        if (!result.Success)
            TempData["Error"] = result.Error;
        else
            TempData["Success"] = _localizer["Msg_SongDeleted"];
        return RedirectToAction("Songs");
    }
}