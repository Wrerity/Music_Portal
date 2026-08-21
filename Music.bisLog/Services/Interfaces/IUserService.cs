using Music.bisLog.Dtos;

namespace Music.bisLog.Services;

public interface IUserService
{
    Task<UserDto?> GetUserAsync(int userId);
    Task<UserListDto> GetUsersAsync(string? search, int page, int pageSize);
    Task<List<UserDto>> GetPendingAsync();
    Task<int> CountAsync();
    Task<int> CountPendingAsync();
    Task<OperationResult> CreateAsync(CreateUserDto dto);
    Task<OperationResult> UpdateAsync(UpdateUserDto dto);
    Task<OperationResult> DeleteAsync(int userId);
    Task<OperationResult> ActivateUserAsync(ActivateUserDto dto);
    Task<OperationResult> RejectUserAsync(int userId);
}