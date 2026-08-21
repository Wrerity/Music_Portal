using AutoMapper;
using Microsoft.Extensions.Logging;
using Music.bisLog.Dtos;
using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.bisLog.Services;

public class GenreService : IGenreService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<GenreService> _logger;

    public GenreService(IUnitOfWork uow, IMapper mapper, ILogger<GenreService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<GenreDto>> GetAllAsync()
    {
        var genres = await _uow.Genres.GetAllAsync();
        var result = _mapper.Map<List<GenreDto>>(genres);
        foreach (var genre in result)
        {
            genre.SongCount = await _uow.Genres.GetSongCountAsync(genre.Id);
        }
        return result;
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

    public async Task<OperationResult> CreateAsync(GenreDto dto)
    {
        try
        {
            var genre = new Genre
            {
                Name = dto.Name,
                Description = dto.Description
            };
            await _uow.Genres.AddAsync(genre);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании жанра {Name}", dto.Name);
            return OperationResult.Fail("Произошла внутренняя ошибка при создании жанра.");
        }
    }

    public async Task<OperationResult> UpdateAsync(GenreDto dto)
    {
        try
        {
            var genre = await _uow.Genres.GetByIdAsync(dto.Id);
            if (genre == null)
                return OperationResult.Fail("Жанр не найден");

            genre.Name = dto.Name;
            genre.Description = dto.Description;
            await _uow.Genres.UpdateAsync(genre);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении жанра {GenreId}", dto.Id);
            return OperationResult.Fail("Произошла внутренняя ошибка при обновлении жанра.");
        }
    }

    public async Task<OperationResult> DeleteAsync(int genreId)
    {
        try
        {
            var genre = await _uow.Genres.GetByIdAsync(genreId);
            if (genre == null)
                return OperationResult.Ok();

            await _uow.Genres.DeleteAsync(genre);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении жанра {GenreId}", genreId);
            return OperationResult.Fail("Произошла внутренняя ошибка при удалении жанра.");
        }
    }

    public async Task<int> CountAsync()
    {
        return await _uow.Genres.CountAsync();
    }
}