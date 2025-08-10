using Ghanavats.ResultPattern.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ghanavats.ResultPattern.Extensions;

public static class MappingExtensions
{
    /// <summary>
    /// For Minimal API
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static IResult ToResult(this Result result)
    {
        return result.Status switch
        {
            ResultStatus.Ok => Results.Ok(result),
            ResultStatus.Error => Results.Problem(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = result.Status.ToString(),
                Detail = result.ErrorMessages.Any() ? string.Join("; ", result.ErrorMessages) : null
            }),
            ResultStatus.NotFound => Results.NotFound(result),
            ResultStatus.Invalid => Results.BadRequest(result),
            ResultStatus.None => Results.UnprocessableEntity(),
            _ => Results.Problem()
        };
    }

    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        return result.Status switch
        {
            ResultStatus.Ok => controller.Ok(result),
            ResultStatus.Error => controller.StatusCode(StatusCodes.Status500InternalServerError, 
                new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = result.Status.ToString(),
                Detail = result.ErrorMessages.Any() ? string.Join("; ", result.ErrorMessages) : null
            }),
            ResultStatus.Invalid => controller.BadRequest(result),
            ResultStatus.NotFound => controller.NotFound(result),
            ResultStatus.None => controller.UnprocessableEntity(),
            _ => controller.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
