using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.DataAccess.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
}
