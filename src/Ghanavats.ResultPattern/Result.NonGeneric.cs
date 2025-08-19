using FluentValidation.Results;
using Ghanavats.ResultPattern.Aggregate;
using Ghanavats.ResultPattern.Enums;
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
    /// Creates a <see cref="Result"/> representing a not found outcome.
    /// </summary>
    /// <returns>
    /// A <see cref="Result"/> instance with <see cref="ResultStatus.NotFound"/> status,
    /// indicating that the requested resource could not be found.
    /// </returns>
    /// <example>
    /// <code>
    /// var result = Result.NotFound();
    /// if (result.IsNotFound)
    /// {
    ///     Console.WriteLine("Customer not found.");
    /// }
    /// </code>
    /// </example>
    public new static Result NotFound() => new(ResultStatus.NotFound);
    
    /// <summary>
    /// Aggregates a collection of <see cref="Result"/> instances into a grouped summary by <see cref="ResultStatus"/>.
    /// </summary>
    /// <param name="results">
    /// The collection of <see cref="Result"/> objects to aggregate. 
    /// Results with statuses <see cref="ResultStatus.Ok"/> and <see cref="ResultStatus.NotFound"/> are excluded.
    /// </param>
    /// <returns>
    /// A read-only collection of <see cref="AggregateResultsModel"/> objects, 
    /// each representing a grouped result with status and error information.
    /// </returns>
    /// <remarks>
    /// Use <c>Aggregate</c> to collate outcomes from multiple operations, 
    /// such as service calls or validation checks, into a simplified summary.
    /// By default, <see cref="AggregateResultsModel.Messages"/> contains error and validation messages as strings.
    /// For structured validation details, call <see cref="Aggregate.AggregateFeatures.WithFullValidationErrors" />
    /// on the aggregated result.
    /// </remarks>
    /// <example>
    /// <code>
    /// var results = new[]
    /// {
    ///     Result.Success(),
    ///     Result.Error("Database connection failed."),
    ///     Result.Invalid([new ValidationError("Email is required.")])
    /// };
    ///
    /// var aggregated = Result.Aggregate(results);
    /// </code>
    /// </example>
    public static IReadOnlyCollection<AggregateResultsModel> Aggregate(params Result[] results)
    {
        return results
            .GroupBy(result => result.Status)
            .Skip(results.Count(x => x.Status is ResultStatus.Ok or ResultStatus.NotFound))
            .Select(whatIWant => new AggregateResultsModel
            {
                Status = whatIWant.Key,
                Messages = whatIWant.Key switch
                {
                    ResultStatus.Error => whatIWant.GetErrorMessages(),
                    ResultStatus.Invalid => whatIWant.GetValidationMessages(),
                    ResultStatus.Ok
                        or ResultStatus.NotFound =>
                        throw new NotSupportedException($"Status '{whatIWant.Key}' is not supported."),
                    _ => throw new NotSupportedException($"Status '{whatIWant.Key}' is not supported.")
                },
                OriginalResults = whatIWant.ToList().AsReadOnly() // stored quietly for future use
            }).ToList().AsReadOnly();
    }
}
