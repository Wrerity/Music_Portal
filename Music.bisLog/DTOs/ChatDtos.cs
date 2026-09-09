namespace Music.bisLog.Dtos;

public record ChatUserDto(int Id, string Username, string? ConnectionId, bool IsOnline, DateTime LastSeen);
public record ChatMessageDto(int Id, int SenderId, string SenderName, int? ReceiverId, string? ReceiverName, int? RoomId, string Content, DateTime Timestamp, bool IsPrivate);
public record SendMessageDto(string Content, int? RoomId = 1);
public record SendPrivateDto(string ReceiverConnectionId, string Content);
