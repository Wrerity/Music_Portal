using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels.Admin;

public class AdminUserCreateViewModel
{
    [Required(ErrorMessage = "Validation_Required_Username")]
    [MaxLength(100)]
    [Display(Name="Field_Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Validation_Required_Password")]
    [MinLength(6, ErrorMessage = "Validation_Password_MinLength")]
    [Display(Name="Field_Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Validation_Required_Role")]
    [Display(Name="Field_Role")]
    public string Role { get; set; } = "User";

    [Display(Name="Field_IsApproved")]
    public bool IsApproved { get; set; } = true;
}