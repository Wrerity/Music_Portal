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

    public AuthController(IAuthService authService, ITokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
        var user = await _authService.RegisterAsync(dto);
        return CreatedAtAction(nameof(Login), new { username = user.Username }, user);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);
        try
        {
            var result = await _authService.LoginAsync(dto);
            // Успешный вход — result.Success == true и User != null (исключения уже брошены для остальных случаев)
            var tokenData = _tokenService.CreateToken(result.User!);

            return Ok(new LoginResponseDto
            {
                Token = tokenData.Token,
                TokenType = "Bearer",
                ExpiresAt = tokenData.ExpiresAt,
                User = result.User!
            });
        }
        catch (Music.bisLog.Exceptions.UserNotFoundException ex)
        {
            return Unauthorized(ApiProblem(StatusCodes.Status401Unauthorized, "Не авторизован", ex.Message));
        }
        catch (Music.bisLog.Exceptions.InvalidCredentialsException ex)
        {
            return Unauthorized(ApiProblem(StatusCodes.Status401Unauthorized, "Не авторизован", ex.Message));
        }
        catch (Music.bisLog.Exceptions.UserNotApprovedException ex)
        {
            // 4. Неподтверждённый пользователь — 403 Forbidden (альтернативно 409 Conflict)
            return StatusCode(StatusCodes.Status403Forbidden, ApiProblem(StatusCodes.Status403Forbidden, "Доступ запрещён", ex.Message));
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        await _authService.ChangePasswordAsync(GetCurrentUserId(), dto.OldPassword, dto.NewPassword);
        return Ok();
    }

    [HttpPost("reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        await _authService.ResetPasswordAsync(dto.Username, dto.NewPassword);
        return Ok();
    }
}