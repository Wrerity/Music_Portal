using System.ComponentModel.DataAnnotations;

namespace Music.bisLog.Dtos;

public class GenreDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название жанра обязательно")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Название жанра от 2 до 100 символов")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Описание не более 500 символов")]
    public string? Description { get; set; }
    public int SongCount { get; set; }
}