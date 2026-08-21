using Music.bisLog.Dtos;

namespace Music.bisLog.Services;

public interface IGenreService
{
    Task<List<GenreDto>> GetAllAsync();
    Task<List<GenreDto>> GetAllLightAsync();
    Task<GenreDto?> GetByIdAsync(int id);
    Task<OperationResult> CreateAsync(GenreDto dto);
    Task<OperationResult> UpdateAsync(GenreDto dto);
    Task<OperationResult> DeleteAsync(int genreId);
    Task<int> CountAsync();
}