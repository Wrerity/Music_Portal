using System.ComponentModel.DataAnnotations;

namespace Music_portal.ViewModels.Admin;

public class AdminGenreViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int SongCount { get; set; }
}