using Music.bisLog.Dtos;
using Music.bisLog.Services;
using Music.DataAccess.Models;

namespace Music_portal.Tests;

public class SongServiceTests : ServiceTestBase
{
    private const string UploadsDirName = "uploads";

    private async Task<(User owner, int genreId, int authorId)> SeedActiveUserWithDefaultsAsync()
    {
        var auth = CreateAuthService();
        await auth.RegisterAsync(new RegisterRequestDto { Username = "owner", Password = "Pass123" });
        var user = (await Uow.Users.GetByUsernameAsync("owner"))!;
        user.IsApproved = true;
        await Uow.Users.UpdateAsync(user);

        var genre = await Uow.Genres.AddAsync(new Genre { Name = "Rock" });
        var author = await Uow.Authors.AddAsync(new Author { Name = "Artist" });

        return (user, genre.Id, author.Id);
    }

    private CreateSongDto BuildCreateDto(int userId, int genreId, int authorId)
    {
        return new CreateSongDto
        {
            Title = "Song",
            UserId = userId,
            Duration = 180,
            Lyrics = "Lyrics",
            AuthorIds = new[] { authorId },
            GenreIds = new[] { genreId },
            AudioFileName = "song.mp3",
            AudioStream = new MemoryStream(new byte[] { 1, 2, 3, 4 })
        };
    }

    [Fact]
    public async Task Create_ByUnapprovedUser_Fails()
    {
        await CreateAuthService().RegisterAsync(new RegisterRequestDto { Username = "owner", Password = "Pass123" });
        var user = (await Uow.Users.GetByUsernameAsync("owner"))!;
        var genre = await Uow.Genres.AddAsync(new Genre { Name = "Rock" });
        var author = await Uow.Authors.AddAsync(new Author { Name = "Artist" });
        var service = CreateSongService();

        var result = await service.CreateAsync(BuildCreateDto(user.Id, genre.Id, author.Id));

        Assert.False(result.Success);
        Assert.Contains("подтверждена", result.Error);
    }

    [Fact]
    public async Task Create_ByApprovedUser_Succeeds()
    {
        var (user, genreId, authorId) = await SeedActiveUserWithDefaultsAsync();
        var service = CreateSongService();

        var result = await service.CreateAsync(BuildCreateDto(user.Id, genreId, authorId));

        Assert.True(result.Success);
        var songs = await Uow.Songs.GetByUserIdAsync(user.Id);
        var song = Assert.Single(songs);
        Assert.Equal("Song", song.Title);
    }

    [Fact]
    public async Task Edit_ByNonOwner_Fails()
    {
        var (owner, genreId, authorId) = await SeedActiveUserWithDefaultsAsync();
        var service = CreateSongService();
        await service.CreateAsync(BuildCreateDto(owner.Id, genreId, authorId));
        var song = (await Uow.Songs.GetByUserIdAsync(owner.Id)).Single();

        await CreateAuthService().RegisterAsync(new RegisterRequestDto { Username = "intruder", Password = "Pass123" });
        var intruder = (await Uow.Users.GetByUsernameAsync("intruder"))!;

        var result = await service.UpdateAsync(new UpdateSongDto
        {
            Id = song.Id,
            UserId = intruder.Id,
            Title = "Hijacked"
        });

        Assert.False(result.Success);
        Assert.Contains("не найдена", result.Error);
    }

    [Fact]
    public async Task Delete_ByNonOwner_Fails()
    {
        var (owner, genreId, authorId) = await SeedActiveUserWithDefaultsAsync();
        var service = CreateSongService();
        await service.CreateAsync(BuildCreateDto(owner.Id, genreId, authorId));
        var song = (await Uow.Songs.GetByUserIdAsync(owner.Id)).Single();

        await CreateAuthService().RegisterAsync(new RegisterRequestDto { Username = "intruder", Password = "Pass123" });
        var intruder = (await Uow.Users.GetByUsernameAsync("intruder"))!;

        var result = await service.DeleteAsync(new DeleteSongDto { SongId = song.Id, UserId = intruder.Id });

        Assert.False(result.Success);
        Assert.Contains("чужую", result.Error);
        Assert.NotNull(await Uow.Songs.GetByIdAsync(song.Id));
    }

    [Fact]
    public async Task Edit_GetEditData_ByNonOwner_ReturnsNull()
    {
        var (owner, genreId, authorId) = await SeedActiveUserWithDefaultsAsync();
        var service = CreateSongService();
        await service.CreateAsync(BuildCreateDto(owner.Id, genreId, authorId));
        var song = (await Uow.Songs.GetByUserIdAsync(owner.Id)).Single();

        await CreateAuthService().RegisterAsync(new RegisterRequestDto { Username = "intruder", Password = "Pass123" });
        var intruder = (await Uow.Users.GetByUsernameAsync("intruder"))!;

        var data = await service.GetEditDataAsync(song.Id, intruder.Id);

        Assert.Null(data);
    }

    [Fact]
    public async Task Download_IncrementsPlayCount()
    {
        var (owner, genreId, authorId) = await SeedActiveUserWithDefaultsAsync();
        var service = CreateSongService();
        await service.CreateAsync(BuildCreateDto(owner.Id, genreId, authorId));
        var song = (await Uow.Songs.GetByUserIdAsync(owner.Id)).Single();

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), UploadsDirName);
        Directory.CreateDirectory(uploadsDir);
        var fakePath = Path.Combine(uploadsDir, song.FilePath);
        await File.WriteAllBytesAsync(fakePath, new byte[] { 0, 1, 2, 3 });

        var result = await service.DownloadAsync(song.Id);

        Assert.True(result.Success);
        var reloaded = (await Uow.Songs.GetByIdWithDetailsAsync(song.Id))!;
        Assert.Equal(1, reloaded.PlayCount);
        File.Delete(fakePath);
    }

    [Fact]
    public async Task AdminUpdate_ChangesSong()
    {
        var (owner, genreId, authorId) = await SeedActiveUserWithDefaultsAsync();
        var service = CreateSongService();
        await service.CreateAsync(BuildCreateDto(owner.Id, genreId, authorId));
        var song = (await Uow.Songs.GetByUserIdAsync(owner.Id)).Single();

        var result = await service.AdminUpdateAsync(new AdminUpdateSongDto
        {
            Id = song.Id,
            UserId = owner.Id,
            Title = "Renamed",
            Duration = 120,
            AuthorIds = new[] { authorId },
            GenreIds = new[] { genreId }
        });

        Assert.True(result.Success);
        var updated = (await Uow.Songs.GetByIdWithDetailsAsync(song.Id))!;
        Assert.Equal("Renamed", updated.Title);
    }

    [Fact]
    public async Task AdminDelete_RemovesSong()
    {
        var (owner, genreId, authorId) = await SeedActiveUserWithDefaultsAsync();
        var service = CreateSongService();
        await service.CreateAsync(BuildCreateDto(owner.Id, genreId, authorId));
        var song = (await Uow.Songs.GetByUserIdAsync(owner.Id)).Single();

        var result = await service.AdminDeleteAsync(song.Id);

        Assert.True(result.Success);
        Assert.Null(await Uow.Songs.GetByIdAsync(song.Id));
    }
}