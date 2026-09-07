using System.Diagnostics;
using Microsoft.Extensions.Options;
using Music.API.Configuration;
using Music.DataAccess.Utils;

namespace Music.API.Infrastructure;

public static class UploadsInitializer
{
    public static void Initialize(WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        var opts = app.Services.GetRequiredService<IOptions<UploadOptions>>().Value;
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.UploadFolder);
        if (Directory.Exists(uploadsPath))
        {
            logger.LogInformation("Папка загрузок найдена: {Path}", uploadsPath);
            return;
        }
        var sharedPath = opts.SharedPath;
        if (!string.IsNullOrWhiteSpace(sharedPath) && Directory.Exists(sharedPath))
        {
            var fullShared = Path.GetFullPath(sharedPath);
            try
            {
                var psi = new ProcessStartInfo("cmd.exe", $"/c mklink /J \"{uploadsPath}\" \"{fullShared}\"") { CreateNoWindow = true, UseShellExecute = false };
                using var process = Process.Start(psi);
                process?.WaitForExit(10_000);
                if (Directory.Exists(uploadsPath))
                {
                    logger.LogInformation("Папка загрузок связана с общей папкой: {Path}", fullShared);
                    return;
                }
            }
            catch (Exception ex) { logger.LogWarning(ex, "Не удалось создать junction для папки загрузок"); }
        }
        Directory.CreateDirectory(uploadsPath);
        logger.LogInformation("Папка загрузок создана: {Path}", uploadsPath);
    }
}
