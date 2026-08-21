namespace Music.bisLog.Dtos;

public class UserListDto
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public List<UserDto> Users { get; set; } = new();
}