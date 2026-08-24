using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Validation_Required_Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Validation_Required_Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
