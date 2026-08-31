using Music.bisLog.Dtos;
using Music.bisLog.Exceptions;
using Music.bisLog.Services;

namespace Music_portal.Tests;

public class GenreServiceTests : ServiceTestBase
{
    [Fact]
    public async Task Create_ThenGet_ReturnsGenre()
    {
        var service = CreateGenreService();
        var genre = await service.CreateAsync(new GenreDto { Name = "Rock", Description = "Rock genre" });
        Assert.Equal("Rock", genre.Name);
        var genres = await service.GetAllAsync();
        var single = Assert.Single(genres);
        Assert.Equal("Rock", single.Name);
    }

    [Fact]
    public async Task Create_Duplicate_Throws()
    {
        var service = CreateGenreService();
        await service.CreateAsync(new GenreDto { Name = "Rock" });
        await Assert.ThrowsAsync<GenreAlreadyExistsException>(() => service.CreateAsync(new GenreDto { Name = "Rock" }));
        await Assert.ThrowsAsync<GenreAlreadyExistsException>(() => service.CreateAsync(new GenreDto { Name = "rock" }));
    }

    [Fact]
    public async Task Search_ReturnsFiltered()
    {
        var service = CreateGenreService();
        await service.CreateAsync(new GenreDto { Name = "Rock" });
        await service.CreateAsync(new GenreDto { Name = "Jazz" });
        var result = await service.SearchAsync("ro");
        Assert.Single(result);
        Assert.Equal("Rock", result[0].Name);
    }

    [Fact]
    public async Task Update_ChangesGenre()
    {
        var service = CreateGenreService();
        await service.CreateAsync(new GenreDto { Name = "Rock" });
        var id = (await service.GetAllAsync()).Single().Id;
        var updated = await service.UpdateAsync(new GenreDto { Id = id, Name = "Jazz" });
        Assert.Equal("Jazz", updated.Name);
        var fromDb = await service.GetByIdAsync(id);
        Assert.Equal("Jazz", fromDb!.Name);
    }

    [Fact]
    public async Task Update_Duplicate_Throws()
    {
        var service = CreateGenreService();
        await service.CreateAsync(new GenreDto { Name = "Rock" });
        await service.CreateAsync(new GenreDto { Name = "Jazz" });
        var rockId = (await service.SearchAsync("Rock")).Single().Id;
        await Assert.ThrowsAsync<GenreAlreadyExistsException>(() => service.UpdateAsync(new GenreDto { Id = rockId, Name = "Jazz" }));
    }

    [Fact]
    public async Task Delete_RemovesGenre()
    {
        var service = CreateGenreService();
        await service.CreateAsync(new GenreDto { Name = "Rock" });
        var id = (await service.GetAllAsync()).Single().Id;
        await service.DeleteAsync(id);
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task Delete_NotFound_Throws()
    {
        var service = CreateGenreService();
        await Assert.ThrowsAsync<EntityNotFoundException>(() => service.DeleteAsync(9999));
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
