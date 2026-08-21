namespace Music.DataAccess.Models;

public class Song
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public int Duration { get; set; }

    public string? Lyrics { get; set; }

    public int PlayCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;

    public ICollection<Author> Authors { get; set; } = new List<Author>();

    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
