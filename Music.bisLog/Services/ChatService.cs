using Music.bisLog.Dtos;
using Music.bisLog.Exceptions;
using Music.DataAccess.Models;
using Music.DataAccess.Repositories;

namespace Music.bisLog.Services;

public class ChatService
{
    private readonly IChatRepository _repo;
    public ChatService(IChatRepository repo) => _repo = repo;

    public async Task<ChatUserDto> RegisterOrUpdateAsync(string username, string connectionId)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new BusinessValidationException("Имя пользователя обязательно");
        var existing = await _repo.GetByUsernameAsync(username);
        if (existing != null)
        {
            await _repo.UpdateConnectionIdAsync(existing.Id, connectionId);
            existing.ConnectionId = connectionId;
            return Map(existing);
        }
        var user = new ChatUser { Username = username.Trim(), ConnectionId = connectionId };
        var created = await _repo.AddUserAsync(user);
        return Map(created);
    }

    public async Task SetOfflineAsync(string connectionId)
    {
        var u = await _repo.GetByConnectionIdAsync(connectionId);
        if (u != null) await _repo.UpdateConnectionIdAsync(u.Id, null);
    }

    public async Task<ChatMessageDto> SendAllAsync(string senderConnectionId, string content, int? roomId = 1)
    {
        if (string.IsNullOrWhiteSpace(content)) throw new BusinessValidationException("Сообщение не может быть пустым");
        var sender = await _repo.GetByConnectionIdAsync(senderConnectionId) ?? throw new EntityNotFoundException("Отправитель не найден");
        var msg = new ChatMessage { SenderId = sender.Id, Content = content.Trim(), IsPrivate = false, RoomId = roomId };
        var saved = await _repo.AddMessageAsync(msg);
        return new ChatMessageDto(saved.Id, sender.Id, sender.Username, null, null, roomId, saved.Content, saved.Timestamp, false);
    }

    public async Task<ChatMessageDto> SendPrivateAsync(string senderConnectionId, string receiverConnectionId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) throw new BusinessValidationException("Сообщение не может быть пустым");
        var sender = await _repo.GetByConnectionIdAsync(senderConnectionId) ?? throw new EntityNotFoundException("Отправитель не найден");
        var receiver = await _repo.GetByConnectionIdAsync(receiverConnectionId) ?? throw new EntityNotFoundException("Получатель не найден");
        var msg = new ChatMessage { SenderId = sender.Id, ReceiverId = receiver.Id, Content = content.Trim(), IsPrivate = true };
        var saved = await _repo.AddMessageAsync(msg);
        return new ChatMessageDto(saved.Id, sender.Id, sender.Username, receiver.Id, receiver.Username, null, saved.Content, saved.Timestamp, true);
    }

    public async Task<List<ChatUserDto>> GetActiveUsersAsync() => (await _repo.GetAllActiveAsync()).Select(Map).ToList();

    public async Task<List<ChatMessageDto>> GetHistoryAsync(int? roomId = 1, int count = 50, string? forConnectionId = null)
    {
        int? forUserId = null;
        if (forConnectionId != null)
        {
            var u = await _repo.GetByConnectionIdAsync(forConnectionId);
            forUserId = u?.Id;
        }
        var list = await _repo.GetRecentAsync(roomId, count, forUserId);
        return list.Select(m => new ChatMessageDto(m.Id, m.SenderId, m.Sender.Username, m.ReceiverId, m.Receiver?.Username, m.RoomId, m.Content, m.Timestamp, m.IsPrivate)).ToList();
    }

    private static ChatUserDto Map(ChatUser u) => new(u.Id, u.Username, u.ConnectionId, u.IsOnline, u.LastSeen);
}
