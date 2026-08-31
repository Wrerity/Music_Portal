using AutoMapper;
using Music.bisLog.Dtos;
using Music.bisLog.Exceptions;
using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.bisLog.Services;

public class GenreService : IGenreService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public GenreService(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<List<GenreDto>> GetAllAsync()
    {
        var genres = await _uow.Genres.GetAllAsync();
        var result = _mapper.Map<List<GenreDto>>(genres);
        foreach (var genre in result) genre.SongCount = await _uow.Genres.GetSongCountAsync(genre.Id);
        return result;
    }
    public async Task<List<GenreDto>> SearchAsync(string? search)
    {
        if (string.IsNullOrWhiteSpace(search)) return await GetAllAsync();
        var all = await GetAllAsync();
        return all.Where(g => g.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    public async Task<List<GenreDto>> GetAllLightAsync()
    {
        var genres = await _uow.Genres.GetAllAsync();
        return _mapper.Map<List<GenreDto>>(genres);
    }
    public async Task<GenreDto?> GetByIdAsync(int id)
    {
        var genre = await _uow.Genres.GetByIdAsync(id);
        if (genre == null) return null;
        var dto = _mapper.Map<GenreDto>(genre);
        dto.SongCount = await _uow.Genres.GetSongCountAsync(genre.Id);
        return dto;
    }
    public async Task<GenreDto> CreateAsync(GenreDto dto)
    {
        var exists = (await _uow.Genres.GetAllAsync()).Any(g => g.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));
        if (exists) throw new GenreAlreadyExistsException($"Жанр с именем '{dto.Name}' уже существует");
        var genre = new Genre { Name = dto.Name, Description = dto.Description };
        await _uow.Genres.AddAsync(genre);
        return _mapper.Map<GenreDto>(genre);
    }
    public async Task<GenreDto> UpdateAsync(GenreDto dto)
    {
        var genre = await _uow.Genres.GetByIdAsync(dto.Id);
        if (genre == null) throw new EntityNotFoundException("Жанр не найден");
        var dup = (await _uow.Genres.GetAllAsync()).Any(g => g.Id != dto.Id && g.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));
        if (dup) throw new GenreAlreadyExistsException($"Жанр с именем '{dto.Name}' уже существует");
        genre.Name = dto.Name; genre.Description = dto.Description;
        await _uow.Genres.UpdateAsync(genre);
        return _mapper.Map<GenreDto>(genre);
    }
    public async Task DeleteAsync(int genreId)
    {
        var genre = await _uow.Genres.GetByIdAsync(genreId);
        if (genre == null) throw new EntityNotFoundException("Жанр не найден");
        await _uow.Genres.DeleteAsync(genre);
    }
    public async Task<int> CountAsync() => await _uow.Genres.CountAsync();
}
