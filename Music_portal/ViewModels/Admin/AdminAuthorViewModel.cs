using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels.Admin;

public class AdminAuthorViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Имя обязательно")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Country { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int SongCount { get; set; }
}