using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Aggregate;

public static class AggregateFeatures
{
    /// <summary>
    /// Enhances aggregated results by replacing validation messages with full 
    /// validation error details for <see cref="ResultStatus.Invalid"/> results.
    /// </summary>
    /// <param name="aggregated">
    /// The aggregated collection produced by <see cref="Result.Aggregate(Result[])"/>.
    /// </param>
    /// <returns>
    /// A new read-only collection of <see cref="AggregateResultsModel"/> objects. 
    /// For <see cref="ResultStatus.Invalid"/> results, <see cref="AggregateResultsModel.ValidationErrorsPair"/> 
    /// is populated with detailed validation errors,
    /// and <see cref="AggregateResultsModel.Messages"/> is cleared.
    /// </returns>
    /// <remarks>
    /// Use this method when you require full validation detail, such as for API responses, 
    /// while avoiding duplication between <see cref="AggregateResultsModel.Messages"/> 
    /// and <see cref="AggregateResultsModel.ValidationErrorsPair"/>.
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
        //TODO Reconsider this functionality. You can populate the Dictionary<string, string[]> by default.
        return aggregated.Select(result =>
        {
            if (result.Status != ResultStatus.Invalid
                || result.OriginalResults.Count == 0)
            {
                return result;
            }

            return result with
            {
                Messages = [],
                ValidationErrorsPair = result.OriginalResults
                    .Where(r => r.Status == ResultStatus.Invalid)
                    .SelectMany(r => r.ValidationErrorsByField ?? new Dictionary<string, string[]>())
                    .ToList().AsReadOnly()
            };
        }).ToList().AsReadOnly();
    }

    internal static IReadOnlyCollection<string> GetErrorMessages(this IEnumerable<Result> results)
    {
        return results
            .Where(result => result.Status == ResultStatus.Error
                             && result.ErrorMessages.Any())
            .SelectMany(error => error.ErrorMessages).ToList();
    }

    internal static IReadOnlyCollection<string> GetValidationMessages(this IEnumerable<Result> results)
    {
        return results
            .Where(invalids => invalids.Status == ResultStatus.Invalid)
            .SelectMany(invalidMessages => invalidMessages.ValidationErrorsByField != null
                ? invalidMessages.ValidationErrorsByField.Values
                : [])
            .SelectMany(validationMessages => validationMessages)
            .ToList().AsReadOnly();
    }
}
