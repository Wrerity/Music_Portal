using Microsoft.EntityFrameworkCore;
using Music.API.Auth;
using Music.API.Configuration;
using Music.DataAccess.Data;
using Music.bisLog.Services;

namespace Music.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerWithJwt();
        services.AddProblemDetails();
        services.AddAutoMapper(typeof(Music.bisLog.MappingProfile));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<ISongService, SongService>();
        services.AddSingleton<PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(o => o.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        services.Configure<UploadOptions>(config.GetSection("Uploads"));
        services.Configure<BootstrapAdminOptions>(config.GetSection("BootstrapAdmin"));
        services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
        services.AddJwtAuthentication(config);
        var origins = config.GetSection("Cors:AllowedOrigins").Get<string[]>();
        services.AddCors(o =>
        {
            if (origins is { Length: > 0 } && origins.Any(x => x != "*"))
                o.AddPolicy("ApiCorsPolicy", p => p.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            else
                o.AddPolicy("ApiCorsPolicy", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });
        return services;
    }
}
