using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.DataAccess.Repositories;

public interface IAuthorRepository : IRepository<Author>
{
    Task<int> GetSongCountAsync(int authorId);
}
