using Microsoft.AspNetCore.Http;


namespace Ghanavats.ResultPattern.ProblemDetailsHandlers;

internal sealed class BetterProblemDetailsFactory<TProblem> :
    IResult,
    IStatusCodeHttpResult,
    IContentTypeHttpResult,
    IValueHttpResult<TProblem>
    where TProblem : Microsoft.AspNetCore.Mvc.ProblemDetails
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
        Value.Instance = httpContext.Request.Path;
        Value.Extensions["TraceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = StatusCode ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = ContentType;
        return httpContext.Response.WriteAsJsonAsync(Value);
    }
}
