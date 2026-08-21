using Music.bisLog.Dtos;

namespace Music.bisLog.Services;

public interface IAuthService
{
    Task<OperationResult> RegisterAsync(RegisterRequestDto dto);
    Task<LoginResultDto> LoginAsync(LoginRequestDto dto);
    Task<bool> UsernameExistsAsync(string username);
}