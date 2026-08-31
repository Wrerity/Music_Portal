using Music.bisLog.Dtos;

namespace Music.bisLog.Services;

public interface IUserService
{
    Task<UserDto?> GetUserAsync(int userId);
    Task<UserListDto> GetUsersAsync(string? search, int page, int pageSize);
    Task<List<UserDto>> GetPendingAsync();
    Task<int> CountAsync();
    Task<int> CountPendingAsync();
    // Бизнес-слой возвращает DTO или выбрасывает исключение, без Success/Error флагов
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto> UpdateAsync(UpdateUserDto dto);
    Task DeleteAsync(int userId);
    Task ActivateUserAsync(ActivateUserDto dto);
    Task RejectUserAsync(int userId);
}