using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Helpers;

public static class ResultStatusHelper
{
    /// <summary>
    /// Gets a value indicating whether this result represents a successful operation.
    /// </summary>
    /// <value><c>true</c> if <see cref="ResultStatus"/> is <see cref="ResultStatus.Ok"/>;
    /// otherwise, <c>false</c>.</value>
    /// <remarks>
    /// When this property is <c>true</c>, the <c>Value</c> is guaranteed to be non-null (by contract).
    /// </remarks>
    public static bool IsSuccess<T>(this Result<T> result) =>  result.Status == ResultStatus.Ok;
    
    /// <summary>
    /// Gets a value indicating whether this result represents an operational error (non-validation failure).
    /// </summary>
    /// <value><c>true</c> if <see cref="ResultStatus"/> is <see cref="ResultStatus.Error"/>;
    /// otherwise, <c>false</c>.</value>
    /// <remarks>
    /// Error results are mapped to <c>ProblemDetails</c> at the HTTP boundary.
    /// Use <c>Kind</c> (when set) to refine transport mapping (e.g., 409/401/403/429/5xx).
    /// </remarks>
    public static bool IsError<T>(this Result<T> result) =>  result.Status == ResultStatus.Error;
    
    /// <summary>
    /// Gets a value indicating whether this result represents a validation failure.
    /// </summary>
    /// <value><c>true</c> if <see cref="ResultStatus"/> is <see cref="ResultStatus.Invalid"/>;
    /// otherwise, <c>false</c>.</value>
    /// <remarks>
    /// Invalid results are mapped to <see cref="Microsoft.AspNetCore.Mvc.ValidationProblemDetails"/> 
    /// at the HTTP boundary.
    /// When <c>true</c>, <c>ValidationErrorsDictionary</c> may be populated
    /// with a field-to-messages map used to initialize <c>ValidationProblemDetails.Errors</c>.
    /// </remarks>
    public static bool IsInvalid<T>(this Result<T> result) =>  result.Status == ResultStatus.Invalid;
    
    /// <summary>
    /// Gets a value indicating whether this result represents a missing resource.
    /// </summary>
    /// <value><c>true</c> if <see cref="ResultStatus"/> is <see cref="ResultStatus.NotFound"/>;
    /// otherwise, <c>false</c>.</value>
    /// <remarks>
    /// Not-found results are mapped to HTTP 404 at the transport boundary using <c>ProblemDetails</c>.
    /// </remarks>
    public static bool IsNotFound<T>(this Result<T> result) =>  result.Status == ResultStatus.NotFound;
}
