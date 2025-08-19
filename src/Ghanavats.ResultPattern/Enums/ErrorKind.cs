namespace Ghanavats.ResultPattern.Enums;

public enum ErrorKind
{
    Unknown = 0,
    Validation,        // for Invalid results
    NotFound,          // for missing resources
    Conflict,          // version/constraint conflicts
    Unauthorized,
    Forbidden,
    RateLimited,
    Timeout,
    Concurrency,
    DependencyFailure, // downstream system failed
    BusinessRule       // domain rule violated (non-validation)
}
