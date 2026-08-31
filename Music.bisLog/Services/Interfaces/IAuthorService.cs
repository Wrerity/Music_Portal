using Music.bisLog.Dtos;

namespace Music.bisLog.Services;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAllAsync();
    Task<List<AuthorDto>> GetAllLightAsync();
    Task<AuthorDto?> GetByIdAsync(int id);
    Task<AuthorDto> CreateAsync(AuthorDto dto);
    Task<AuthorDto> UpdateAsync(AuthorDto dto);
    Task DeleteAsync(int authorId);
    Task<int> CountAsync();
}