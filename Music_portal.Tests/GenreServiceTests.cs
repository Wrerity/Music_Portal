using Music.bisLog.Dtos;
using Music.bisLog.Services;

namespace Music_portal.Tests;

public class GenreServiceTests : ServiceTestBase
{
    [Fact]
    public async Task Create_ThenGet_ReturnsGenre()
    {
        var service = CreateGenreService();

        var result = await service.CreateAsync(new GenreDto { Name = "Rock", Description = "Rock genre" });

        Assert.True(result.Success);
        var genres = await service.GetAllAsync();
        var genre = Assert.Single(genres);
        Assert.Equal("Rock", genre.Name);
    }

    [Fact]
    public async Task Update_ChangesGenre()
    {
        var service = CreateGenreService();
        await service.CreateAsync(new GenreDto { Name = "Rock" });
        var id = (await service.GetAllAsync()).Single().Id;

        var result = await service.UpdateAsync(new GenreDto { Id = id, Name = "Jazz" });

        Assert.True(result.Success);
        var updated = await service.GetByIdAsync(id);
        Assert.Equal("Jazz", updated!.Name);
    }

    [Fact]
    public async Task Delete_RemovesGenre()
    {
        var service = CreateGenreService();
        await service.CreateAsync(new GenreDto { Name = "Rock" });
        var id = (await service.GetAllAsync()).Single().Id;

        var result = await service.DeleteAsync(id);

        Assert.True(result.Success);
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task Count_ReturnsTotal()
    {
        var service = CreateGenreService();
        await service.CreateAsync(new GenreDto { Name = "Rock" });
        await service.CreateAsync(new GenreDto { Name = "Jazz" });

        Assert.Equal(2, await service.CountAsync());
    }
}