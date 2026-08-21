using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Music.API.Auth;
using Music.API.Configuration;
using Music.API.Middleware;
using Music.DataAccess.Data;
using Music.DataAccess.Models;
using Music.DataAccess.Utils;
using Music.bisLog.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(Music.bisLog.MappingProfile));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddSingleton<PasswordHasher>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.Configure<UploadOptions>(builder.Configuration.GetSection("Uploads"));
builder.Services.Configure<BootstrapAdminOptions>(builder.Configuration.GetSection("BootstrapAdmin"));

builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddCors(options =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
    if (origins is { Length: > 0 } && origins.Any(o => o != "*"))
    {
        options.AddPolicy("ApiCorsPolicy", policy =>
            policy.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader());
    }
    else
    {
        options.AddPolicy("ApiCorsPolicy", policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    }
});

var app = builder.Build();

EnsureUploadsDirectory(app);
BootstrapAdmin(app);

app.UseGlobalExceptionMiddleware();

app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    if (response.StatusCode is StatusCodes.Status401Unauthorized
        or StatusCodes.Status403Forbidden
        or StatusCodes.Status404NotFound
        or StatusCodes.Status405MethodNotAllowed)
    {
        var problem = new ProblemDetails
        {
            Status = response.StatusCode,
            Title = response.StatusCode switch
            {
                StatusCodes.Status401Unauthorized => "Не авторизован",
                StatusCodes.Status403Forbidden => "Доступ запрещён",
                StatusCodes.Status404NotFound => "Ресурс не найден",
                StatusCodes.Status405MethodNotAllowed => "Метод не поддерживается",
                _ => "Ошибка"
            }
        };
        response.ContentType = "application/problem+json";
        await response.WriteAsJsonAsync(problem, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Music Portal API v1");
    options.DefaultModelsExpandDepth(-1);
});

app.UseHttpsRedirection();
app.UseCors("ApiCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void EnsureUploadsDirectory(WebApplication app)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var options = app.Services.GetRequiredService<IOptions<UploadOptions>>().Value;

    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), FilePaths.UploadFolder);

    if (Directory.Exists(uploadsPath))
    {
        logger.LogInformation("Папка загрузок найдена: {Path}", uploadsPath);
        return;
    }

    var sharedPath = options.SharedPath;
    if (!string.IsNullOrWhiteSpace(sharedPath) && Directory.Exists(sharedPath))
    {
        var fullShared = Path.GetFullPath(sharedPath);
        try
        {
            var psi = new ProcessStartInfo("cmd.exe", $"/c mklink /J \"{uploadsPath}\" \"{fullShared}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };
            using var process = Process.Start(psi);
            process?.WaitForExit(10_000);

            if (Directory.Exists(uploadsPath))
            {
                logger.LogInformation("Папка загрузок связана с общей папкой: {Path}", fullShared);
                return;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Не удалось создать junction для папки загрузок");
        }
    }

    Directory.CreateDirectory(uploadsPath);
    logger.LogInformation("Папка загрузок создана: {Path}", uploadsPath);
}

static void BootstrapAdmin(WebApplication app)
{
    var options = app.Services.GetRequiredService<IOptions<BootstrapAdminOptions>>().Value;
    if (!options.Enabled || string.IsNullOrWhiteSpace(options.Username) || string.IsNullOrWhiteSpace(options.Password))
        return;

    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    using var scope = app.Services.CreateScope();
    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    var hasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();

    var existing = uow.Users.GetByUsernameAsync(options.Username).GetAwaiter().GetResult();
    if (existing != null)
    {
        logger.LogInformation("Пользователь {Username} уже существует, инициализация администратора пропущена", options.Username);
        return;
    }

    var role = uow.Roles.GetByNameAsync(RoleNames.Admin).GetAwaiter().GetResult();
    var (hash, salt) = hasher.Hash(options.Password);

    var user = new User
    {
        Username = options.Username,
        PasswordHash = hash,
        Salt = salt,
        IsApproved = true,
        CreatedAt = DateTime.UtcNow
    };

    if (role != null)
        user.Roles.Add(role);

    uow.Users.AddAsync(user).GetAwaiter().GetResult();

    logger.LogWarning("Создан администратор {Username} из конфигурации BootstrapAdmin", options.Username);
}