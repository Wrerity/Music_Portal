using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Music.bisLog.Dtos;
using Music.DataAccess.Utils;

namespace Music.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected int GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : 0;
    }

    protected static ProblemDetails ApiProblem(int status, string title, string detail)
    {
        return new ProblemDetails { Status = status, Title = title, Detail = detail };
    }

    protected IActionResult FromResult(OperationResult result)
    {
        if (result.Success)
            return Ok(result);

        var isNotFound = result.Error.Contains("не найдена", StringComparison.OrdinalIgnoreCase)
            || result.Error.Contains("не найден", StringComparison.OrdinalIgnoreCase);

        return isNotFound
            ? NotFound(ApiProblem(StatusCodes.Status404NotFound, "Ресурс не найден", result.Error))
            : BadRequest(ApiProblem(StatusCodes.Status400BadRequest, "Ошибка запроса", result.Error));
    }

    protected IActionResult? ValidateAudioFile(IFormFile? file, bool required)
    {
        if (file == null || file.Length == 0)
        {
            return required
                ? BadRequest(ApiProblem(StatusCodes.Status400BadRequest, "Файл не загружен", "Необходимо загрузить аудиофайл"))
                : null;
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!FileExtensions.Allowed.Contains(ext))
        {
            return BadRequest(ApiProblem(
                StatusCodes.Status400BadRequest,
                "Недопустимый формат",
                $"Разрешены только файлы: {string.Join(", ", FileExtensions.Allowed)}"));
        }

        if (file.Length > FileLimits.MaxFileSize)
        {
            return BadRequest(ApiProblem(
                StatusCodes.Status400BadRequest,
                "Файл слишком большой",
                $"Максимальный размер файла — {FileLimits.MaxFileSize / (1024 * 1024)} МБ"));
        }

        return null;
    }
}