using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Music.API.Middleware;

namespace Music.API.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        app.UseGlobalExceptionMiddleware();
        app.UseStatusCodePages(async ctx =>
        {
            var res = ctx.HttpContext.Response;
            if (res.StatusCode is StatusCodes.Status401Unauthorized or StatusCodes.Status403Forbidden or StatusCodes.Status404NotFound or StatusCodes.Status405MethodNotAllowed)
            {
                var problem = new ProblemDetails
                {
                    Status = res.StatusCode,
                    Title = res.StatusCode switch
                    {
                        StatusCodes.Status401Unauthorized => "Не авторизован",
                        StatusCodes.Status403Forbidden => "Доступ запрещён",
                        StatusCodes.Status404NotFound => "Ресурс не найден",
                        StatusCodes.Status405MethodNotAllowed => "Метод не поддерживается",
                        _ => "Ошибка"
                    }
                };
                res.ContentType = "application/problem+json";
                await res.WriteAsJsonAsync(problem, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            }
        });
        app.UseSwagger();
        app.UseSwaggerUI(o => { o.SwaggerEndpoint("/swagger/v1/swagger.json", "Music Portal API v1"); o.DefaultModelsExpandDepth(-1); });
        app.UseHttpsRedirection();
        app.UseCors("ApiCorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }
}
