using Microsoft.Extensions.Options;
using Music.API.Configuration;
using Music.DataAccess.Data;
using Music.DataAccess.Models;
using Music.DataAccess.Utils;
using Music.bisLog.Services;

namespace Music.API.Infrastructure;

public static class AdminSeeder
{
    public static void Seed(WebApplication app)
    {
        var opts = app.Services.GetRequiredService<IOptions<BootstrapAdminOptions>>().Value;
        if (!opts.Enabled || string.IsNullOrWhiteSpace(opts.Username) || string.IsNullOrWhiteSpace(opts.Password)) return;
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        using var scope = app.Services.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var hasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();
        var existing = uow.Users.GetByUsernameAsync(opts.Username).GetAwaiter().GetResult();
        if (existing != null)
        {
            logger.LogInformation("Пользователь {Username} уже существует, инициализация пропущена", opts.Username);
            return;
        }
        var role = uow.Roles.GetByNameAsync(RoleNames.Admin).GetAwaiter().GetResult();
        var (hash, salt) = hasher.Hash(opts.Password);
        var user = new User { Username = opts.Username, PasswordHash = hash, Salt = salt, IsApproved = true, CreatedAt = DateTime.UtcNow };
        if (role != null) user.Roles.Add(role);
        uow.Users.AddAsync(user).GetAwaiter().GetResult();
        logger.LogWarning("Создан администратор {Username} из BootstrapAdmin", opts.Username);
    }
}
