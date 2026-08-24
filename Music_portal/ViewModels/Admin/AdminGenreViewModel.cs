using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels.Admin;

public class AdminGenreViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Validation_Required_Title")]
    [MaxLength(100)]
    [Display(Name="Field_Title")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name="Field_Description")]
    public string? Description { get; set; }

    public int SongCount { get; set; }
}