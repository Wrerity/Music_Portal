using AutoMapper;
using Music.bisLog.Dtos;
using Music.bisLog.Exceptions;
using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.bisLog.Services;

public class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public AuthorService(IUnitOfWork uow, IMapper mapper) { _uow = uow; _mapper = mapper; }

    public async Task<List<AuthorDto>> GetAllAsync()
    {
        var authors = await _uow.Authors.GetAllAsync();
        var result = _mapper.Map<List<AuthorDto>>(authors);
        foreach (var a in result) a.SongCount = await _uow.Authors.GetSongCountAsync(a.Id);
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
    public async Task<AuthorDto> CreateAsync(AuthorDto dto)
    {
        var exists = (await _uow.Authors.GetAllAsync()).Any(a => a.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));
        if (exists) throw new AuthorAlreadyExistsException($"Автор с именем '{dto.Name}' уже существует");
        var author = new Author { Name = dto.Name, Country = dto.Country, Description = dto.Description };
        await _uow.Authors.AddAsync(author);
        return _mapper.Map<AuthorDto>(author);
    }
    public async Task<AuthorDto> UpdateAsync(AuthorDto dto)
    {
        var author = await _uow.Authors.GetByIdAsync(dto.Id);
        if (author == null) throw new EntityNotFoundException("Автор не найден");
        var dup = (await _uow.Authors.GetAllAsync()).Any(a => a.Id != dto.Id && a.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase));
        if (dup) throw new AuthorAlreadyExistsException($"Автор с именем '{dto.Name}' уже существует");
        author.Name = dto.Name; author.Country = dto.Country; author.Description = dto.Description;
        await _uow.Authors.UpdateAsync(author);
        return _mapper.Map<AuthorDto>(author);
    }
    public async Task DeleteAsync(int authorId)
    {
        var author = await _uow.Authors.GetByIdAsync(authorId);
        if (author == null) throw new EntityNotFoundException("Автор не найден");
        await _uow.Authors.DeleteAsync(author);
    }
    public async Task<int> CountAsync() => await _uow.Authors.CountAsync();
}
