using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Validation_Required_Password")]
    [MinLength(6, ErrorMessage = "Validation_Password_MinLength")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Validation_Required_ConfirmPassword")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Validation_Passwords_NotMatch")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Validation_Required_Username")]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;
}
