using FluentValidation.Results;
using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Extensions;

/// <summary>
/// Fluent Validation Result extension
/// </summary>
public static class FluentValidationFailureExtension
{
    public static IEnumerable<ValidationError> PopulateValidationErrors(this ValidationResult? input)
    {
        if (input is null)
        {
            return [];
        }
        
        return input.Errors.Select(x => new ValidationError
        {
            ErrorCode = x.ErrorCode,
            ErrorMessage = x.ErrorMessage,
            ValidationErrorType = (ValidationErrorType)x.Severity
        });
    }
}
