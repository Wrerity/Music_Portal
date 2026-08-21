using Music.bisLog.Dtos;

namespace Music.bisLog.Services;

public interface ISongService
{
    Task<CatalogDto> GetCatalogAsync(string? search, List<int>? genreIds, string sortBy, int page);
    Task<SongDetailDto?> GetDetailAsync(int id);
    Task<List<SongDto>> GetUserSongsAsync(int userId);
    Task<OperationResult> CreateAsync(CreateSongDto dto);
    Task<OperationResult> UpdateAsync(UpdateSongDto dto);
    Task<OperationResult> DeleteAsync(DeleteSongDto dto);
    Task<DownloadResultDto> DownloadAsync(int id);

    Task<AdminSongListDto> GetAdminSongsAsync(string? search, int page, int pageSize);
    Task<SongEditDataDto?> GetEditDataAsync(int id, int currentUserId);
    Task<AdminSongEditDataDto?> GetAdminEditDataAsync(int id);
    Task<OperationResult> AdminUpdateAsync(AdminUpdateSongDto dto);
    Task<OperationResult> AdminDeleteAsync(int songId);
    Task<int> CountAsync();
}