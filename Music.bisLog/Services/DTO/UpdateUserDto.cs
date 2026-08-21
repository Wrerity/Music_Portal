using Music.DataAccess.Utils;

namespace Music.bisLog.Dtos;

public class UpdateUserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string Role { get; set; } = RoleNames.User;
    public bool IsApproved { get; set; }
}