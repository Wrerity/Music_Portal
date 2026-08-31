using Music.bisLog.Dtos;
using Music.bisLog.Services;
using Music.DataAccess.Models;

namespace Music_portal.Tests;

public class AuthServiceTests : ServiceTestBase
{
    [Fact]
    public async Task Register_CreatesPendingUser()
    {
        var auth = CreateAuthService();
        var user = await auth.RegisterAsync(new RegisterRequestDto { Username = "user1", Password = "Pass123" });
        Assert.Equal("user1", user.Username);
        var dbUser = await Uow.Users.GetByUsernameAsync("user1");
        Assert.NotNull(dbUser);
        Assert.False(dbUser!.IsApproved);
    }

    [Fact]
    public async Task Register_DuplicateUsername_Throws()
    {
        var auth = CreateAuthService();
        await auth.RegisterAsync(new RegisterRequestDto { Username = "user1", Password = "Pass123" });
        await Assert.ThrowsAsync<Music.bisLog.Exceptions.UserAlreadyExistsException>(
            () => auth.RegisterAsync(new RegisterRequestDto { Username = "user1", Password = "Pass123" }));
    }

    [Fact]
    public async Task Login_UnapprovedUser_Throws()
    {
        var auth = CreateAuthService();
        await auth.RegisterAsync(new RegisterRequestDto { Username = "user1", Password = "Pass123" });
        await Assert.ThrowsAsync<Music.bisLog.Exceptions.UserNotApprovedException>(
            () => auth.LoginAsync(new LoginRequestDto { Username = "user1", Password = "Pass123" }));
    }

    [Fact]
    public async Task Login_ApprovedUser_Succeeds()
    {
        var auth = CreateAuthService();
        await auth.RegisterAsync(new RegisterRequestDto { Username = "user1", Password = "Pass123" });
        var user = await Uow.Users.GetByUsernameAsync("user1");
        user!.IsApproved = true;
        await Uow.Users.UpdateAsync(user);
        var result = await auth.LoginAsync(new LoginRequestDto { Username = "user1", Password = "Pass123" });
        Assert.True(result.Success);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("user1", result.User!.Username);
    }

    [Fact]
    public async Task Login_WrongPassword_Throws()
    {
        var auth = CreateAuthService();
        await auth.RegisterAsync(new RegisterRequestDto { Username = "user1", Password = "Pass123" });
        var user = await Uow.Users.GetByUsernameAsync("user1");
        user!.IsApproved = true;
        await Uow.Users.UpdateAsync(user);
        await Assert.ThrowsAsync<Music.bisLog.Exceptions.InvalidCredentialsException>(
            () => auth.LoginAsync(new LoginRequestDto { Username = "user1", Password = "Wrong" }));
    }

    [Fact]
    public async Task Login_UserNotFound_Throws()
    {
        var auth = CreateAuthService();
        await Assert.ThrowsAsync<Music.bisLog.Exceptions.UserNotFoundException>(
            () => auth.LoginAsync(new LoginRequestDto { Username = "nouser", Password = "Pass123" }));
    }
}
