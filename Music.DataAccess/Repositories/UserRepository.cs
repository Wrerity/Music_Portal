using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.DataAccess.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db) { }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByIdWithRolesAsync(int id)
    {
        return await _db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<PagedResult<User>> GetPagedAsync(string? search, int page, int pageSize)
    {
        var query = _db.Users
            .Include(u => u.Roles)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(u => u.Username.Contains(search));

        query = query.OrderBy(u => u.CreatedAt);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<User>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<int> CountByApprovalStatusAsync(bool isApproved)
    {
        return await _db.Users.CountAsync(u => u.IsApproved == isApproved);
    }

    public async Task<List<User>> GetByApprovalStatusAsync(bool isApproved)
    {
        return await _db.Users
            .Include(u => u.Roles)
            .Where(u => u.IsApproved == isApproved)
            .ToListAsync();
    }
}