namespace Music.bisLog.Dtos;

public class LoginResultDto
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public UserDto? User { get; set; }
}