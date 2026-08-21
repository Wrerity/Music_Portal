using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.DataAccess.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext db) : base(db) { }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _db.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }
}
