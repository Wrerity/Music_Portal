using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.DataAccess.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(AppDbContext db) : base(db) { }

    public async Task<int> GetSongCountAsync(int authorId)
    {
        return await _db.Authors
            .Where(a => a.Id == authorId)
            .SelectMany(a => a.Songs)
            .CountAsync();
    }
}