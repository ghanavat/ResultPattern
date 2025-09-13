using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ghanavats.ResultPattern.Helpers;

internal sealed class BetterProblemDetailsFactory<TProblem> :
    IResult,
    IStatusCodeHttpResult,
    IContentTypeHttpResult,
    IValueHttpResult<TProblem>
    where TProblem : ProblemDetails
{
    public int? StatusCode { get; }
    public string ContentType => "application/problem+json";
    public TProblem Value { get; }
    
    public BetterProblemDetailsFactory(int statusCode, TProblem value)
    {
        StatusCode = statusCode;
        Value = value;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        Value.Extensions["TraceId"] = httpContext.TraceIdentifier;
        Value.Instance = httpContext.Request.Path;
        
        httpContext.Response.StatusCode = StatusCode ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = ContentType;
        return httpContext.Response.WriteAsJsonAsync(Value);
    }
}
