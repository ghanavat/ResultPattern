using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Models;

namespace Ghanavats.ResultPattern.Extensions;

public static class ResultExtensions
{    
    public static IReadOnlyCollection<string> GetErrorMessages(this IEnumerable<Result> results)
    {
        var resultErrors = results
            .Where(result => result.Status == ResultStatus.Error
                             && result.ErrorMessages.Any())
            .SelectMany(error => error.ErrorMessages).ToList();

        return resultErrors;
    }

    public static IReadOnlyCollection<string> GetValidationMessages(this IEnumerable<Result> results)
    {
        return results
            .Where(invalids => invalids.ValidationErrors.Any()
                               && invalids.Status == ResultStatus.Invalid)
            .SelectMany(invalidMessages => invalidMessages.ValidationErrors
                .Select(x => x.ErrorMessage)).ToList();
    }
    
    /// <summary>
    /// Enhances aggregated results by replacing validation messages with full 
    /// <see cref="ValidationError"/> details for <see cref="ResultStatus.Invalid"/> results.
    /// </summary>
    /// <param name="aggregated">
    /// The aggregated collection produced by <see cref="Result.Aggregate(Result[])"/>.
    /// </param>
    /// <returns>
    /// A new read-only collection of <see cref="AggregateResultsModel"/> objects. 
    /// For <see cref="ResultStatus.Invalid"/> results, <see cref="AggregateResultsModel.ValidationErrors"/> 
    /// is populated with detailed validation errors,
    /// and <see cref="AggregateResultsModel.Messages"/> is cleared.
    /// </returns>
    /// <remarks>
    /// Use this method when you require full validation detail, such as for API responses, 
    /// while avoiding duplication between <see cref="AggregateResultsModel.Messages"/> 
    /// and <see cref="AggregateResultsModel.ValidationErrors"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var aggregated = Result.Aggregate(results)
    ///                        .WithFullValidationErrors();
    /// </code>
    /// </example>
    public static IReadOnlyCollection<AggregateResultsModel> WithFullValidationErrors(
        this IReadOnlyCollection<AggregateResultsModel> aggregated)
    {
        var updated = aggregated.Select(result =>
        {
            if (result.Status != ResultStatus.Invalid 
                || result.OriginalResults.Count == 0)
            {
                return result;
            }

            return result with
            {
                Messages = [],
                ValidationErrors = result.OriginalResults
                    .Where(r => r.Status == ResultStatus.Invalid)
                    .SelectMany(r => r.ValidationErrors)
                    .ToList()
            };
        });

        return updated.ToList().AsReadOnly();
    }
}
