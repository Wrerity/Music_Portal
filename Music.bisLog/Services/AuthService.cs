using AutoMapper;
using Music.bisLog.Dtos;
using Music.bisLog.Exceptions;
using Music.DataAccess.Data;
using Music.DataAccess.Models;
using Music.DataAccess.Utils;

namespace Music.bisLog.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly PasswordHasher _hasher;
    private readonly IMapper _mapper;
    public AuthService(IUnitOfWork uow, PasswordHasher hasher, IMapper mapper)
    {
        _uow = uow;
        _hasher = hasher;
        _mapper = mapper;
    }

    public async Task<UserDto> RegisterAsync(RegisterRequestDto dto)
    {
        if (await _uow.Users.GetByUsernameAsync(dto.Username) != null)
            throw new UserAlreadyExistsException($"Пользователь с именем '{dto.Username}' уже существует");
        var (hash, salt) = _hasher.Hash(dto.Password);
        var user = new User
        {
            PasswordHash = hash,
            Salt = salt,
            Username = dto.Username,
            IsApproved = false,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Users.AddAsync(user);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<LoginResultDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _uow.Users.GetByUsernameAsync(dto.Username);
        if (user == null) throw new UserNotFoundException();
        if (!user.IsApproved) throw new UserNotApprovedException();
        if (!_hasher.Verify(dto.Password, user.PasswordHash, user.Salt)) throw new InvalidCredentialsException();
        return new LoginResultDto
        {
            Success = true,
            UserId = user.Id,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                IsApproved = user.IsApproved,
                CreatedAt = user.CreatedAt,
                Role = user.Roles.Select(r => r.Name).FirstOrDefault() ?? RoleNames.User
            }
        };
    }

    public async Task<bool> UsernameExistsAsync(string username) => await _uow.Users.GetByUsernameAsync(username) != null;

    public async Task ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user == null) throw new Exceptions.EntityNotFoundException("Пользователь не найден");
        if (!_hasher.Verify(oldPassword, user.PasswordHash, user.Salt)) throw new Exceptions.BusinessValidationException("Неверный текущий пароль");
        var (hash, salt) = _hasher.Hash(newPassword);
        user.PasswordHash = hash; user.Salt = salt;
        await _uow.Users.UpdateAsync(user);
    }

    public async Task ResetPasswordAsync(string username, string newPassword)
    {
        var user = await _uow.Users.GetByUsernameAsync(username);
        if (user == null) throw new Exceptions.EntityNotFoundException("Пользователь не найден");
        var (hash, salt) = _hasher.Hash(newPassword);
        user.PasswordHash = hash; user.Salt = salt;
        await _uow.Users.UpdateAsync(user);
    }
}
