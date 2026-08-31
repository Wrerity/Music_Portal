using System.ComponentModel.DataAnnotations;

namespace Music.bisLog.Dtos;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Имя пользователя обязательно")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Имя пользователя должно быть от 3 до 50 символов")]
    [RegularExpression(@"^[a-zA-Z0-9_\-]+$", ErrorMessage = "Имя пользователя может содержать только буквы, цифры, _ и -")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен быть минимум 6 символов")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "Пароль должен содержать буквы и цифры")]
    public string Password { get; set; } = string.Empty;
}