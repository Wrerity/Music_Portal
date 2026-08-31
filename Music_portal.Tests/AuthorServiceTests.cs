using Music.bisLog.Dtos;
using Music.bisLog.Exceptions;
using Music.bisLog.Services;

namespace Music_portal.Tests;

public class AuthorServiceTests : ServiceTestBase
{
    [Fact]
    public async Task Create_ThenGet_ReturnsAuthor()
    {
        var service = CreateAuthorService();
        var author = await service.CreateAsync(new AuthorDto { Name = "Artist", Country = "RU" });
        Assert.Equal("Artist", author.Name);
        var authors = await service.GetAllAsync();
        var single = Assert.Single(authors);
        Assert.Equal("Artist", single.Name);
    }

    [Fact]
    public async Task Create_Duplicate_Throws()
    {
        var service = CreateAuthorService();
        await service.CreateAsync(new AuthorDto { Name = "Artist" });
        await Assert.ThrowsAsync<AuthorAlreadyExistsException>(() => service.CreateAsync(new AuthorDto { Name = "Artist" }));
    }

    [Fact]
    public async Task Update_ChangesAuthor()
    {
        var service = CreateAuthorService();
        await service.CreateAsync(new AuthorDto { Name = "Artist" });
        var id = (await service.GetAllAsync()).Single().Id;
        var updated = await service.UpdateAsync(new AuthorDto { Id = id, Name = "Renamed", Country = "US" });
        Assert.Equal("Renamed", updated.Name);
        var fromDb = await service.GetByIdAsync(id);
        Assert.Equal("Renamed", fromDb!.Name);
    }

    [Fact]
    public async Task Delete_RemovesAuthor()
    {
        var service = CreateAuthorService();
        await service.CreateAsync(new AuthorDto { Name = "Artist" });
        var id = (await service.GetAllAsync()).Single().Id;
        await service.DeleteAsync(id);
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task Delete_NotFound_Throws()
    {
        var service = CreateAuthorService();
        await Assert.ThrowsAsync<EntityNotFoundException>(() => service.DeleteAsync(9999));
    }

    [Fact]
    public async Task Count_ReturnsTotal()
    {
        var service = CreateAuthorService();
        await service.CreateAsync(new AuthorDto { Name = "A" });
        await service.CreateAsync(new AuthorDto { Name = "B" });
        Assert.Equal(2, await service.CountAsync());
    }
}
