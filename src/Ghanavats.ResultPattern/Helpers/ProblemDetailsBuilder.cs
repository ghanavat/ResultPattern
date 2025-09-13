using Ghanavats.ResultPattern.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Ghanavats.ResultPattern.Helpers;

/// <summary>
/// Helper class to build ProblemDetails and ValidationProblemDetails instances.
/// </summary>
internal static class ProblemDetailsBuilder
{
    /// <summary>
    /// Builds a <see cref="ProblemDetails"/> instance based on the provided parameters.
    /// </summary>
    /// <param name="title">Title string. It is used to create the ProblemDetails.Title.</param>
    /// <param name="detail">Optional parameter.
    /// It must be used only when ProblemDetails is built for Error results.</param>
    /// <param name="statusCode">StatusCode integer to assigning to ProblemDetails.Status.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static ProblemDetails Build(string title, string detail, 
        int statusCode)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode
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
            Title = ProblemDetailsConstants.ValidationTitle,
            Detail = ProblemDetailsConstants.ValidationDetail,
            Errors = errors,
            Status = statusCode
        };
    }
}
