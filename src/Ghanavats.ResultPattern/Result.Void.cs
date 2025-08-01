﻿using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Extensions;
using Ghanavats.ResultPattern.Models;

namespace Ghanavats.ResultPattern;

/// <summary>
/// A different variant of the global Result.
/// Use this when you want to populate/return Result without having to specify its T 'Data' type.
/// </summary>
public class Result : Result<Result>
{
    /// <summary>
    /// Private constructor that is used in this class.
    /// No need to set Status here as its default is OK.
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
    /// Represents invalid result with validation errors.
    /// </summary>
    /// <param name="validationErrors"></param>
    /// <returns>An Invalid Result</returns>
    public new static Result Invalid(IEnumerable<ValidationError> validationErrors)
    {
        return new Result(ResultStatus.Invalid)
        {
            ValidationErrors = validationErrors
        };
    }

    /// <summary>
    /// Represents the error result with the error message
    /// </summary>
    /// <returns>An Error Result</returns>
    public new static Result Error(string errorMessage) => new(ResultStatus.Error)
    {
        ErrorMessages = [errorMessage]
    };

    /// <summary>
    /// Represents the not found result for non-generic scenarios
    /// </summary>
    /// <returns>Result with NotFound status</returns>
    public new static Result NotFound() => new(ResultStatus.NotFound);

    /// <summary>
    /// To gather all non-success results of type Result (non-generic) into a single object.
    /// </summary>
    /// <param name="results">Array of all Results (non-generic)</param>
    /// <returns>A Read-Only Collection of Aggregate Results Model</returns>
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
                    _ => throw new NotSupportedException($"Status '{whatIWant.Key}' is not supported.")
                },
                OriginalResults = whatIWant.ToList().AsReadOnly() // stored quietly for future use
            }).ToList().AsReadOnly();
    }
}
