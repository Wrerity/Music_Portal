using System.ComponentModel.DataAnnotations;
using Music.bisLog.Dtos;

namespace Music_portal.ViewModels.Admin;

public class AdminSongEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public int UserId { get; set; }

    [Required(ErrorMessage = "Выберите хотя бы одного автора")]
    public List<int> AuthorIds { get; set; } = new();

    [Required(ErrorMessage = "Выберите хотя бы один жанр")]
    public List<int> GenreIds { get; set; } = new();

    [MaxLength(2000)]
    public string? Lyrics { get; set; }

    public int Duration { get; set; }

    public IFormFile? AudioFile { get; set; }

    [MaxLength(200)]
    public string? NewAuthorName { get; set; }

    public List<AuthorDto> AllAuthors { get; set; } = new();
    public List<GenreDto> AllGenres { get; set; } = new();
    public List<UserDto> AllUsers { get; set; } = new();
}