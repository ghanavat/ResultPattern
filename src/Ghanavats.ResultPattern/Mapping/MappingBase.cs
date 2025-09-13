using System.ComponentModel;
using Ghanavats.ResultPattern.Constants;
using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ghanavats.ResultPattern.Mapping;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class MappingBase
{
    internal static IActionResult ToActionResultBase(this Result result, ControllerBase controller)
    {
        var statusCode = MappingErrorKind.MapStatusCode(result);

        switch (result.Status)
        {
            case ResultStatus.Ok:
                return controller.Ok(result);
            case ResultStatus.Error:
                var problemDetailsForError = ProblemDetailsBuilder
                    .Build($"{result.Kind.ToString()}. ProblemDetailsConstants.ErrorTitle",
                        result.ErrorMessages.ToList()[0],
                        statusCode);
                
                problemDetailsForError.Extensions["TraceId"] = controller.HttpContext.TraceIdentifier;
                problemDetailsForError.Instance = controller.HttpContext.Request.Path;
                
                return controller.StatusCode(statusCode, problemDetailsForError);
            case ResultStatus.NotFound:
                var problemDetailsForNotFound = ProblemDetailsBuilder
                    .Build(ProblemDetailsConstants.NotFoundTitle, 
                        ProblemDetailsConstants.NotFoundDetail, 
                    statusCode);
                
                problemDetailsForNotFound.Extensions["TraceId"] = controller.HttpContext.TraceIdentifier;
                problemDetailsForNotFound.Instance = controller.HttpContext.Request.Path;
                
                return controller.NotFound(problemDetailsForNotFound);
            case ResultStatus.Invalid:
                var errors = result.ValidationErrorsByField != null
                             && result.ValidationErrorsByField.Any()
                    ? result.ValidationErrorsByField.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    : new Dictionary<string, string[]>(StringComparer.Ordinal);
                
                var validationProblemDetails = ProblemDetailsBuilder.Build(errors, statusCode);
                
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
                var problemDetailsForError = ProblemDetailsBuilder
                    .Build($"{result.Kind.ToString()}. {ProblemDetailsConstants.ErrorTitle}",
                        result.ErrorMessages.ToList()[0],
                        statusCode);
                
                problemDetailsForError.Extensions["TraceId"] = controller.HttpContext.TraceIdentifier;
                problemDetailsForError.Instance = controller.HttpContext.Request.Path;
                
                return controller.StatusCode(statusCode, problemDetailsForError);
            case ResultStatus.NotFound:
                var problemDetailsForNotFound = ProblemDetailsBuilder
                    .Build(ProblemDetailsConstants.NotFoundTitle,
                        ProblemDetailsConstants.NotFoundDetail,
                        statusCode);
                
                problemDetailsForNotFound.Extensions["TraceId"] = controller.HttpContext.TraceIdentifier;
                problemDetailsForNotFound.Instance = controller.HttpContext.Request.Path;
                
                return controller.NotFound(problemDetailsForNotFound);
            case ResultStatus.Invalid:
                var errors = result.ValidationErrorsByField != null
                             && result.ValidationErrorsByField.Any()
                    ? result.ValidationErrorsByField.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    : new Dictionary<string, string[]>(StringComparer.Ordinal);
                
                var validationProblemDetails = ProblemDetailsBuilder.Build(errors, statusCode);
                
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
                return new BetterProblemDetailsFactory<ProblemDetails>(statusCode,
                    ProblemDetailsBuilder
                        .Build($"{result.Kind.ToString()}. {ProblemDetailsConstants.ErrorTitle}",
                            result.ErrorMessages.ToList()[0],
                            statusCode));
            case ResultStatus.NotFound:
                return new BetterProblemDetailsFactory<ProblemDetails>(statusCode,
                    ProblemDetailsBuilder
                        .Build(ProblemDetailsConstants.NotFoundTitle,
                            ProblemDetailsConstants.NotFoundDetail,
                            statusCode));
            case ResultStatus.Invalid:
                var errors = result.ValidationErrorsByField != null
                             && result.ValidationErrorsByField.Any()
                    ? result.ValidationErrorsByField.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    : new Dictionary<string, string[]>(StringComparer.Ordinal);
                
                return new BetterProblemDetailsFactory<ValidationProblemDetails>(statusCode,
                    ProblemDetailsBuilder.Build(errors, statusCode));
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
                return new BetterProblemDetailsFactory<ProblemDetails>(statusCode,
                    ProblemDetailsBuilder
                        .Build($"{result.Kind.ToString()}. {ProblemDetailsConstants.ErrorTitle}",
                            result.ErrorMessages.ToList()[0],
                            statusCode));
            case ResultStatus.NotFound:
                return new BetterProblemDetailsFactory<ProblemDetails>(statusCode,
                    ProblemDetailsBuilder
                        .Build(ProblemDetailsConstants.NotFoundTitle,
                            ProblemDetailsConstants.NotFoundDetail,
                            statusCode));
            case ResultStatus.Invalid:
                var errors = result.ValidationErrorsByField != null
                             && result.ValidationErrorsByField.Any()
                    ? result.ValidationErrorsByField.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                    : new Dictionary<string, string[]>(StringComparer.Ordinal);
                
                return new BetterProblemDetailsFactory<ValidationProblemDetails>(statusCode,
                    ProblemDetailsBuilder.Build(errors, statusCode));
            default:
                return Results.Problem();
        }
    }
}
