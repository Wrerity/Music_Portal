using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels.Admin;

public class AdminUserEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Имя пользователя обязательно")]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Роль обязательна")]
    public string Role { get; set; } = "User";

    public bool IsApproved { get; set; }
}