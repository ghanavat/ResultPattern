using System.ComponentModel;
using System.Diagnostics;
using Ghanavats.ResultPattern.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ghanavats.ResultPattern.Mapping;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class ResultMappingBase
{
    internal static IActionResult ToActionResultBase(this Result result, ControllerBase controller)
    {
        var statusCode = MappingErrorKind.MapStatusCode(result);

        switch (result.Status)
        {
            case ResultStatus.Ok:
                return controller.Ok(result);
            case ResultStatus.Error:
                return controller.StatusCode(statusCode, new ProblemDetails
                {
                    Title = "There has been a problem with your request.",
                    Detail = result.ErrorMessages.FirstOrDefault(),
                    Instance = controller.HttpContext.Request.Path.Value,
                    Status = statusCode,
                    Extensions = { ["TraceId"] = controller.HttpContext.TraceIdentifier }
                });
            case ResultStatus.NotFound:
                return controller.NotFound(new ProblemDetails
                {
                    Title = "Not found.",
                    Detail = "The requested resource was not found.",
                    Instance = controller.HttpContext.Request.Path.Value,
                    Status = statusCode
                });
            case ResultStatus.Invalid:
                var validationProblemDetails = new ValidationProblemDetails();
                
                if (result.ValidationErrorsByField is { Count: > 0 } dict)
                {
                    foreach (var kvp in dict)
                    {
                        validationProblemDetails.Errors[kvp.Key] = kvp.Value;
                    }
                }

                validationProblemDetails.Title = "Invalid request.";
                validationProblemDetails.Detail = "Your request is invalid. The details are populated in Errors.";
                validationProblemDetails.Instance = controller.HttpContext.Request.Path.Value;
                validationProblemDetails.Status = statusCode;
                
                return controller.BadRequest(validationProblemDetails);
            default:
                return controller.StatusCode(statusCode);
        }
    }

    internal static IActionResult ToActionResultBase<T>(this Result<T> result, ControllerBase controller)
    {
        var statusCode = MappingErrorKind.MapStatusCode(result);

        switch (result.Status)
        {
            case ResultStatus.Ok:
                return controller.Ok(result);
            case ResultStatus.Error:
                return controller.StatusCode(statusCode, new ProblemDetails
                {
                    Title = "There has been a problem with your request.",
                    Detail = result.ErrorMessages.FirstOrDefault(),
                    Instance = controller.HttpContext.Request.Path.Value,
                    Status = statusCode,
                    Extensions = { ["TraceId"] = controller.HttpContext.TraceIdentifier }
                });
            case ResultStatus.NotFound:
                return controller.NotFound(new ProblemDetails
                {
                    Title = "Not found.",
                    Detail = "The requested resource was not found.",
                    Instance = controller.HttpContext.Request.Path.Value,
                    Status = statusCode
                });
            case ResultStatus.Invalid:
                var validationProblemDetails = new ValidationProblemDetails();
                
                if (result.ValidationErrorsByField is { Count: > 0 } dict)
                {
                    foreach (var kvp in dict)
                    {
                        validationProblemDetails.Errors[kvp.Key] = kvp.Value;
                    }
                }

                validationProblemDetails.Title = "Invalid request.";
                validationProblemDetails.Detail = "Your request is invalid. The details are populated in Errors.";
                validationProblemDetails.Instance = controller.HttpContext.Request.Path.Value;
                validationProblemDetails.Status = statusCode;
                
                return controller.BadRequest(validationProblemDetails);
            default:
                return controller.StatusCode(statusCode);
        }
    }

    internal static IResult ToResultBase<T>(this Result<T> result)
    {
        var statusCode = MappingErrorKind.MapStatusCode(result);

        switch (result.Status)
        {
            case ResultStatus.Ok:
                return Results.Ok(result);
            case ResultStatus.Error:
                return Results.Problem(new ProblemDetails
                {
                    Title = $"{result.Kind.ToString()}. There has been a problem with your request.",
                    Detail = result.ErrorMessages.FirstOrDefault(),
                    Status = statusCode,
                    Extensions = { ["TraceId"] = Activity.Current?.TraceId.ToString() }
                });
            case ResultStatus.NotFound:
                return Results.NotFound(new ProblemDetails
                {
                    Title = "Not found.",
                    Detail = "The requested resource was not found.",
                    Status = statusCode,
                    Instance = string.Empty
                });
            case ResultStatus.Invalid:
                var errors = result.ValidationErrorsByField != null 
                             && result.ValidationErrorsByField.Any() ? result.ValidationErrorsByField
                    : new Dictionary<string, string[]>(StringComparer.Ordinal);

                return Results.ValidationProblem(errors.ToDictionary(),
                    "Your request is invalid. The details are populated in Errors.", 
                    "instance", statusCode, "Invalid request.");
            default:
                return Results.Problem();
        }
    }

    internal static IResult ToResultBase(this Result result)
    {
        var statusCode = MappingErrorKind.MapStatusCode(result);

        switch (result.Status)
        {
            case ResultStatus.Ok:
                return Results.Ok(result);
            case ResultStatus.Error:
                return Results.Problem(new ProblemDetails
                {
                    Title = $"{result.Kind.ToString()}. There has been a problem with your request.",
                    Detail = result.ErrorMessages.FirstOrDefault(),
                    Status = statusCode,
                    Extensions = { ["TraceId"] = Activity.Current?.TraceId.ToString() }
                });
            case ResultStatus.NotFound:
                return Results.NotFound(new ProblemDetails
                {
                    Title = "Not found.",
                    Detail = "The requested resource was not found.",
                    Status = statusCode,
                    Instance = string.Empty
                });
            case ResultStatus.Invalid:
                var errors = result.ValidationErrorsByField != null
                             && result.ValidationErrorsByField.Any()
                    ? result.ValidationErrorsByField
                    : new Dictionary<string, string[]>(StringComparer.Ordinal);

                return Results.ValidationProblem(errors.ToDictionary(),
                    "Your request is invalid. The details are populated in Errors.",
                    "instance", statusCode, "Invalid request.");
            default:
                return Results.Problem();
        }
    }
}
