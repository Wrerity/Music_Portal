namespace Music.DataAccess.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public ChatUser Sender { get; set; } = null!;
    public int? ReceiverId { get; set; }
    public ChatUser? Receiver { get; set; }
    public int? RoomId { get; set; }
    public ChatRoom? Room { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsPrivate { get; set; }
}
