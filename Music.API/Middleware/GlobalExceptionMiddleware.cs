using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Music.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Music.bisLog.Exceptions.UserNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found {Path}", context.Request.Path);
            await WriteProblemAsync(context, StatusCodes.Status404NotFound, "Ресурс не найден", ex.Message, ex.Code);
        }
        catch (Music.bisLog.Exceptions.InvalidCredentialsException ex)
        {
            _logger.LogWarning(ex, "Invalid credentials {Path}", context.Request.Path);
            await WriteProblemAsync(context, StatusCodes.Status401Unauthorized, "Не авторизован", ex.Message, ex.Code);
        }
        catch (Music.bisLog.Exceptions.UserNotApprovedException ex)
        {
            _logger.LogWarning(ex, "User not approved {Path}", context.Request.Path);
            await WriteProblemAsync(context, StatusCodes.Status403Forbidden, "Доступ запрещён", ex.Message, ex.Code);
        }
        catch (Music.bisLog.Exceptions.BusinessException ex)
        {
            var status = ex switch
            {
                Music.bisLog.Exceptions.EntityNotFoundException => StatusCodes.Status404NotFound,
                Music.bisLog.Exceptions.UserAlreadyExistsException => StatusCodes.Status409Conflict,
                Music.bisLog.Exceptions.GenreAlreadyExistsException => StatusCodes.Status409Conflict,
                Music.bisLog.Exceptions.AuthorAlreadyExistsException => StatusCodes.Status409Conflict,
                Music.bisLog.Exceptions.AccessDeniedException => StatusCodes.Status403Forbidden,
                Music.bisLog.Exceptions.BusinessValidationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };
            _logger.LogWarning(ex, "Business error {Code} {Path}", ex.Code, context.Request.Path);
            await WriteProblemAsync(context, status, "Ошибка запроса", ex.Message, ex.Code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Необработанное исключение при обработке {Method} {Path}",
                context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, "Внутренняя ошибка сервера", "Произошла непредвиденная ошибка. Попробуйте позже.");
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, int status, string title, string detail, string? code = null)
    {
        if (context.Response.HasStarted) return;
        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
        problem.Extensions["code"] = code ?? status.ToString();
        await context.Response.WriteAsJsonAsync(problem, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}