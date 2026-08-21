namespace Music.DataAccess.Models;

public class User
{
    public int Id { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public string Salt { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public bool IsApproved { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Role> Roles { get; set; } = new List<Role>();

    public ICollection<Song> Songs { get; set; } = new List<Song>();
}
