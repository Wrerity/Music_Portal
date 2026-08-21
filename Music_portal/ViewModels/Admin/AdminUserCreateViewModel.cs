using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels.Admin;

public class AdminUserCreateViewModel
{
    [Required(ErrorMessage = "Имя пользователя обязательно")]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Роль обязательна")]
    public string Role { get; set; } = "User";

    public bool IsApproved { get; set; } = true;
}