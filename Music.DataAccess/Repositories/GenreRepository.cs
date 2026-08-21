using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.DataAccess.Repositories;

public class GenreRepository : Repository<Genre>, IGenreRepository
{
    public GenreRepository(AppDbContext db) : base(db) { }

    public async Task<int> GetSongCountAsync(int genreId)
    {
        return await _db.Genres
            .Where(g => g.Id == genreId)
            .SelectMany(g => g.Songs)
            .CountAsync();
    }
}