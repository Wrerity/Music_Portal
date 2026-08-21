namespace Music.API.Dtos;

public class ResetPasswordRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}