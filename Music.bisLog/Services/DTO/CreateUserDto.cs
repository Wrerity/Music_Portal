using System.ComponentModel.DataAnnotations;
using Music.DataAccess.Utils;

namespace Music.bisLog.Dtos;

public class CreateUserDto
{
    [Required(ErrorMessage = "Имя пользователя обязательно")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Имя пользователя от 3 до 50 символов")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль минимум 6 символов")]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = RoleNames.User;
    public bool IsApproved { get; set; }
}