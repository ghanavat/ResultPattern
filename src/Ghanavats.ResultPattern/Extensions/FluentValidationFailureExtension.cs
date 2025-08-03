using FluentValidation.Results;
using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Extensions;

/// <summary>
/// Fluent Validation Result extension to populate Validation Error collection
/// </summary>
public static class FluentValidationFailureExtension
{
    /// <summary>
    /// Converts a <see cref="FluentValidation.Results.ValidationResult"/> into a collection of 
    /// <see cref="ValidationError"/> instances that can be used with the <c>ResultPattern</c>.
    /// </summary>
    /// <param name="input">
    /// The <see cref="FluentValidation.Results.ValidationResult"/> produced by FluentValidation.
    /// </param>
    /// <returns>
    /// A read-only collection of <see cref="ValidationError"/> objects representing the validation failures.
    /// Each <see cref="ValidationError"/> includes the property name and the associated error message.
    /// </returns>
    /// <remarks>
    /// This extension provides a convenient way to bridge FluentValidation with the <c>ResultPattern</c>,
    /// allowing validation failures to be returned consistently as part of a <c>Result{T}</c>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var validator = new UserValidator();
    /// var validationResult = validator.Validate(user);
    /// 
    /// if (!validationResult.IsValid)
    /// {
    ///     var errors = validationResult.ToValidationErrors();
    ///     return Result&lt;User&gt;.Invalid(errors);
    /// }
    /// </code>
    /// </example>
    public static IEnumerable<ValidationError> PopulateValidationErrors(this ValidationResult input)
    {
        return input.Errors
                .Select(validationFailure => new ValidationError(validationFailure.ErrorMessage,
                    validationFailure.ErrorCode,
                    Enum.Parse<ValidationErrorType>(validationFailure.Severity.ToString())));
    }
}
