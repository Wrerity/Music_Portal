using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Music.API.Auth;
using Music.API.Dtos;
using Music.bisLog.Dtos;
using Music.bisLog.Services;
using Music.DataAccess.Data;

namespace Music.API.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _uow;
    private readonly PasswordHasher _hasher;

    public AuthController(IAuthService authService, ITokenService tokenService, IUnitOfWork uow, PasswordHasher hasher)
    {
        _authService = authService;
        _tokenService = tokenService;
        _uow = uow;
        _hasher = hasher;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(ApiProblem(StatusCodes.Status400BadRequest, "Ошибка запроса", "Имя пользователя и пароль обязательны"));

        var result = await _authService.RegisterAsync(dto);
        return FromResult(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(ApiProblem(StatusCodes.Status400BadRequest, "Ошибка запроса", "Имя пользователя и пароль обязательны"));

        var result = await _authService.LoginAsync(dto);
        if (!result.Success || result.User == null)
            return Unauthorized(ApiProblem(StatusCodes.Status401Unauthorized, "Не авторизован", result.Error));

        var tokenData = _tokenService.CreateToken(result.User);

        return Ok(new LoginResponseDto
        {
            Token = tokenData.Token,
            TokenType = "Bearer",
            ExpiresAt = tokenData.ExpiresAt,
            User = result.User
        });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.OldPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
            return BadRequest(ApiProblem(StatusCodes.Status400BadRequest, "Ошибка запроса", "Старый и новый пароль обязательны"));

        var userId = GetCurrentUserId();
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user == null)
            return NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", "Пользователь не найден"));

        if (!_hasher.Verify(dto.OldPassword, user.PasswordHash, user.Salt))
            return BadRequest(ApiProblem(StatusCodes.Status400BadRequest, "Ошибка запроса", "Неверный текущий пароль"));

        var (hash, salt) = _hasher.Hash(dto.NewPassword);
        user.PasswordHash = hash;
        user.Salt = salt;
        await _uow.Users.UpdateAsync(user);

        return Ok(OperationResult.Ok());
    }

    [HttpPost("reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.NewPassword))
            return BadRequest(ApiProblem(StatusCodes.Status400BadRequest, "Ошибка запроса", "Имя пользователя и новый пароль обязательны"));

        var user = await _uow.Users.GetByUsernameAsync(dto.Username);
        if (user == null)
            return NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", "Пользователь не найден"));

        var (hash, salt) = _hasher.Hash(dto.NewPassword);
        user.PasswordHash = hash;
        user.Salt = salt;
        await _uow.Users.UpdateAsync(user);

        return Ok(OperationResult.Ok());
    }
}