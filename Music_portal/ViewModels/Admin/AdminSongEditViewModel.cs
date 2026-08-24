using System.ComponentModel.DataAnnotations;
using Music.bisLog.Dtos;

namespace Music_portal.ViewModels.Admin;

public class AdminSongEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Validation_Required_Title")]
    [MaxLength(200)]
    [Display(Name="Field_Title")]
    public string Title { get; set; } = string.Empty;

    [Display(Name="Field_User")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Validation_Required_AuthorIds")]
    public List<int> AuthorIds { get; set; } = new();

    [Required(ErrorMessage = "Validation_Required_GenreIds")]
    public List<int> GenreIds { get; set; } = new();

    [MaxLength(2000)]
    [Display(Name="Field_Text")]
    public string? Lyrics { get; set; }

    [Display(Name="Field_DurationShort")]
    public int Duration { get; set; }

    [Display(Name="Field_ReplaceAudio")]
    public IFormFile? AudioFile { get; set; }

    [MaxLength(200)]
    [Display(Name="Field_NewAuthor_Alt")]
    public string? NewAuthorName { get; set; }

    public List<AuthorDto> AllAuthors { get; set; } = new();
    public List<GenreDto> AllGenres { get; set; } = new();
    public List<UserDto> AllUsers { get; set; } = new();
}