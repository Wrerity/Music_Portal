using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels.Admin;

public class AdminAuthorViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Validation_Required_Name")]
    [MaxLength(200)]
    [Display(Name="Field_Title")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    [Display(Name="Field_Country")]
    public string? Country { get; set; }

    [MaxLength(1000)]
    [Display(Name="Field_Description")]
    public string? Description { get; set; }

    public int SongCount { get; set; }
}