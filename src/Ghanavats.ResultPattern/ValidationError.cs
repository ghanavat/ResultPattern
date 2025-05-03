using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern;

public class ValidationError
{
    internal ValidationError(string errorMessage, string errorCode, ValidationErrorType validationError)
    {
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        ValidationErrorType = validationError;
    }

    public string? ErrorMessage { get; init; }
    public string? ErrorCode { get; init; }
    public ValidationErrorType ValidationErrorType { get; init; }
}
