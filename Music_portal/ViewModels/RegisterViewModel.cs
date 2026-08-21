using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтверждение пароля обязательно")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Имя пользователя обязательно")]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
}
