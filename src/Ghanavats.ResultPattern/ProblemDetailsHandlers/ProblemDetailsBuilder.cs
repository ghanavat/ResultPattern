using System.Runtime.InteropServices;
using Ghanavats.ResultPattern.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Ghanavats.ResultPattern.ProblemDetailsHandlers;

/// <summary>
/// Helper class to build ProblemDetails and ValidationProblemDetails instances.
/// </summary>
internal static class ProblemDetailsBuilder
{
    /// <summary>
    /// Builds a <see cref="ProblemDetails"/> instance based on the provided parameters.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="title">Title string. It is used to create the ProblemDetails.Title.</param>
    /// <param name="detail">Optional parameter.
    /// It must be used only when ProblemDetails is built for Error results.</param>
    /// <param name="statusCode">StatusCode integer to assigning to ProblemDetails.Status.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static Microsoft.AspNetCore.Mvc.ProblemDetails Build(string title, string detail, 
        int statusCode, [Optional]Uri? type)
    {
        return new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = statusCode,
            Type = type?.ToString(),
            Title = title,
            Detail = detail,
        };
    }
    
    /// <summary>
    /// Builds a <see cref="ValidationProblemDetails"/> instance based on the provided parameters.
    /// </summary>
    /// <param name="errors">Dictionary of errors</param>
    /// <param name="statusCode">StatusCode mapped from ErrorKind and used for ValidationProblemDetails.Status</param>
    /// <returns></returns>
    internal static ValidationProblemDetails Build(Dictionary<string, string[]> errors, 
        int statusCode)
    {
        return new ValidationProblemDetails
        {
            Status = statusCode,
            Title = ProblemDetailsConstants.ValidationTitle,
            Detail = ProblemDetailsConstants.ValidationDetail,
            Errors = errors
        };
    }
}
