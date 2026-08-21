using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.bisLog.Dtos;
using Music.bisLog.Services;

namespace Music.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        return Ok(await _userService.GetUsersAsync(search, page, pageSize));
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        return Ok(await _userService.GetPendingAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetUserAsync(id);
        if (user == null)
            return NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", "Пользователь не найден"));

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(ApiProblem(StatusCodes.Status400BadRequest, "Ошибка запроса", "Имя пользователя и пароль обязательны"));

        var result = await _userService.CreateAsync(dto);
        return FromResult(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        dto.Id = id;
        var result = await _userService.UpdateAsync(dto);
        return FromResult(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteAsync(id);
        return FromResult(result);
    }

    [HttpPost("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id)
    {
        var result = await _userService.ActivateUserAsync(new ActivateUserDto { UserId = id });
        return FromResult(result);
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var result = await _userService.RejectUserAsync(id);
        return FromResult(result);
    }
}