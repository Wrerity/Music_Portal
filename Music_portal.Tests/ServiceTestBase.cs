using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Music.DataAccess.Data;
using Music.bisLog;
using Music.bisLog.Services;

namespace Music_portal.Tests;

public abstract class ServiceTestBase : IDisposable
{
    protected readonly AppDbContext Db;
    protected readonly IUnitOfWork Uow;
    protected readonly IMapper Mapper;
    protected readonly PasswordHasher Hasher;

    protected ServiceTestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        Db = new AppDbContext(options);
        Uow = new UnitOfWork(Db);
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        Mapper = config.CreateMapper();
        Hasher = new PasswordHasher();
    }

    protected IAuthService CreateAuthService() => new AuthService(Uow, Hasher, Mapper);
    protected IUserService CreateUserService() => new UserService(Uow, Hasher, Mapper, NullLogger<UserService>.Instance);
    protected IGenreService CreateGenreService() => new GenreService(Uow, Mapper);
    protected IAuthorService CreateAuthorService() => new AuthorService(Uow, Mapper);
    protected ISongService CreateSongService() => new SongService(Uow, CreateGenreService(), Mapper);

    public void Dispose()
    {
        Db.Dispose();
    }
}