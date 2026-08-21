namespace Music.DataAccess.Models;

public class Author
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Country { get; set; }

    public string? Description { get; set; }

    public ICollection<Song> Songs { get; set; } = new List<Song>();
}
