using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Music_portal.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UsePortalPipeline(this WebApplication app)
    {
        var cultures = new List<CultureInfo> { new("ru"), new("uk"), new("uk-UA") };
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("ru"),
            SupportedCultures = cultures,
            SupportedUICultures = cultures
        });

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseStatusCodePages(ctx =>
        {
            if (ctx.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                ctx.HttpContext.Response.Redirect("/Home/NotFoundPage");
            return Task.CompletedTask;
        });
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
        return app;
    }
}
