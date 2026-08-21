namespace Music_portal.ViewModels;

public class SongDisplayViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
    public string Genres { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public int PlayCount { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
}
