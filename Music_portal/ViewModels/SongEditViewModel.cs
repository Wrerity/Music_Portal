using System.ComponentModel.DataAnnotations;
using Music.bisLog.Dtos;
using Music_portal.Validation;

namespace Music_portal.ViewModels;

public class SongEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Validation_Required_Title")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Validation_Required_AuthorIds")]
    public List<int> AuthorIds { get; set; } = new();

    [Required(ErrorMessage = "Validation_Required_GenreIds")]
    public List<int> GenreIds { get; set; } = new();

    [MaxLength(2000)]
    public string? Lyrics { get; set; }

    public int Duration { get; set; }

    [AllowedFileExtensions(".mp3", ".wav", ErrorMessage = "Validation_AllowedExtensions")]
    [MaxFileSize(20 * 1024 * 1024, ErrorMessage = "Validation_MaxFileSize")]
    public IFormFile? AudioFile { get; set; }

    [MaxLength(200)]
    public string? NewAuthorName { get; set; }

    public List<AuthorDto> AllAuthors { get; set; } = new();
    public List<GenreDto> AllGenres { get; set; } = new();
}