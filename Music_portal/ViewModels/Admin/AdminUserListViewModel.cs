using Music.bisLog.Dtos;

namespace Music_portal.ViewModels.Admin;

public class AdminUserListViewModel
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public List<UserDto> Users { get; set; } = new();
}