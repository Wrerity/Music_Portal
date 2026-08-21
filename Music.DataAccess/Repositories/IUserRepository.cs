using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.DataAccess.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByIdWithRolesAsync(int id);
    Task<PagedResult<User>> GetPagedAsync(string? search, int page, int pageSize);
    Task<int> CountByApprovalStatusAsync(bool isApproved);
    Task<List<User>> GetByApprovalStatusAsync(bool isApproved);
}