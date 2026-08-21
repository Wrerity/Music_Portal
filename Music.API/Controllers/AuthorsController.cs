using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.bisLog.Dtos;
using Music.bisLog.Services;

namespace Music.API.Controllers;

[Route("api/[controller]")]
public class AuthorsController : ApiControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _authorService.GetAllAsync());
    }

    [HttpGet("light")]
    public async Task<IActionResult> GetAllLight()
    {
        return Ok(await _authorService.GetAllLightAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var author = await _authorService.GetByIdAsync(id);
        if (author == null)
            return NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", "Автор не найден"));

        return Ok(author);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] AuthorDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(ApiProblem(StatusCodes.Status400BadRequest, "Ошибка запроса", "Имя автора обязательно"));

        return FromResult(await _authorService.CreateAsync(dto));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] AuthorDto dto)
    {
        dto.Id = id;
        return FromResult(await _authorService.UpdateAsync(dto));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        return FromResult(await _authorService.DeleteAsync(id));
    }
}