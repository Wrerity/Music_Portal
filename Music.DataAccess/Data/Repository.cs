using Microsoft.EntityFrameworkCore;

namespace Music.DataAccess.Data;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _set;

    public Repository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _set.FindAsync(id);
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await _set.ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        _set.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _set.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(T entity)
    {
        _set.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public virtual async Task<int> CountAsync()
    {
        return await _set.CountAsync();
    }
}
