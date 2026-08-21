using Music.DataAccess.Repositories;

namespace Music.DataAccess.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Users = new UserRepository(db);
        Songs = new SongRepository(db);
        Genres = new GenreRepository(db);
        Authors = new AuthorRepository(db);
        Roles = new RoleRepository(db);
    }

    public IUserRepository Users { get; }
    public ISongRepository Songs { get; }
    public IGenreRepository Genres { get; }
    public IAuthorRepository Authors { get; }
    public IRoleRepository Roles { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
