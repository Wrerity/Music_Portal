using System.ComponentModel.DataAnnotations;

namespace Music.bisLog.Dtos;

public class AuthorDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Имя автора обязательно")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя автора от 2 до 100 символов")]
    public string Name { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Страна не более 50 символов")]
    public string? Country { get; set; }

    [StringLength(1000, ErrorMessage = "Описание не более 1000 символов")]
    public string? Description { get; set; }
    public int SongCount { get; set; }
}