using Music.bisLog.Dtos;

namespace Music.bisLog.Services;

public interface IGenreService
{
    Task<List<GenreDto>> GetAllAsync();
    Task<List<GenreDto>> SearchAsync(string? search);
    Task<List<GenreDto>> GetAllLightAsync();
    Task<GenreDto?> GetByIdAsync(int id);
    Task<GenreDto> CreateAsync(GenreDto dto);
    Task<GenreDto> UpdateAsync(GenreDto dto);
    Task DeleteAsync(int genreId);
    Task<int> CountAsync();
}