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
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _genreService.GetAllAsync());
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
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(ApiProblem(StatusCodes.Status400BadRequest, "Ошибка запроса", "Название жанра обязательно"));

        return FromResult(await _genreService.CreateAsync(dto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] GenreDto dto)
    {
        dto.Id = id;
        return FromResult(await _genreService.UpdateAsync(dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        return FromResult(await _genreService.DeleteAsync(id));
    }
}