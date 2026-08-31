using AutoMapper;
using Microsoft.Extensions.Logging;
using Music.bisLog.Dtos;
using Music.bisLog.Exceptions;
using Music.DataAccess.Data;
using Music.DataAccess.Models;
using Music.DataAccess.Utils;

namespace Music.bisLog.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly PasswordHasher _hasher;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork uow, PasswordHasher hasher, IMapper mapper, ILogger<UserService> logger)
    {
        _uow = uow;
        _hasher = hasher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserAsync(int userId)
    {
        var user = await _uow.Users.GetByIdWithRolesAsync(userId);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserListDto> GetUsersAsync(string? search, int page, int pageSize)
    {
        var result = await _uow.Users.GetPagedAsync(search, page, pageSize);
        return new UserListDto
        {
            Search = search,
            Page = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = result.TotalPages,
            Users = _mapper.Map<List<UserDto>>(result.Items)
        };
    }

    public async Task<List<UserDto>> GetPendingAsync()
    {
        var users = await _uow.Users.GetByApprovalStatusAsync(false);
        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<int> CountAsync() => await _uow.Users.CountAsync();
    public async Task<int> CountPendingAsync() => await _uow.Users.CountByApprovalStatusAsync(false);

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        if (await _uow.Users.GetByUsernameAsync(dto.Username) != null)
            throw new UserAlreadyExistsException($"Пользователь с именем '{dto.Username}' уже существует");

        var (hash, salt) = _hasher.Hash(dto.Password);
        var role = await _uow.Roles.GetByNameAsync(dto.Role);
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = hash,
            Salt = salt,
            IsApproved = dto.IsApproved,
            CreatedAt = DateTime.UtcNow
        };
        if (role != null) user.Roles.Add(role);
        await _uow.Users.AddAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateAsync(UpdateUserDto dto)
    {
        var user = await _uow.Users.GetByIdWithRolesAsync(dto.Id);
        if (user == null) throw new EntityNotFoundException("Пользователь не найден");
        var byName = await _uow.Users.GetByUsernameAsync(dto.Username);
        if (byName != null && byName.Id != dto.Id) throw new UserAlreadyExistsException($"Пользователь с именем '{dto.Username}' уже существует");
        user.Username = dto.Username;
        user.IsApproved = dto.IsApproved;
        var selectedRole = await _uow.Roles.GetByNameAsync(dto.Role);
        if (selectedRole != null) { user.Roles.Clear(); user.Roles.Add(selectedRole); }
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var (hash, salt) = _hasher.Hash(dto.Password);
            user.PasswordHash = hash; user.Salt = salt;
        }
        await _uow.Users.UpdateAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task DeleteAsync(int userId)
    {
        var user = await _uow.Users.GetByIdWithRolesAsync(userId);
        if (user == null) throw new EntityNotFoundException("Пользователь не найден");
        if (user.Roles.Any(r => r.Name == RoleNames.Admin)) throw new AccessDeniedException("Нельзя удалить администратора");
        var userSongs = await _uow.Songs.GetByUserIdAsync(userId);
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.UploadFolder);
        foreach (var song in userSongs)
        {
            var filePath = Path.Combine(uploadsDir, song.FilePath);
            if (File.Exists(filePath)) File.Delete(filePath);
        }
        await _uow.Users.DeleteAsync(user);
    }

    public async Task ActivateUserAsync(ActivateUserDto dto)
    {
        var user = await _uow.Users.GetByIdAsync(dto.UserId);
        if (user == null) throw new EntityNotFoundException("Пользователь не найден");
        user.IsApproved = true;
        await _uow.Users.UpdateAsync(user);
    }

    public async Task RejectUserAsync(int userId)
    {
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user == null) throw new EntityNotFoundException("Пользователь не найден");
        await _uow.Users.DeleteAsync(user);
    }
}
