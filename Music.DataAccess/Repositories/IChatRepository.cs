using Music.DataAccess.Models;

namespace Music.DataAccess.Repositories;

public interface IChatRepository
{
    Task<ChatUser> AddUserAsync(ChatUser user);
    Task<ChatUser?> GetByConnectionIdAsync(string connectionId);
    Task<ChatUser?> GetByUsernameAsync(string username);
    Task<ChatUser?> GetByIdAsync(int id);
    Task UpdateConnectionIdAsync(int userId, string? connectionId);
    Task<List<ChatUser>> GetAllActiveAsync();
    Task<ChatMessage> AddMessageAsync(ChatMessage message);
    Task<List<ChatMessage>> GetRecentAsync(int? roomId, int count, int? forUserId = null);
}
