using System.ComponentModel;
using Ghanavats.ResultPattern.Enums;
using Microsoft.AspNetCore.Http;

namespace Ghanavats.ResultPattern.Mapping;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class MappingErrorKind
{
    internal static int MapStatusCode(Result result) => result.Status switch
    {
        ResultStatus.Ok => StatusCodes.Status200OK,
        ResultStatus.Invalid => StatusCodes.Status400BadRequest, // validation
        ResultStatus.NotFound => StatusCodes.Status404NotFound,
        ResultStatus.Error => result.Kind switch
        {
            ErrorKind.Conflict => StatusCodes.Status409Conflict,
            ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
            ErrorKind.RateLimited => StatusCodes.Status429TooManyRequests,
            ErrorKind.Timeout => StatusCodes.Status504GatewayTimeout,
            ErrorKind.DependencyFailure => StatusCodes.Status502BadGateway,
            ErrorKind.BusinessRule => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        },
        _ => StatusCodes.Status500InternalServerError
    };
    
    internal static int MapStatusCode<T>(Result<T> result) => result.Status switch
    {
        ResultStatus.Ok => StatusCodes.Status200OK,
        ResultStatus.Invalid => StatusCodes.Status400BadRequest, // validation
        ResultStatus.NotFound => StatusCodes.Status404NotFound,
        ResultStatus.Error => result.Kind switch
        {
            ErrorKind.Conflict => StatusCodes.Status409Conflict,
            ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
            ErrorKind.RateLimited => StatusCodes.Status429TooManyRequests,
            ErrorKind.Timeout => StatusCodes.Status504GatewayTimeout,
            ErrorKind.DependencyFailure => StatusCodes.Status502BadGateway,
            ErrorKind.BusinessRule => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        },
        _ => StatusCodes.Status500InternalServerError
    };
}
