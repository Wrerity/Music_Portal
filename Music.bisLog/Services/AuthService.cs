using Microsoft.Extensions.Logging;
using Music.bisLog.Dtos;
using Music.DataAccess.Data;
using Music.DataAccess.Models;
using Music.DataAccess.Utils;

namespace Music.bisLog.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly PasswordHasher _hasher;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUnitOfWork uow, PasswordHasher hasher, ILogger<AuthService> logger)
    {
        _uow = uow;
        _hasher = hasher;
        _logger = logger;
    }

    public async Task<OperationResult> RegisterAsync(RegisterRequestDto dto)
    {
        try
        {
            if (await _uow.Users.GetByUsernameAsync(dto.Username) != null)
                return OperationResult.Fail("Пользователь с таким именем уже зарегистрирован");

            var (hash, salt) = _hasher.Hash(dto.Password);

            var user = new User
            {
                PasswordHash = hash,
                Salt = salt,
                Username = dto.Username,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Users.AddAsync(user);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при регистрации пользователя {Username}", dto.Username);
            return OperationResult.Fail("Произошла внутренняя ошибка. Попробуйте позже.");
        }
    }

    public async Task<LoginResultDto> LoginAsync(LoginRequestDto dto)
    {
        try
        {
            var user = await _uow.Users.GetByUsernameAsync(dto.Username);
            if (user == null)
                return new LoginResultDto { Error = "Неверное имя пользователя или пароль" };

            if (!user.IsApproved)
                return new LoginResultDto { Error = "Ваша учётная запись ещё не подтверждена администратором" };

            if (!_hasher.Verify(dto.Password, user.PasswordHash, user.Salt))
                return new LoginResultDto { Error = "Неверное имя пользователя или пароль" };

            return new LoginResultDto
            {
                Success = true,
                UserId = user.Id,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    IsApproved = user.IsApproved,
                    CreatedAt = user.CreatedAt,
                    Role = user.Roles.Select(r => r.Name).FirstOrDefault() ?? RoleNames.User
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при входе пользователя {Username}", dto.Username);
            return new LoginResultDto { Error = "Произошла внутренняя ошибка. Попробуйте позже." };
        }
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        try
        {
            return await _uow.Users.GetByUsernameAsync(username) != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке существования пользователя {Username}", username);
            return false;
        }
    }
}