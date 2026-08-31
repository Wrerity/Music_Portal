using Music.bisLog.Dtos;

namespace Music.bisLog.Services;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterRequestDto dto);
    Task<LoginResultDto> LoginAsync(LoginRequestDto dto);
    Task<bool> UsernameExistsAsync(string username);
    Task ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    Task ResetPasswordAsync(string username, string newPassword);
}