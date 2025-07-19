using FluentValidation.Results;
using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Extensions;

/// <summary>
/// Fluent Validation Result extension to populate Validation Error collection
/// </summary>
public static class FluentValidationFailureExtension
{
    public static IEnumerable<ValidationError> PopulateValidationErrors(this ValidationResult? input)
    {
        return input is null
            ? []
            : input.Errors
                .Select(validationFailure => new ValidationError(validationFailure.ErrorMessage,
                    validationFailure.ErrorCode,
                    (ValidationErrorType)validationFailure.Severity));
    }
}
