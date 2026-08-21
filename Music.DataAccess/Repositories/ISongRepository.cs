using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.DataAccess.Repositories;

public interface ISongRepository : IRepository<Song>
{
    Task<PagedResult<Song>> GetFilteredAsync(string? search, List<int>? genreIds, string sortBy, int page, int pageSize);
    Task<PagedResult<Song>> GetAllWithDetailsAsync(string? search, int page, int pageSize);
    Task<Song?> GetByIdWithDetailsAsync(int id);
    Task<List<Song>> GetByUserIdAsync(int userId);
}
