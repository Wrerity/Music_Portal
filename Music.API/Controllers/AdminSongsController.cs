using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.API.Dtos;
using Music.bisLog.Dtos;
using Music.bisLog.Services;

namespace Music.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin/songs")]
public class AdminSongsController : ApiControllerBase
{
    private readonly ISongService _songService;

    public AdminSongsController(ISongService songService)
    {
        _songService = songService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSongs(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        return Ok(await _songService.GetAdminSongsAsync(search, page, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEditData(int id)
    {
        var data = await _songService.GetAdminEditDataAsync(id);
        if (data == null)
            return NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", "Песня не найдена"));

        return Ok(data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateSongForm form)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        var userId = form.UserId ?? (await _songService.GetAdminEditDataAsync(id))?.UserId;
        if (userId == null)
            return NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", "Песня не найдена"));

        var dto = new AdminUpdateSongDto
        {
            Id = id,
            Title = form.Title,
            Duration = form.Duration,
            Lyrics = form.Lyrics,
            UserId = userId.Value,
            AuthorIds = form.AuthorIds?.ToArray() ?? Array.Empty<int>(),
            GenreIds = form.GenreIds?.ToArray() ?? Array.Empty<int>(),
            NewAuthorName = form.NewAuthorName,
            AudioStream = form.AudioFile?.OpenReadStream(),
            AudioFileName = form.AudioFile?.FileName
        };

        var updated = await _songService.AdminUpdateAsync(dto);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _songService.AdminDeleteAsync(id);
        return NoContent();
    }
}