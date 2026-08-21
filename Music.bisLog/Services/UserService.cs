using AutoMapper;
using Microsoft.Extensions.Logging;
using Music.bisLog.Dtos;
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
        try
        {
            var user = await _uow.Users.GetByIdWithRolesAsync(userId);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении пользователя {UserId}", userId);
            return null;
        }
    }

    public async Task<UserListDto> GetUsersAsync(string? search, int page, int pageSize)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка пользователей");
            return new UserListDto { Search = search, Page = page, PageSize = pageSize };
        }
    }

    public async Task<List<UserDto>> GetPendingAsync()
    {
        try
        {
            var users = await _uow.Users.GetByApprovalStatusAsync(false);
            return _mapper.Map<List<UserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка заявок на регистрацию");
            return new List<UserDto>();
        }
    }

    public async Task<int> CountAsync()
    {
        return await _uow.Users.CountAsync();
    }

    public async Task<int> CountPendingAsync()
    {
        return await _uow.Users.CountByApprovalStatusAsync(false);
    }

    public async Task<OperationResult> CreateAsync(CreateUserDto dto)
    {
        try
        {
            if (await _uow.Users.GetByUsernameAsync(dto.Username) != null)
                return OperationResult.Fail("Пользователь с таким именем уже существует");

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

            if (role != null)
                user.Roles.Add(role);

            await _uow.Users.AddAsync(user);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании пользователя {Username}", dto.Username);
            return OperationResult.Fail("Произошла внутренняя ошибка при создании пользователя.");
        }
    }

    public async Task<OperationResult> UpdateAsync(UpdateUserDto dto)
    {
        try
        {
            var user = await _uow.Users.GetByIdWithRolesAsync(dto.Id);
            if (user == null)
                return OperationResult.Fail("Пользователь не найден");

            var byName = await _uow.Users.GetByUsernameAsync(dto.Username);
            if (byName != null && byName.Id != dto.Id)
                return OperationResult.Fail("Пользователь с таким именем уже существует");

            user.Username = dto.Username;
            user.IsApproved = dto.IsApproved;

            var selectedRole = await _uow.Roles.GetByNameAsync(dto.Role);
            if (selectedRole != null)
            {
                user.Roles.Clear();
                user.Roles.Add(selectedRole);
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var (hash, salt) = _hasher.Hash(dto.Password);
                user.PasswordHash = hash;
                user.Salt = salt;
            }

            await _uow.Users.UpdateAsync(user);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении пользователя {UserId}", dto.Id);
            return OperationResult.Fail("Произошла внутренняя ошибка при обновлении пользователя.");
        }
    }

    public async Task<OperationResult> DeleteAsync(int userId)
    {
        try
        {
            var user = await _uow.Users.GetByIdWithRolesAsync(userId);
            if (user == null)
                return OperationResult.Ok();

            if (user.Roles.Any(r => r.Name == RoleNames.Admin))
                return OperationResult.Fail("Нельзя удалить администратора");

            var userSongs = await _uow.Songs.GetByUserIdAsync(userId);
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.UploadFolder);
            foreach (var song in userSongs)
            {
                var filePath = Path.Combine(uploadsDir, song.FilePath);
                if (File.Exists(filePath)) File.Delete(filePath);
            }

            await _uow.Users.DeleteAsync(user);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении пользователя {UserId}", userId);
            return OperationResult.Fail("Произошла внутренняя ошибка при удалении пользователя.");
        }
    }

    public async Task<OperationResult> ActivateUserAsync(ActivateUserDto dto)
    {
        try
        {
            var user = await _uow.Users.GetByIdAsync(dto.UserId);
            if (user == null)
                return OperationResult.Fail("Пользователь не найден");

            user.IsApproved = true;
            await _uow.Users.UpdateAsync(user);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при активации пользователя {UserId}", dto.UserId);
            return OperationResult.Fail("Произошла внутренняя ошибка при активации пользователя.");
        }
    }

    public async Task<OperationResult> RejectUserAsync(int userId)
    {
        try
        {
            var user = await _uow.Users.GetByIdAsync(userId);
            if (user == null)
                return OperationResult.Ok();

            await _uow.Users.DeleteAsync(user);
            return OperationResult.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отклонении заявки {UserId}", userId);
            return OperationResult.Fail("Произошла внутренняя ошибка при отклонении заявки.");
        }
    }
}