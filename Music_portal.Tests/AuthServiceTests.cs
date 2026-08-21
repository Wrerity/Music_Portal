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

        var result = await auth.RegisterAsync(new RegisterRequestDto { Username = "user1", Password = "Pass123" });

        Assert.True(result.Success);
        var user = await Uow.Users.GetByUsernameAsync("user1");
        Assert.NotNull(user);
        Assert.False(user!.IsApproved);
    }

    [Fact]
    public async Task Register_DuplicateUsername_Fails()
    {
        var auth = CreateAuthService();
        await auth.RegisterAsync(new RegisterRequestDto { Username = "user1", Password = "Pass123" });

        var result = await auth.RegisterAsync(new RegisterRequestDto { Username = "user1", Password = "Pass123" });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Login_UnapprovedUser_Fails()
    {
        var auth = CreateAuthService();
        await auth.RegisterAsync(new RegisterRequestDto { Username = "user1", Password = "Pass123" });

        var result = await auth.LoginAsync(new LoginRequestDto { Username = "user1", Password = "Pass123" });

        Assert.False(result.Success);
        Assert.Contains("подтверждена", result.Error);
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
    public async Task Login_WrongPassword_Fails()
    {
        var auth = CreateAuthService();
        await auth.RegisterAsync(new RegisterRequestDto { Username = "user1", Password = "Pass123" });
        var user = await Uow.Users.GetByUsernameAsync("user1");
        user!.IsApproved = true;
        await Uow.Users.UpdateAsync(user);

        var result = await auth.LoginAsync(new LoginRequestDto { Username = "user1", Password = "Wrong" });

        Assert.False(result.Success);
    }
}