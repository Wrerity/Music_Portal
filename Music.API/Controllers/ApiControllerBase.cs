using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

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
}