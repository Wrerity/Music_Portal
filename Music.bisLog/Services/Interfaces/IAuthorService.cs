using Music.bisLog.Dtos;

namespace Music.bisLog.Services;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAllAsync();
    Task<List<AuthorDto>> GetAllLightAsync();
    Task<AuthorDto?> GetByIdAsync(int id);
    Task<OperationResult> CreateAsync(AuthorDto dto);
    Task<OperationResult> UpdateAsync(AuthorDto dto);
    Task<OperationResult> DeleteAsync(int authorId);
    Task<int> CountAsync();
}