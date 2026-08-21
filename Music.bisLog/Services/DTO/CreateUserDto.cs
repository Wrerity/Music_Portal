using Music.DataAccess.Utils;

namespace Music.bisLog.Dtos;

public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = RoleNames.User;
    public bool IsApproved { get; set; }
}