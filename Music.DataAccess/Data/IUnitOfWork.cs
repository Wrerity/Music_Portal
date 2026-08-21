using Music.DataAccess.Repositories;

namespace Music.DataAccess.Data;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ISongRepository Songs { get; }
    IGenreRepository Genres { get; }
    IAuthorRepository Authors { get; }
    IRoleRepository Roles { get; }
    Task<int> SaveChangesAsync();
}
