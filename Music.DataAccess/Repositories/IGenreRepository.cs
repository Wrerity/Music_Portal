using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.DataAccess.Repositories;

public interface IGenreRepository : IRepository<Genre>
{
    Task<int> GetSongCountAsync(int genreId);
}
