using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Data;
using Music.DataAccess.Models;
using Music.DataAccess.Utils;

namespace Music.DataAccess.Repositories;

public class SongRepository : Repository<Song>, ISongRepository
{
    public SongRepository(AppDbContext db) : base(db) { }

    public async Task<PagedResult<Song>> GetFilteredAsync(string? search, List<int>? genreIds, List<int>? authorIds, string sortBy, int page, int pageSize)
    {
        var query = _db.Songs
            .Include(s => s.User)
            .Include(s => s.Genres)
            .Include(s => s.Authors)
            .AsQueryable();

        // Модель: фильтрация через EF — IQueryable (фильтрация на стороне БД)
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Title.Contains(search));

        // Фильтрация по жанру (через связь Many-to-Many)
        if (genreIds != null && genreIds.Count != 0)
            query = query.Where(s => s.Genres.Any(g => genreIds.Contains(g.Id)));

        // Фильтрация по исполнителю (автору)
        if (authorIds != null && authorIds.Count != 0)
            query = query.Where(s => s.Authors.Any(a => authorIds.Contains(a.Id)));

        // Модель: сортировка через EF — OrderBy на IQueryable
        query = sortBy switch
        {
            SongSortKeys.Title => query.OrderBy(s => s.Title),
            SongSortKeys.Popularity => query.OrderByDescending(s => s.PlayCount),
            _ => query.OrderByDescending(s => s.CreatedAt),
        };

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<Song>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<PagedResult<Song>> GetAllWithDetailsAsync(string? search, int page, int pageSize)
    {
        var query = _db.Songs
            .Include(s => s.User)
            .Include(s => s.Genres)
            .Include(s => s.Authors)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Title.Contains(search));

        query = query.OrderByDescending(s => s.CreatedAt);

        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<Song>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<Song?> GetByIdWithDetailsAsync(int id)
    {
        return await _db.Songs
            .Include(s => s.User)
            .Include(s => s.Genres)
            .Include(s => s.Authors)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Song>> GetByUserIdAsync(int userId)
    {
        return await _db.Songs
            .Include(s => s.Genres)
            .Include(s => s.Authors)
            .Where(s => s.User.Id == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
}