using Music.bisLog.Dtos;

namespace Music.bisLog.Services;

public interface ISongService
{
    Task<CatalogDto> GetCatalogAsync(string? search, List<int>? genreIds, List<int>? authorIds, string sortBy, int page);
    Task<SongDetailDto?> GetDetailAsync(int id);
    Task<List<SongDto>> GetUserSongsAsync(int userId);
    Task<SongDto> CreateAsync(CreateSongDto dto);
    Task<SongDto> UpdateAsync(UpdateSongDto dto);
    Task DeleteAsync(DeleteSongDto dto);
    Task<DownloadResultDto> DownloadAsync(int id);

    Task<AdminSongListDto> GetAdminSongsAsync(string? search, int page, int pageSize);
    Task<SongEditDataDto?> GetEditDataAsync(int id, int currentUserId);
    Task<AdminSongEditDataDto?> GetAdminEditDataAsync(int id);
    Task<SongDto> AdminUpdateAsync(AdminUpdateSongDto dto);
    Task AdminDeleteAsync(int songId);
    Task<int> CountAsync();
}