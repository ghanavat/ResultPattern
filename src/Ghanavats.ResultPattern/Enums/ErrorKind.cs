namespace Ghanavats.ResultPattern.Enums;

public enum ErrorKind
{
    Unknown = 0,
    Conflict,          // version/constraint conflicts
    Unauthorized,
    Forbidden,
    RateLimited,
    Timeout,
    DependencyFailure, // downstream system failed
    BusinessRule       // domain rule violated (non-validation)
}
