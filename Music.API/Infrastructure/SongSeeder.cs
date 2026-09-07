using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Data;
using Music.DataAccess.Models;

namespace Music.API.Infrastructure;

/// <summary>
/// Сидер 40+ пустышек для проверки пагинации (15/стр → 3 страницы)
/// </summary>
public static class SongSeeder
{
    public static async Task SeedAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        // Идемпотентно: если уже 40+ — пропускаем
        var count = await db.Songs.CountAsync();
        if (count >= 45) // 5 из миграции + 40
        {
            logger.LogInformation("SongSeeder: уже {Count} песен, пропуск", count);
            return;
        }

        var demoUser = await db.Users.FirstOrDefaultAsync(u => u.Username == "demo");
        if (demoUser == null) demoUser = await db.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        if (demoUser == null)
        {
            logger.LogWarning("SongSeeder: не найден demo/admin пользователь, пропуск");
            return;
        }

        var genres = await db.Genres.ToListAsync();
        var authors = await db.Authors.ToListAsync();
        if (genres.Count == 0 || authors.Count == 0)
        {
            logger.LogWarning("SongSeeder: нет жанров/авторов, пропуск");
            return;
        }

        var rnd = new Random(42);
        var toAdd = 45 - count;
        if (toAdd <= 0) return;
        if (toAdd > 40) toAdd = 40;
        for (int i = 1; i <= toAdd; i++)
        {
            var idx = count + i;
            var g = genres[rnd.Next(genres.Count)];
            var a = authors[rnd.Next(authors.Count)];
            var song = new Song
            {
                Title = $"Test Track {idx:D2}",
                User = demoUser,
                FilePath = $"dummy_{idx:D2}.mp3",
                Duration = 180 + rnd.Next(120),
                Lyrics = $"Lyrics for test track {idx}",
                PlayCount = rnd.Next(5000),
                CreatedAt = DateTime.UtcNow.AddMinutes(-idx * 10)
            };
            song.Genres.Add(g);
            song.Authors.Add(a);
            if (rnd.Next(3) == 0)
            {
                var g2 = genres[rnd.Next(genres.Count)];
                if (g2.Id != g.Id) song.Genres.Add(g2);
            }
            db.Songs.Add(song);
        }
        await db.SaveChangesAsync();

        var total = await db.Songs.CountAsync();
        logger.LogInformation("SongSeeder: добавлено {Added}, всего {Total}", toAdd, total);
    }
}
