using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.DataAccess.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext _db;
    public ChatRepository(AppDbContext db) => _db = db;

    public async Task<ChatUser> AddUserAsync(ChatUser user)
    {
        _db.ChatUsers.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<ChatUser?> GetByConnectionIdAsync(string connectionId)
        => await _db.ChatUsers.FirstOrDefaultAsync(u => u.ConnectionId == connectionId);

    public async Task<ChatUser?> GetByUsernameAsync(string username)
        => await _db.ChatUsers.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<ChatUser?> GetByIdAsync(int id)
        => await _db.ChatUsers.FindAsync(id);

    public async Task UpdateConnectionIdAsync(int userId, string? connectionId)
    {
        var u = await _db.ChatUsers.FindAsync(userId);
        if (u == null) return;
        u.ConnectionId = connectionId;
        u.LastSeen = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<List<ChatUser>> GetAllActiveAsync()
        => await _db.ChatUsers.Where(u => u.ConnectionId != null).ToListAsync();

    public async Task<ChatMessage> AddMessageAsync(ChatMessage message)
    {
        _db.ChatMessages.Add(message);
        await _db.SaveChangesAsync();
        return message;
    }

    public async Task<List<ChatMessage>> GetRecentAsync(int? roomId, int count, int? forUserId = null)
    {
        var q = _db.ChatMessages.Include(m => m.Sender).Include(m => m.Receiver).AsQueryable();
        if (roomId != null)
            q = q.Where(m => m.RoomId == roomId);
        // Общие + личные для пользователя
        if (forUserId != null)
            q = q.Where(m => !m.IsPrivate || m.SenderId == forUserId || m.ReceiverId == forUserId);
        else
            q = q.Where(m => !m.IsPrivate);
        return await q.OrderByDescending(m => m.Timestamp).Take(count).OrderBy(m => m.Timestamp).ToListAsync();
    }
}
