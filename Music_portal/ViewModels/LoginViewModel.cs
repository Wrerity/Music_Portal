using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Имя пользователя обязательно")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
