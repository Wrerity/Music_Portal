using AutoMapper;
using Microsoft.Extensions.Logging;
using Music.bisLog.Dtos;
using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.bisLog.Services;

public class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService(IUnitOfWork uow, IMapper mapper, ILogger<AuthorService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<AuthorDto>> GetAllAsync()
    {
        var authors = await _uow.Authors.GetAllAsync();
        var result = _mapper.Map<List<AuthorDto>>(authors);
        foreach (var author in result)
        {
            author.SongCount = await _uow.Authors.GetSongCountAsync(author.Id);
        }
        return result;
    }

    public async Task<List<AuthorDto>> GetAllLightAsync()
    {
        var authors = await _uow.Authors.GetAllAsync();
        return _mapper.Map<List<AuthorDto>>(authors);
    }

    public async Task<AuthorDto?> GetByIdAsync(int id)
    {
        var author = await _uow.Authors.GetByIdAsync(id);
        if (author == null) return null;

        var dto = _mapper.Map<AuthorDto>(author);
        dto.SongCount = await _uow.Authors.GetSongCountAsync(author.Id);
        return dto;
    }

    public async Task<OperationResult> CreateAsync(AuthorDto dto)
    {
        try
        {
            var author = new Author
            {
                Name = dto.Name,
                Country = dto.Country,
                Description = dto.Description
            };
            await _uow.Authors.AddAsync(author);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании автора {Name}", dto.Name);
            return OperationResult.Fail("Произошла внутренняя ошибка при создании автора.");
        }
    }

    public async Task<OperationResult> UpdateAsync(AuthorDto dto)
    {
        try
        {
            var author = await _uow.Authors.GetByIdAsync(dto.Id);
            if (author == null)
                return OperationResult.Fail("Автор не найден");

            author.Name = dto.Name;
            author.Country = dto.Country;
            author.Description = dto.Description;
            await _uow.Authors.UpdateAsync(author);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении автора {AuthorId}", dto.Id);
            return OperationResult.Fail("Произошла внутренняя ошибка при обновлении автора.");
        }
    }

    public async Task<OperationResult> DeleteAsync(int authorId)
    {
        try
        {
            var author = await _uow.Authors.GetByIdAsync(authorId);
            if (author == null)
                return OperationResult.Ok();

            await _uow.Authors.DeleteAsync(author);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении автора {AuthorId}", authorId);
            return OperationResult.Fail("Произошла внутренняя ошибка при удалении автора.");
        }
    }

    public async Task<int> CountAsync()
    {
        return await _uow.Authors.CountAsync();
    }
}