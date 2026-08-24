using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels.Admin;

public class AdminUserEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Validation_Required_Username")]
    [MaxLength(100)]
    [Display(Name="Field_Username")]
    public string Username { get; set; } = string.Empty;

    [MaxLength(100)]
    [Display(Name="Field_Password")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Validation_Required_Role")]
    [Display(Name="Field_Role")]
    public string Role { get; set; } = "User";

    [Display(Name="Field_IsApproved")]
    public bool IsApproved { get; set; }
}