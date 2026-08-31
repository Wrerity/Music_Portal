using AutoMapper;
using Music.bisLog.Dtos;
using Music.bisLog.Exceptions;
using Music.DataAccess.Data;
using Music.DataAccess.Models;
using Music.DataAccess.Utils;

namespace Music.bisLog.Services;

public class SongService : ISongService
{
    private const int CatalogPageSize = 15;
    private readonly IUnitOfWork _uow;
    private readonly IGenreService _genreService;
    private readonly IMapper _mapper;
    public SongService(IUnitOfWork uow, IGenreService genreService, IMapper mapper) { _uow = uow; _genreService = genreService; _mapper = mapper; }
    private string UploadsDirectory => Path.Combine(Directory.GetCurrentDirectory(), FilePaths.UploadFolder);
    public async Task<CatalogDto> GetCatalogAsync(string? search, List<int>? genreIds, List<int>? authorIds, string sortBy, int page)
    {
        var result = await _uow.Songs.GetFilteredAsync(search, genreIds, authorIds, sortBy, page, CatalogPageSize);
        return new CatalogDto
        {
            SearchTerm = search ?? "", SelectedGenreIds = genreIds ?? new List<int>(), SelectedAuthorIds = authorIds ?? new List<int>(),
            SortBy = sortBy, Page = result.PageNumber, PageSize = result.PageSize, TotalCount = result.TotalCount, TotalPages = result.TotalPages,
            Songs = _mapper.Map<List<SongDto>>(result.Items), AllGenres = await _genreService.GetAllLightAsync(), AllAuthors = await GetAuthorsLightAsync()
        };
    }
    public async Task<SongDetailDto?> GetDetailAsync(int id)
    {
        var song = await _uow.Songs.GetByIdWithDetailsAsync(id);
        return song == null ? null : _mapper.Map<SongDetailDto>(song);
    }
    public async Task<List<SongDto>> GetUserSongsAsync(int userId)
    {
        var songs = await _uow.Songs.GetByUserIdAsync(userId);
        return _mapper.Map<List<SongDto>>(songs);
    }
    public async Task<SongDto> CreateAsync(CreateSongDto dto)
    {
        var user = await _uow.Users.GetByIdAsync(dto.UserId);
        if (user == null || !user.IsApproved) throw new AccessDeniedException("Ваша учётная запись должна быть подтверждена администратором");
        var ext = Path.GetExtension(dto.AudioFileName).ToLowerInvariant();
        if (!FileExtensions.Allowed.Contains(ext)) throw new BusinessValidationException($"Разрешены только файлы: {string.Join(", ", FileExtensions.Allowed)}");
        if (dto.AudioStream.Length > FileLimits.MaxFileSize) throw new BusinessValidationException($"Максимальный размер файла — {FileLimits.MaxFileSize / (1024 * 1024)} МБ");
        var uploadsDir = UploadsDirectory; Directory.CreateDirectory(uploadsDir);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create)) await dto.AudioStream.CopyToAsync(stream);
        var song = new Song { Title = dto.Title, User = user, FilePath = fileName, Duration = dto.Duration, Lyrics = dto.Lyrics, PlayCount = 0, CreatedAt = DateTime.UtcNow };
        song.Authors = (await GetAuthorsByIdsAsync(dto.AuthorIds)).ToList();
        song.Genres = (await GetGenresByIdsAsync(dto.GenreIds)).ToList();
        if (!string.IsNullOrWhiteSpace(dto.NewAuthorName))
        {
            var author = new Author { Name = dto.NewAuthorName.Trim() };
            await _uow.Authors.AddAsync(author);
            song.Authors.Add(author);
        }
        await _uow.Songs.AddAsync(song);
        return _mapper.Map<SongDto>(song);
    }
    public async Task<SongDto> UpdateAsync(UpdateSongDto dto)
    {
        var song = await _uow.Songs.GetByIdWithDetailsAsync(dto.Id);
        if (song == null || song.User.Id != dto.UserId) throw new EntityNotFoundException("Песня не найдена");
        song.Title = dto.Title; song.Duration = dto.Duration; song.Lyrics = dto.Lyrics; song.UpdatedAt = DateTime.UtcNow;
        await ReplaceAuthorsAsync(song, dto.AuthorIds);
        await ReplaceGenresAsync(song, dto.GenreIds);
        if (!string.IsNullOrWhiteSpace(dto.NewAuthorName))
        {
            var author = new Author { Name = dto.NewAuthorName.Trim() };
            await _uow.Authors.AddAsync(author);
            song.Authors.Add(author);
        }
        if (dto.AudioStream != null && dto.AudioStream.Length > 0 && dto.AudioFileName != null)
        {
            var ext = Path.GetExtension(dto.AudioFileName).ToLowerInvariant();
            if (!FileExtensions.Allowed.Contains(ext)) throw new BusinessValidationException($"Разрешены только файлы: {string.Join(", ", FileExtensions.Allowed)}");
            if (dto.AudioStream.Length > FileLimits.MaxFileSize) throw new BusinessValidationException($"Максимальный размер файла — {FileLimits.MaxFileSize / (1024 * 1024)} МБ");
            var uploadsDir = UploadsDirectory;
            var oldPath = Path.Combine(uploadsDir, song.FilePath);
            if (File.Exists(oldPath)) File.Delete(oldPath);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var newPath = Path.Combine(uploadsDir, fileName);
            using (var stream = new FileStream(newPath, FileMode.Create)) await dto.AudioStream.CopyToAsync(stream);
            song.FilePath = fileName;
        }
        await _uow.Songs.UpdateAsync(song);
        return _mapper.Map<SongDto>(song);
    }
    public async Task DeleteAsync(DeleteSongDto dto)
    {
        var song = await _uow.Songs.GetByIdWithDetailsAsync(dto.SongId);
        if (song == null) throw new EntityNotFoundException("Песня не найдена");
        if (song.User.Id != dto.UserId) throw new AccessDeniedException("Нельзя удалить чужую песню");
        var filePath = Path.Combine(UploadsDirectory, song.FilePath);
        if (File.Exists(filePath)) File.Delete(filePath);
        await _uow.Songs.DeleteAsync(song);
    }
    public async Task<DownloadResultDto> DownloadAsync(int id)
    {
        var song = await _uow.Songs.GetByIdWithDetailsAsync(id);
        if (song == null) return new DownloadResultDto();
        song.PlayCount++; await _uow.Songs.UpdateAsync(song);
        var fullPath = Path.Combine(UploadsDirectory, song.FilePath);
        if (!File.Exists(fullPath)) return new DownloadResultDto();
        return new DownloadResultDto { Success = true, FilePath = fullPath };
    }
    public async Task<int> CountAsync() => await _uow.Songs.CountAsync();
    public async Task<AdminSongListDto> GetAdminSongsAsync(string? search, int page, int pageSize)
    {
        var result = await _uow.Songs.GetAllWithDetailsAsync(search, page, pageSize);
        return new AdminSongListDto { Search = search, Page = result.PageNumber, PageSize = result.PageSize, TotalCount = result.TotalCount, TotalPages = result.TotalPages, Songs = _mapper.Map<List<SongDto>>(result.Items) };
    }
    public async Task<SongEditDataDto?> GetEditDataAsync(int id, int currentUserId)
    {
        var song = await _uow.Songs.GetByIdWithDetailsAsync(id);
        if (song == null || song.User.Id != currentUserId) return null;
        return new SongEditDataDto { Id = song.Id, Title = song.Title, Duration = song.Duration, Lyrics = song.Lyrics, AuthorIds = song.Authors.Select(a => a.Id).ToList(), GenreIds = song.Genres.Select(g => g.Id).ToList(), AllAuthors = await GetAuthorsLightAsync(), AllGenres = await _genreService.GetAllLightAsync() };
    }
    public async Task<AdminSongEditDataDto?> GetAdminEditDataAsync(int id)
    {
        var song = await _uow.Songs.GetByIdWithDetailsAsync(id);
        if (song == null) return null;
        return new AdminSongEditDataDto { Id = song.Id, UserId = song.User.Id, Title = song.Title, Duration = song.Duration, Lyrics = song.Lyrics, AuthorIds = song.Authors.Select(a => a.Id).ToList(), GenreIds = song.Genres.Select(g => g.Id).ToList(), AllAuthors = await GetAuthorsLightAsync(), AllGenres = await _genreService.GetAllLightAsync(), AllUsers = _mapper.Map<List<UserDto>>(await _uow.Users.GetAllAsync()) };
    }
    public async Task<SongDto> AdminUpdateAsync(AdminUpdateSongDto dto)
    {
        var song = await _uow.Songs.GetByIdWithDetailsAsync(dto.Id);
        if (song == null) throw new EntityNotFoundException("Песня не найдена");
        var user = await _uow.Users.GetByIdAsync(dto.UserId);
        if (user == null) throw new EntityNotFoundException("Пользователь не найден");
        song.Title = dto.Title; song.User = user; song.Duration = dto.Duration; song.Lyrics = dto.Lyrics; song.UpdatedAt = DateTime.UtcNow;
        await ReplaceAuthorsAsync(song, dto.AuthorIds);
        await ReplaceGenresAsync(song, dto.GenreIds);
        if (!string.IsNullOrWhiteSpace(dto.NewAuthorName))
        {
            var author = new Author { Name = dto.NewAuthorName.Trim() };
            await _uow.Authors.AddAsync(author);
            song.Authors.Add(author);
        }
        if (dto.AudioStream != null && dto.AudioStream.Length > 0 && dto.AudioFileName != null)
        {
            var ext = Path.GetExtension(dto.AudioFileName).ToLowerInvariant();
            if (!FileExtensions.Allowed.Contains(ext)) throw new BusinessValidationException($"Разрешены только файлы: {string.Join(", ", FileExtensions.Allowed)}");
            if (dto.AudioStream.Length > FileLimits.MaxFileSize) throw new BusinessValidationException($"Максимальный размер файла — {FileLimits.MaxFileSize / (1024 * 1024)} МБ");
            var uploadsDir = UploadsDirectory;
            var oldPath = Path.Combine(uploadsDir, song.FilePath);
            if (File.Exists(oldPath)) File.Delete(oldPath);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var newPath = Path.Combine(uploadsDir, fileName);
            using (var stream = new FileStream(newPath, FileMode.Create)) await dto.AudioStream.CopyToAsync(stream);
            song.FilePath = fileName;
        }
        await _uow.Songs.UpdateAsync(song);
        return _mapper.Map<SongDto>(song);
    }
    public async Task AdminDeleteAsync(int songId)
    {
        var song = await _uow.Songs.GetByIdWithDetailsAsync(songId);
        if (song == null) throw new EntityNotFoundException("Песня не найдена");
        var filePath = Path.Combine(UploadsDirectory, song.FilePath);
        if (File.Exists(filePath)) File.Delete(filePath);
        await _uow.Songs.DeleteAsync(song);
    }
    private async Task<List<AuthorDto>> GetAuthorsLightAsync()
    {
        var authors = await _uow.Authors.GetAllAsync();
        return _mapper.Map<List<AuthorDto>>(authors);
    }
    private async Task<IEnumerable<Author>> GetAuthorsByIdsAsync(IEnumerable<int> ids)
    {
        var authors = await _uow.Authors.GetAllAsync();
        return authors.Where(a => ids.Contains(a.Id));
    }
    private async Task<IEnumerable<Genre>> GetGenresByIdsAsync(IEnumerable<int> ids)
    {
        var genres = await _uow.Genres.GetAllAsync();
        return genres.Where(g => ids.Contains(g.Id));
    }
    private async Task ReplaceAuthorsAsync(Song song, IEnumerable<int> authorIds)
    {
        song.Authors.Clear();
        var authors = await GetAuthorsByIdsAsync(authorIds);
        foreach (var author in authors) song.Authors.Add(author);
    }
    private async Task ReplaceGenresAsync(Song song, IEnumerable<int> genreIds)
    {
        song.Genres.Clear();
        var genres = await GetGenresByIdsAsync(genreIds);
        foreach (var genre in genres) song.Genres.Add(genre);
    }
}
