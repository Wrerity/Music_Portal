using Music.bisLog.Dtos;
using Music.bisLog.Services;
using Music.DataAccess.Models;

namespace Music_portal.Tests;

public class UserServiceTests : ServiceTestBase
{
    private async Task<User> CreateRegisteredUserAsync(string username = "user1")
    {
        var auth = CreateAuthService();
        await auth.RegisterAsync(new RegisterRequestDto { Username = username, Password = "Pass123" });
        return (await Uow.Users.GetByUsernameAsync(username))!;
    }

    [Fact]
    public async Task ActivateUser_SetsApproved()
    {
        var user = await CreateRegisteredUserAsync();
        var service = CreateUserService();

        var result = await service.ActivateUserAsync(new ActivateUserDto { UserId = user.Id });

        Assert.True(result.Success);
        var updated = await Uow.Users.GetByIdAsync(user.Id);
        Assert.True(updated!.IsApproved);
    }

    [Fact]
    public async Task ActivateUser_MissingUser_Fails()
    {
        var service = CreateUserService();

        var result = await service.ActivateUserAsync(new ActivateUserDto { UserId = 9999 });

        Assert.False(result.Success);
    }

    [Fact]
    public async Task RejectUser_DeletesPendingUser()
    {
        var user = await CreateRegisteredUserAsync();
        var service = CreateUserService();

        var result = await service.RejectUserAsync(user.Id);

        Assert.True(result.Success);
        Assert.Null(await Uow.Users.GetByIdAsync(user.Id));
    }

    [Fact]
    public async Task GetPending_ReturnsOnlyUnapproved()
    {
        var pending = await CreateRegisteredUserAsync("pending1");
        var active = await CreateRegisteredUserAsync("active1");
        var approved = await Uow.Users.GetByUsernameAsync("active1");
        approved!.IsApproved = true;
        await Uow.Users.UpdateAsync(approved);

        var service = CreateUserService();
        var pendingList = await service.GetPendingAsync();

        Assert.Contains(pendingList, u => u.Id == pending.Id);
        Assert.DoesNotContain(pendingList, u => u.Id == active.Id);
    }

    [Fact]
    public async Task CreateUser_AppliesRole()
    {
        await Uow.Roles.AddAsync(new Role { Name = "Admin" });
        var service = CreateUserService();

        var result = await service.CreateAsync(new CreateUserDto
        {
            Username = "admin1",
            Password = "Pass123",
            Role = "Admin",
            IsApproved = true
        });

        Assert.True(result.Success);
        var dto = await service.GetUserAsync((await Uow.Users.GetByUsernameAsync("admin1"))!.Id);
        Assert.NotNull(dto);
        Assert.Equal("Admin", dto!.Role);
        Assert.True(dto.IsApproved);
    }

    [Fact]
    public async Task DeleteUser_AdminUser_Rejected()
    {
        await Uow.Roles.AddAsync(new Role { Name = "Admin" });
        var service = CreateUserService();
        await service.CreateAsync(new CreateUserDto
        {
            Username = "admin1",
            Password = "Pass123",
            Role = "Admin",
            IsApproved = true
        });
        var admin = await Uow.Users.GetByUsernameAsync("admin1");

        var result = await service.DeleteAsync(admin!.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteUser_RegularUser_Completes()
    {
        var user = await CreateRegisteredUserAsync();
        var service = CreateUserService();

        var result = await service.DeleteAsync(user.Id);

        Assert.True(result.Success);
        Assert.Null(await Uow.Users.GetByIdAsync(user.Id));
    }
}