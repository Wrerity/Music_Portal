namespace Music.DataAccess.Models;

public class ChatUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? ConnectionId { get; set; }
    public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    public bool IsOnline => !string.IsNullOrEmpty(ConnectionId);
    public ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
}
