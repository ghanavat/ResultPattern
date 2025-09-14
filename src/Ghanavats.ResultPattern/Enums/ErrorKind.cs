using Ghanavats.ResultPattern.Attributes;

namespace Ghanavats.ResultPattern.Enums;

public enum ErrorKind
{
    [ProblemDetailsType("https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1")] Unknown = 0,
    [ProblemDetailsType("https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10")] Conflict, // version/constraint conflicts
    [ProblemDetailsType("https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2")] Unauthorized,
    [ProblemDetailsType("https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4")] Forbidden,
    [ProblemDetailsType("https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status/429")] RateLimited,
    [ProblemDetailsType("https://datatracker.ietf.org/doc/html/rfc9110#name-504-gateway-timeout")] Timeout,
    [ProblemDetailsType("https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.3")] DependencyFailure, // downstream system failed
    [ProblemDetailsType("https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10")] BusinessRule // domain rule violated (non-validation)
}
