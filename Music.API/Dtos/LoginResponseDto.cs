using Music.bisLog.Dtos;

namespace Music.API.Dtos;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
    public UserDto? User { get; set; }
}