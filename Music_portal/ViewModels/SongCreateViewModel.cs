using System.ComponentModel.DataAnnotations;
using Music.bisLog.Dtos;
using Music_portal.Validation;

namespace Music_portal.ViewModels;

public class SongCreateViewModel
{
    [Required(ErrorMessage = "Название обязательно")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите хотя бы одного автора")]
    public List<int> AuthorIds { get; set; } = new();

    [Required(ErrorMessage = "Выберите хотя бы один жанр")]
    public List<int> GenreIds { get; set; } = new();

    [MaxLength(2000)]
    public string? Lyrics { get; set; }

    public int Duration { get; set; }

    [Required(ErrorMessage = "Загрузите аудиофайл")]
    [AllowedFileExtensions(".mp3", ".wav", ErrorMessage = "Разрешены только файлы .mp3 и .wav")]
    [MaxFileSize(20 * 1024 * 1024, ErrorMessage = "Максимальный размер файла — 20 МБ")]
    public IFormFile? AudioFile { get; set; }

    [MaxLength(200)]
    public string? NewAuthorName { get; set; }

    public List<AuthorDto> AllAuthors { get; set; } = new();
    public List<GenreDto> AllGenres { get; set; } = new();
}