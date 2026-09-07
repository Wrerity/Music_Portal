using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Music.DataAccess.Data;
using Music.bisLog.Services;
using Music_portal.Auth;
using Music_portal.Filters;
using Music_portal.Resources;

namespace Music_portal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPortalServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddLocalization();
        services.AddControllersWithViews(o => o.Filters.Add<LocalizationFilter>())
            .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(o => o.DataAnnotationLocalizerProvider = (t, f) => f.Create(typeof(SharedResource)));
        services.AddDbContext<AppDbContext>(o => o.UseSqlServer(config.GetConnectionString("DefaultConnection")));
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(o => { o.LoginPath = "/Auth/Login"; o.AccessDeniedPath = "/Home/AccessDenied"; o.ExpireTimeSpan = TimeSpan.FromHours(8); o.SlidingExpiration = true; o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; });
        services.AddAuthorization();
        services.AddAutoMapper(typeof(Music.bisLog.MappingProfile));
        services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<ISongService, SongService>();
        services.AddSingleton<PasswordHasher>();
        return services;
    }
}
