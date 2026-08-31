using System.ComponentModel.DataAnnotations;

namespace Music.bisLog.Dtos;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Имя пользователя обязательно")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    public string Password { get; set; } = string.Empty;
}