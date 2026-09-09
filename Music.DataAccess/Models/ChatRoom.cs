namespace Music.DataAccess.Models;

public class ChatRoom
{
    public int Id { get; set; }
    public string Name { get; set; } = "General";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
