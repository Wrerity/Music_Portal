using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.bisLog.Dtos;
using Music.bisLog.Services;

namespace Music.API.Controllers;

[Route("api/[controller]")]
public class GenresController : ApiControllerBase
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        // 6. Поиск по имени жанра
        if (!string.IsNullOrWhiteSpace(search))
            return Ok(await _genreService.SearchAsync(search));
        return Ok(await _genreService.GetAllAsync());
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? query)
    {
        return Ok(await _genreService.SearchAsync(query));
    }

    [HttpGet("light")]
    public async Task<IActionResult> GetAllLight()
    {
        return Ok(await _genreService.GetAllLightAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var genre = await _genreService.GetByIdAsync(id);
        if (genre == null)
            return NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", "Жанр не найден"));

        return Ok(genre);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] GenreDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
        var created = await _genreService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] GenreDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
        dto.Id = id;
        var updated = await _genreService.UpdateAsync(dto);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _genreService.DeleteAsync(id);
        return NoContent();
    }
}