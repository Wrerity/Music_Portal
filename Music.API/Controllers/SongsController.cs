using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.API.Dtos;
using Music.bisLog.Dtos;
using Music.bisLog.Services;
using Music.DataAccess.Utils;

namespace Music.API.Controllers;

[Route("api/[controller]")]
public class SongsController : ApiControllerBase
{
    private readonly ISongService _songService;

    public SongsController(ISongService songService)
    {
        _songService = songService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCatalog(
        [FromQuery] string? search,
        [FromQuery] List<int>? genreIds,
        [FromQuery] string sortBy = SongSortKeys.Date,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15)
    {
        if (page < 1) page = 1;

        var catalog = await _songService.GetCatalogAsync(search, genreIds, sortBy, page);
        return Ok(catalog);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMySongs()
    {
        return Ok(await _songService.GetUserSongsAsync(GetCurrentUserId()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var song = await _songService.GetDetailAsync(id);
        if (song == null)
            return NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", "Песня не найдена"));

        return Ok(song);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromForm] CreateSongForm form)
    {
        var fileError = ValidateAudioFile(form.AudioFile, required: true);
        if (fileError != null) return fileError;

        var dto = new CreateSongDto
        {
            Title = form.Title,
            Duration = form.Duration,
            Lyrics = form.Lyrics,
            UserId = GetCurrentUserId(),
            AuthorIds = form.AuthorIds?.ToArray() ?? Array.Empty<int>(),
            GenreIds = form.GenreIds?.ToArray() ?? Array.Empty<int>(),
            NewAuthorName = form.NewAuthorName,
            AudioStream = form.AudioFile!.OpenReadStream(),
            AudioFileName = form.AudioFile.FileName
        };

        var result = await _songService.CreateAsync(dto);
        return FromResult(result);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateSongForm form)
    {
        var fileError = ValidateAudioFile(form.AudioFile, required: false);
        if (fileError != null) return fileError;

        if (User.IsInRole(RoleNames.Admin))
        {
            var adminUserId = form.UserId ?? (await ResolveSongUserIdAsync(id));
            if (adminUserId == null)
                return NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", "Песня не найдена"));

            var adminDto = new AdminUpdateSongDto
            {
                Id = id,
                Title = form.Title,
                Duration = form.Duration,
                Lyrics = form.Lyrics,
                UserId = adminUserId.Value,
                AuthorIds = form.AuthorIds?.ToArray() ?? Array.Empty<int>(),
                GenreIds = form.GenreIds?.ToArray() ?? Array.Empty<int>(),
                NewAuthorName = form.NewAuthorName,
                AudioStream = form.AudioFile?.OpenReadStream(),
                AudioFileName = form.AudioFile?.FileName
            };

            return FromResult(await _songService.AdminUpdateAsync(adminDto));
        }

        var dto = new UpdateSongDto
        {
            Id = id,
            Title = form.Title,
            Duration = form.Duration,
            Lyrics = form.Lyrics,
            UserId = GetCurrentUserId(),
            AuthorIds = form.AuthorIds?.ToArray() ?? Array.Empty<int>(),
            GenreIds = form.GenreIds?.ToArray() ?? Array.Empty<int>(),
            NewAuthorName = form.NewAuthorName,
            AudioStream = form.AudioFile?.OpenReadStream(),
            AudioFileName = form.AudioFile?.FileName
        };

        return FromResult(await _songService.UpdateAsync(dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        if (User.IsInRole(RoleNames.Admin))
            return FromResult(await _songService.AdminDeleteAsync(id));

        var result = await _songService.DeleteAsync(new DeleteSongDto
        {
            SongId = id,
            UserId = GetCurrentUserId()
        });

        return FromResult(result);
    }

    [HttpGet("{id:int}/download")]
    [Authorize]
    public async Task<IActionResult> Download(int id)
    {
        var result = await _songService.DownloadAsync(id);
        if (!result.Success || result.FilePath == null)
            return NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", "Файл песни не найден"));

        var fileName = Path.GetFileName(result.FilePath);
        return PhysicalFile(result.FilePath, "application/octet-stream", fileName);
    }

    [HttpGet("{id:int}/stream")]
    public async Task<IActionResult> Stream(int id)
    {
        var result = await _songService.DownloadAsync(id);
        if (!result.Success || result.FilePath == null)
            return NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", "Файл песни не найден"));

        var ext = Path.GetExtension(result.FilePath).ToLowerInvariant();
        var mime = ext switch
        {
            FileExtensions.Mp3 => "audio/mpeg",
            FileExtensions.Wav => "audio/wav",
            _ => "application/octet-stream"
        };

        return PhysicalFile(result.FilePath, mime, enableRangeProcessing: true);
    }

    private async Task<int?> ResolveSongUserIdAsync(int songId)
    {
        var data = await _songService.GetAdminEditDataAsync(songId);
        return data?.UserId;
    }
}