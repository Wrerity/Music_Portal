using Music.bisLog.Dtos;
using Music.bisLog.Services;

namespace Music_portal.Tests;

public class AuthorServiceTests : ServiceTestBase
{
    [Fact]
    public async Task Create_ThenGet_ReturnsAuthor()
    {
        var service = CreateAuthorService();

        var result = await service.CreateAsync(new AuthorDto { Name = "Artist", Country = "RU" });

        Assert.True(result.Success);
        var authors = await service.GetAllAsync();
        var author = Assert.Single(authors);
        Assert.Equal("Artist", author.Name);
    }

    [Fact]
    public async Task Update_ChangesAuthor()
    {
        var service = CreateAuthorService();
        await service.CreateAsync(new AuthorDto { Name = "Artist" });
        var id = (await service.GetAllAsync()).Single().Id;

        var result = await service.UpdateAsync(new AuthorDto { Id = id, Name = "Renamed", Country = "US" });

        Assert.True(result.Success);
        var updated = await service.GetByIdAsync(id);
        Assert.Equal("Renamed", updated!.Name);
    }

    [Fact]
    public async Task Delete_RemovesAuthor()
    {
        var service = CreateAuthorService();
        await service.CreateAsync(new AuthorDto { Name = "Artist" });
        var id = (await service.GetAllAsync()).Single().Id;

        var result = await service.DeleteAsync(id);

        Assert.True(result.Success);
        Assert.Empty(await service.GetAllAsync());
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