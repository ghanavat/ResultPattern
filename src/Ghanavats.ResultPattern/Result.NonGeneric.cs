using FluentValidation.Results;
using Ghanavats.ResultPattern.Aggregate;
using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Ghanavats.ResultPattern;

/// <summary>
/// A different variant of the global Result.
/// Use this when you want to populate/return Result without having to specify its T 'Data' type.
/// </summary>
public class Result : Result<Result>
{
    /// <summary>
    /// Private constructor that is used in this class.
    /// No need to set Status here as its default is OK.Í
    /// </summary>
    private Result()
    {
    }

    /// <summary>
    /// A constructor that accepts <paramref name="status"/>
    /// </summary>
    /// <param name="status"></param>
    private Result(ResultStatus status) : base(status)
    {
    }

    /// <summary>
    /// Represents a successful operation without a return type
    /// </summary>
    /// <returns>A Success Result</returns>
    public static Result Success() => new();

    /// <summary>
    /// Creates a failed <see cref="Result"/> with the specified error message and classification.
    /// </summary>
    /// <param name="errorMessage">
    /// A descriptive message explaining the cause of the error. 
    /// This value will be included in the internal <c>ErrorMessages</c> collection and may be mapped 
    /// into <see cref="ProblemDetails.Detail"/> during API response generation.
    /// </param>
    /// <param name="errorKind">
    /// An optional classification of the error that determines which HTTP status code 
    /// is emitted when mapped to an API response.
    /// Defaults to <see cref="ErrorKind.Unknown"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> representing a failed operation.
    /// The <see cref="ResultStatus"/> will be <c>Error</c>.
    /// </returns>
    /// <remarks>
    /// Use this method when an operation fails due to business logic or an application-level error.  
    /// For validation-related errors, prefer <c>Invalid</c> instead,
    /// as it maps to <see cref="ValidationProblemDetails"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// return Result.Error("Database connection failed", ErrorKind.Database);
    /// </code>
    /// </example>
    public new static Result Error(string errorMessage, ErrorKind errorKind = ErrorKind.Unknown) =>
        new(ResultStatus.Error)
        {
            ErrorMessages = [errorMessage],
            Kind = errorKind
        };

    /// <summary>
    /// Creates an <see cref="Result"/> representing an invalid operation,
    /// based on the supplied <see cref="FluentValidation.Results.ValidationResult"/>.
    /// </summary>
    /// <param name="validationResult">
    /// The <see cref="FluentValidation.Results.ValidationResult"/> produced by FluentValidation.  
    /// Its validation failures are transformed into a <c>ValidationErrorsByField</c> dictionary,
    /// which is later mapped into <see cref="ValidationProblemDetails.Errors"/> when returning API responses.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> with <see cref="ResultStatus.Invalid"/>.  
    /// This result indicates that input validation failed and should be returned to the API consumer
    /// as <see cref="ValidationProblemDetails"/>.
    /// </returns>
    /// <remarks>
    /// This method is tightly integrated with FluentValidation.  
    /// If you are not using FluentValidation, you should construct the dictionary of validation errors 
    /// manually and assign it to <c>ValidationErrorsByField</c>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var validator = new UserValidator();
    /// var validationResult = validator.Validate(userInput);
    ///
    /// if (!validationResult.IsValid)
    /// {
    ///     var result = Result.Invalid(validationResult);
    ///     return await result.ToActionResultAsync(controller); // maps to ValidationProblemDetails
    /// }
    /// </code>
    /// </example>
    public new static Result Invalid(ValidationResult validationResult)
    {
        return new Result(ResultStatus.Invalid)
        {
            ValidationErrorsByField = validationResult.ToDictionary().AsReadOnly()
        };
    }
    
    /// <summary>
    /// Creates an <see cref="Result"/> representing an invalid operation
    /// using the provided field-to-messages dictionary.
    /// </summary>
    /// <param name="validationErrors">
    /// A dictionary of validation errors where the key is the field or property name,
    /// and the value is an array of validation error messages associated with that field.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> with <see cref="ResultStatus.Invalid"/> status,
    /// containing the supplied <paramref name="validationErrors"/> in 
    /// <see cref="Result{T}.ValidationErrorsByField"/>.
    /// </returns>
    /// <remarks>
    /// This overload is useful when you are not using FluentValidation, or when you
    /// already have validation errors represented as a dictionary.  
    /// The dictionary is defensively copied to ensure immutability and prevent external modification.
    /// </remarks>
    /// <example>
    /// <code>
    /// var errors = new Dictionary&lt;string, string[]&gt;
    /// {
    ///     ["Email"] = new[] { "Email is required." },
    ///     ["Password"] = new[] { "Password must be at least 8 characters." }
    /// };
    ///
    /// var result = Result.Invalid(errors);
    /// if (result.IsInvalid())
    /// {
    ///     // result.ValidationErrorsByField now contains the errors
    /// }
    /// </code>
    /// </example>
    public new static Result Invalid(IDictionary<string, string[]> validationErrors)
    {
        // Defensively copy to prevent external mutation and normalise comparer
        var copy = new Dictionary<string, string[]>(validationErrors.Count, StringComparer.Ordinal);
        
        foreach (var kvp in validationErrors)
            copy[kvp.Key] = kvp.Value.ToArray();

        return new Result(ResultStatus.Invalid)
        {
            ValidationErrorsByField = copy
        };
    }

    /// <summary>
    /// Creates a <see cref="Result"/> representing a not found outcome.
    /// </summary>
    /// <returns>
    /// A <see cref="Result"/> instance with <see cref="ResultStatus.NotFound"/> status,
    /// indicating that the requested resource could not be found.
    /// </returns>
    /// <example>
    /// <code>
    /// var result = Result.NotFound();
    /// if (result.IsNotFound())
    /// {
    ///     Console.WriteLine("Customer not found.");
    /// }
    /// </code>
    /// </example>
    public new static Result NotFound() => new(ResultStatus.NotFound);

    /// <inheritdoc cref="AggregateBase.AggregateResults" />
    public static IReadOnlyCollection<AggregateResultsModel> Aggregate(params Result[] results)
    {
        return AggregateBase.AggregateResults(results);
    }
}
